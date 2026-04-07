using System.Collections.Concurrent;
using System.Security.Claims;

public sealed class AppDataStore
{
    private readonly object _sync = new();
    private readonly ConcurrentDictionary<string, Guid> _sessions = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<UserAccount> _users = [];
    private readonly List<Plan> _plans = [];
    private readonly List<Subscription> _subscriptions = [];
    private readonly List<ShortLink> _links = [];
    private readonly List<ClickEvent> _clickEvents = [];
    private readonly List<AdminAuditLog> _auditLogs = [];

    public AppDataStore()
    {
        Seed();
    }

    public IReadOnlyCollection<Plan> Plans => _plans.AsReadOnly();
    public IReadOnlyCollection<AdminAuditLog> AuditLogs => _auditLogs.AsReadOnly();

    public AuthResult Register(string fullName, string email, string password, DateTime nowUtc)
    {
        lock (_sync)
        {
            if (_users.Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            var user = new UserAccount
            {
                Id = Guid.NewGuid(),
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Password = password,
                Role = "User",
                CreatedAtUtc = nowUtc,
                CurrentPlanId = 1,
                EmailConfirmed = true
            };

            _users.Add(user);
            _subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PlanId = 1,
                Status = "Active",
                StartAtUtc = nowUtc,
                EndAtUtc = nowUtc.AddMonths(1)
            });

            return CreateAuthResult(user, nowUtc);
        }
    }

    public AuthResult Login(string email, string password, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = _users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user is null || !string.Equals(user.Password, password, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Sai email hoặc mật khẩu.");
            }

            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Tài khoản đã bị khóa.");
            }

            user.LastLoginAtUtc = nowUtc;
            return CreateAuthResult(user, nowUtc);
        }
    }

    public UserAccount? GetUserByToken(string token)
    {
        if (!_sessions.TryGetValue(token, out var userId))
        {
            return null;
        }

        lock (_sync)
        {
            return _users.FirstOrDefault(x => x.Id == userId);
        }
    }

    public UserAccount GetRequiredUser(ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId))
        {
            throw new InvalidOperationException("Không xác định được người dùng.");
        }

        return _users.First(x => x.Id == userId);
    }

    public object GetCurrentUserProfile(ClaimsPrincipal principal)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var plan = _plans.First(x => x.Id == user.CurrentPlanId);
            var links = _links.Count(x => x.UserId == user.Id);

            return new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Status,
                user.EmailConfirmed,
                user.CreatedAtUtc,
                user.LastLoginAtUtc,
                plan = new
                {
                    plan.Id,
                    plan.Code,
                    plan.Name,
                    plan.LinkLimit,
                    plan.AnalyticsRetentionDays
                },
                usage = new
                {
                    linksUsed = links,
                    linksLimit = plan.LinkLimit
                }
            };
        }
    }

    public object UpdateProfile(ClaimsPrincipal principal, string fullName)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            user.FullName = fullName.Trim();
            return GetCurrentUserProfile(principal);
        }
    }

    public object ChangePassword(ClaimsPrincipal principal, string currentPassword, string newPassword)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            if (!string.Equals(user.Password, currentPassword, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");
            }

            user.Password = newPassword;
            return new { message = "Đổi mật khẩu thành công." };
        }
    }

    public IEnumerable<object> GetUserLinks(ClaimsPrincipal principal)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            return _links
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(MapLinkSummary)
                .ToList();
        }
    }

    public object CreateLink(ClaimsPrincipal principal, CreateLinkInput input, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var plan = _plans.First(x => x.Id == user.CurrentPlanId);
            var usedCount = _links.Count(x => x.UserId == user.Id && x.Status != "Deleted");
            if (usedCount >= plan.LinkLimit)
            {
                throw new InvalidOperationException("Đã vượt giới hạn số lượng link của gói hiện tại.");
            }

            var slug = string.IsNullOrWhiteSpace(input.CustomSlug)
                ? GenerateSlug()
                : input.CustomSlug.Trim().ToLowerInvariant();
            if (_links.Any(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Slug đã tồn tại.");
            }

            var link = new ShortLink
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Slug = slug,
                OriginalUrl = input.OriginalUrl.Trim(),
                ShortUrl = $"https://sho.rt/{slug}",
                Status = "Active",
                Password = string.IsNullOrWhiteSpace(input.Password) ? null : input.Password,
                ExpiredAtUtc = input.ExpiredAtUtc,
                ClickLimit = input.ClickLimit,
                Tag = input.Tag?.Trim() ?? string.Empty,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            _links.Add(link);
            return MapLinkSummary(link);
        }
    }

    public object GetLinkById(ClaimsPrincipal principal, Guid linkId)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var link = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");

            return MapLinkDetail(link);
        }
    }

    public object UpdateLink(ClaimsPrincipal principal, Guid linkId, UpdateLinkInput input, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var link = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");

            link.OriginalUrl = input.OriginalUrl.Trim();
            link.Tag = input.Tag?.Trim() ?? string.Empty;
            link.Password = string.IsNullOrWhiteSpace(input.Password) ? null : input.Password;
            link.ExpiredAtUtc = input.ExpiredAtUtc;
            link.ClickLimit = input.ClickLimit;
            link.UpdatedAtUtc = nowUtc;

            return MapLinkDetail(link);
        }
    }

    public object SetLinkStatus(ClaimsPrincipal principal, Guid linkId, string status, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var link = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");
            link.Status = status;
            link.UpdatedAtUtc = nowUtc;
            return MapLinkSummary(link);
        }
    }

    public object DeleteLink(ClaimsPrincipal principal, Guid linkId, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var link = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");
            link.Status = "Deleted";
            link.UpdatedAtUtc = nowUtc;
            return new { message = "Đã xóa mềm link." };
        }
    }

    public object GetAnalyticsOverview(ClaimsPrincipal principal)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var links = _links.Where(x => x.UserId == user.Id).ToList();
            var linkIds = links.Select(x => x.Id).ToHashSet();
            var clicks = _clickEvents.Where(x => linkIds.Contains(x.LinkId)).ToList();
            var unique = clicks
                .GroupBy(x => $"{x.LinkId}:{x.MaskedIp}:{x.Browser}:{x.ClickedAtUtc:yyyy-MM-dd}")
                .Count();

            var trend = clicks
                .GroupBy(x => x.ClickedAtUtc.Date)
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    date = x.Key.ToString("yyyy-MM-dd"),
                    totalClicks = x.Count(),
                    uniqueClicks = x.GroupBy(y => $"{y.LinkId}:{y.MaskedIp}:{y.Browser}:{y.ClickedAtUtc:yyyy-MM-dd}").Count(),
                    botClicks = x.Count(y => y.IsBot)
                })
                .ToList();

            var topLinks = links
                .Select(link => new
                {
                    link.Id,
                    link.Slug,
                    shortUrl = link.ShortUrl,
                    totalClicks = clicks.Count(x => x.LinkId == link.Id),
                    uniqueClicks = clicks
                        .Where(x => x.LinkId == link.Id)
                        .GroupBy(x => $"{x.MaskedIp}:{x.Browser}:{x.ClickedAtUtc:yyyy-MM-dd}")
                        .Count(),
                    link.Status
                })
                .OrderByDescending(x => x.totalClicks)
                .Take(5)
                .ToList();

            return new
            {
                metrics = new
                {
                    totalClicks = clicks.Count,
                    uniqueClicks = unique,
                    botClicks = clicks.Count(x => x.IsBot),
                    activeLinks = links.Count(x => x.Status == "Active")
                },
                trend,
                topLinks
            };
        }
    }

    public object GetLinkAnalytics(ClaimsPrincipal principal, Guid linkId)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var link = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");

            var clicks = _clickEvents.Where(x => x.LinkId == link.Id).ToList();
            return new
            {
                link = MapLinkSummary(link),
                metrics = new
                {
                    totalClicks = clicks.Count,
                    uniqueClicks = clicks.GroupBy(x => $"{x.MaskedIp}:{x.Browser}:{x.ClickedAtUtc:yyyy-MM-dd}").Count(),
                    botClicks = clicks.Count(x => x.IsBot)
                },
                sources = clicks.GroupBy(x => x.Referrer)
                    .Select(x => new { label = x.Key, value = x.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(5),
                devices = clicks.GroupBy(x => x.Device)
                    .Select(x => new { label = x.Key, value = x.Count() })
                    .OrderByDescending(x => x.value),
                countries = clicks.GroupBy(x => x.Country)
                    .Select(x => new { label = x.Key, value = x.Count() })
                    .OrderByDescending(x => x.value)
                    .Take(5)
            };
        }
    }

    public object GetLinkTimeseries(ClaimsPrincipal principal, Guid linkId)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            _ = _links.FirstOrDefault(x => x.Id == linkId && x.UserId == user.Id)
                ?? throw new InvalidOperationException("Không tìm thấy link.");

            return _clickEvents
                .Where(x => x.LinkId == linkId)
                .GroupBy(x => x.ClickedAtUtc.Date)
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    date = x.Key.ToString("yyyy-MM-dd"),
                    totalClicks = x.Count(),
                    uniqueClicks = x.GroupBy(y => $"{y.MaskedIp}:{y.Browser}:{y.ClickedAtUtc:yyyy-MM-dd}").Count()
                })
                .ToList();
        }
    }

    public object GetCurrentSubscription(ClaimsPrincipal principal)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var subscription = _subscriptions.First(x => x.UserId == user.Id && x.Status == "Active");
            var plan = _plans.First(x => x.Id == subscription.PlanId);
            return new
            {
                subscription.Id,
                subscription.Status,
                subscription.StartAtUtc,
                subscription.EndAtUtc,
                plan
            };
        }
    }

    public object UpgradePlan(ClaimsPrincipal principal, int planId, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = GetRequiredUser(principal);
            var plan = _plans.FirstOrDefault(x => x.Id == planId)
                ?? throw new InvalidOperationException("Gói dịch vụ không tồn tại.");

            foreach (var item in _subscriptions.Where(x => x.UserId == user.Id && x.Status == "Active"))
            {
                item.Status = "Replaced";
                item.EndAtUtc = nowUtc;
            }

            _subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PlanId = plan.Id,
                Status = "Active",
                StartAtUtc = nowUtc,
                EndAtUtc = nowUtc.AddMonths(1)
            });

            user.CurrentPlanId = plan.Id;

            return new
            {
                message = $"Đã nâng cấp lên gói {plan.Name}.",
                plan
            };
        }
    }

    public object GetAdminDashboard()
    {
        lock (_sync)
        {
            var paidUsers = _users.Count(x => x.CurrentPlanId != 1);
            return new
            {
                business = new
                {
                    totalUsers = _users.Count,
                    paidUsers,
                    conversionRate = _users.Count == 0 ? 0 : Math.Round((decimal)paidUsers / _users.Count * 100, 2),
                    monthlyRevenue = _users.Sum(x => _plans.First(y => y.Id == x.CurrentPlanId).MonthlyPrice)
                },
                product = new
                {
                    totalLinks = _links.Count,
                    activeLinks = _links.Count(x => x.Status == "Active"),
                    totalClicks = _clickEvents.Count,
                    uniqueClicks = _clickEvents.GroupBy(x => $"{x.LinkId}:{x.MaskedIp}:{x.Browser}:{x.ClickedAtUtc:yyyy-MM-dd}").Count()
                },
                operations = new
                {
                    botClicks = _clickEvents.Count(x => x.IsBot),
                    suspiciousClicks = _clickEvents.Count(x => x.IsSuspicious),
                    redirectLatencyP95Ms = 47,
                    queueLagSeconds = 2,
                    errorRate = 0.2m
                }
            };
        }
    }

    public IEnumerable<object> GetAdminUsers()
    {
        lock (_sync)
        {
            return _users
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(user =>
                {
                    var plan = _plans.First(p => p.Id == user.CurrentPlanId);
                    var links = _links.Where(x => x.UserId == user.Id).ToList();
                    var linkIds = links.Select(x => x.Id).ToHashSet();
                    return new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.Role,
                        user.Status,
                        user.EmailConfirmed,
                        plan = plan.Name,
                        links = links.Count,
                        totalClicks = _clickEvents.Count(x => linkIds.Contains(x.LinkId)),
                        user.CreatedAtUtc,
                        user.LastLoginAtUtc
                    };
                })
                .ToList();
        }
    }

    public object GetAdminUser(Guid userId)
    {
        lock (_sync)
        {
            var user = _users.FirstOrDefault(x => x.Id == userId) ?? throw new InvalidOperationException("Không tìm thấy user.");
            var plan = _plans.First(x => x.Id == user.CurrentPlanId);
            return new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Status,
                user.EmailConfirmed,
                user.CreatedAtUtc,
                user.LastLoginAtUtc,
                plan,
                subscription = _subscriptions.FirstOrDefault(x => x.UserId == user.Id && x.Status == "Active"),
                links = _links.Where(x => x.UserId == user.Id).Select(MapLinkSummary).ToList()
            };
        }
    }

    public object SetUserStatus(Guid adminId, Guid userId, string status, DateTime nowUtc)
    {
        lock (_sync)
        {
            var user = _users.FirstOrDefault(x => x.Id == userId) ?? throw new InvalidOperationException("Không tìm thấy user.");
            user.Status = status;
            AddAudit(adminId, status == "Locked" ? "LockUser" : "UnlockUser", "User", userId.ToString(), nowUtc);
            return new { user.Id, user.Status };
        }
    }

    public IEnumerable<object> GetAdminLinks()
    {
        lock (_sync)
        {
            return _links
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(link =>
                {
                    var owner = _users.First(x => x.Id == link.UserId);
                    return new
                    {
                        link.Id,
                        link.Slug,
                        link.ShortUrl,
                        link.OriginalUrl,
                        link.Status,
                        owner = owner.Email,
                        totalClicks = _clickEvents.Count(x => x.LinkId == link.Id),
                        botClicks = _clickEvents.Count(x => x.LinkId == link.Id && x.IsBot),
                        abuseScore = Math.Min(100, _clickEvents.Count(x => x.LinkId == link.Id && x.IsSuspicious) * 10),
                        link.CreatedAtUtc
                    };
                })
                .ToList();
        }
    }

    public object GetAdminLink(Guid linkId)
    {
        lock (_sync)
        {
            var link = _links.FirstOrDefault(x => x.Id == linkId) ?? throw new InvalidOperationException("Không tìm thấy link.");
            var owner = _users.First(x => x.Id == link.UserId);
            return new
            {
                link = MapLinkDetail(link),
                owner = new { owner.Id, owner.Email, owner.FullName, owner.Status },
                clicks = _clickEvents.Where(x => x.LinkId == link.Id)
                    .OrderByDescending(x => x.ClickedAtUtc)
                    .Take(10)
                    .Select(x => new
                    {
                        x.ClickedAtUtc,
                        x.Country,
                        x.City,
                        x.Device,
                        x.Browser,
                        x.OperatingSystem,
                        x.Referrer,
                        x.MaskedIp,
                        x.IsBot,
                        x.IsSuspicious
                    })
            };
        }
    }

    public object SetAdminLinkStatus(Guid adminId, Guid linkId, string status, DateTime nowUtc)
    {
        lock (_sync)
        {
            var link = _links.FirstOrDefault(x => x.Id == linkId) ?? throw new InvalidOperationException("Không tìm thấy link.");
            link.Status = status;
            link.UpdatedAtUtc = nowUtc;
            AddAudit(adminId, status == "Blocked" ? "BlockLink" : "UnblockLink", "Link", linkId.ToString(), nowUtc);
            return new { link.Id, link.Status };
        }
    }

    public ResolveResult ResolveAndTrackClick(string slug, HttpContext context, DateTime nowUtc)
    {
        lock (_sync)
        {
            var link = _links.FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            if (link is null)
            {
                return ResolveResult.NotFound();
            }

            if (link.Status is "Paused" or "Blocked" or "Deleted")
            {
                return ResolveResult.Denied("Link hiện không khả dụng.");
            }

            if (link.ExpiredAtUtc.HasValue && link.ExpiredAtUtc.Value <= nowUtc)
            {
                return ResolveResult.Denied("Link đã hết hạn.");
            }

            var totalClicks = _clickEvents.Count(x => x.LinkId == link.Id);
            if (link.ClickLimit.HasValue && totalClicks >= link.ClickLimit.Value)
            {
                return ResolveResult.Denied("Link đã vượt giới hạn click.");
            }

            if (!string.IsNullOrWhiteSpace(link.Password) && !context.Request.Query.TryGetValue("password", out var password))
            {
                return ResolveResult.PasswordRequired("Link yêu cầu mật khẩu.");
            }

            var requestPassword = context.Request.Query.TryGetValue("password", out var providedPassword)
                ? providedPassword.ToString()
                : null;

            if (!string.IsNullOrWhiteSpace(link.Password) && !string.Equals(requestPassword, link.Password, StringComparison.Ordinal))
            {
                return ResolveResult.Denied("Mật khẩu không đúng.");
            }

            _clickEvents.Add(BuildClickEvent(link.Id, context, nowUtc));
            return ResolveResult.Success(link.OriginalUrl);
        }
    }

    private static ClickEvent BuildClickEvent(Guid linkId, HttpContext context, DateTime nowUtc)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var ua = context.Request.Headers.UserAgent.ToString();
        var referrer = context.Request.Headers.Referer.ToString();
        var loweredUa = ua.ToLowerInvariant();
        var isBot = loweredUa.Contains("bot") || loweredUa.Contains("crawler") || loweredUa.Contains("spider");
        var isMobile = loweredUa.Contains("iphone") || loweredUa.Contains("android");

        return new ClickEvent
        {
            Id = Guid.NewGuid(),
            LinkId = linkId,
            ClickedAtUtc = nowUtc,
            IpAddress = ip,
            MaskedIp = MaskIp(ip),
            Country = isBot ? "Unknown" : "VN",
            City = isBot ? "Unknown" : "Ho Chi Minh City",
            Device = isMobile ? "Mobile" : "Desktop",
            Browser = loweredUa.Contains("chrome") ? "Chrome" : loweredUa.Contains("safari") ? "Safari" : "Other",
            OperatingSystem = loweredUa.Contains("windows") ? "Windows" : loweredUa.Contains("android") ? "Android" : "Other",
            Referrer = string.IsNullOrWhiteSpace(referrer) ? "direct" : referrer,
            UserAgent = ua,
            IsBot = isBot,
            IsSuspicious = isBot || ip.StartsWith("10.") || ip.StartsWith("192.168.", StringComparison.Ordinal),
            RedirectStatus = "302"
        };
    }

    private object MapLinkSummary(ShortLink link)
    {
        var clicks = _clickEvents.Where(x => x.LinkId == link.Id).ToList();
        return new
        {
            link.Id,
            link.Slug,
            link.ShortUrl,
            link.OriginalUrl,
            link.Status,
            link.Tag,
            link.CreatedAtUtc,
            link.UpdatedAtUtc,
            totalClicks = clicks.Count,
            uniqueClicks = clicks.GroupBy(x => $"{x.MaskedIp}:{x.Browser}:{x.ClickedAtUtc:yyyy-MM-dd}").Count(),
            botClicks = clicks.Count(x => x.IsBot)
        };
    }

    private object MapLinkDetail(ShortLink link)
    {
        var plan = _plans.FirstOrDefault(x => x.Id == _users.First(y => y.Id == link.UserId).CurrentPlanId);
        return new
        {
            summary = MapLinkSummary(link),
            settings = new
            {
                link.Password,
                link.ExpiredAtUtc,
                link.ClickLimit
            },
            permissions = new
            {
                canUseCustomDomain = plan?.HasCustomDomain ?? false,
                canUseAdvancedAnalytics = plan?.HasAdvancedAnalytics ?? false,
                canUseApi = plan?.HasApiAccess ?? false
            }
        };
    }

    private AuthResult CreateAuthResult(UserAccount user, DateTime nowUtc)
    {
        var token = $"demo-{Guid.NewGuid():N}";
        _sessions[token] = user.Id;
        return new AuthResult(
            token,
            new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Status,
                user.CurrentPlanId,
                user.EmailConfirmed,
                authenticatedAtUtc = nowUtc
            });
    }

    private void AddAudit(Guid adminId, string action, string targetType, string targetId, DateTime nowUtc)
    {
        _auditLogs.Insert(0, new AdminAuditLog
        {
            Id = Guid.NewGuid(),
            AdminUserId = adminId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            CreatedAtUtc = nowUtc
        });
    }

    private string GenerateSlug()
    {
        const string chars = "abcdefghjkmnpqrstuvwxyz23456789";
        var random = new Random();
        string slug;

        do
        {
            slug = new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
        while (_links.Any(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase)));

        return slug;
    }

    private void Seed()
    {
        _plans.AddRange(
        [
            new Plan { Id = 1, Code = "regular", Name = "Thường", MonthlyPrice = 0, LinkLimit = 10, AnalyticsRetentionDays = 30 },
            new Plan { Id = 2, Code = "pro", Name = "Pro", MonthlyPrice = 199000, LinkLimit = 500, AnalyticsRetentionDays = 180, HasCustomDomain = true, HasAdvancedAnalytics = true, HasApiAccess = true },
            new Plan { Id = 3, Code = "plus", Name = "Plus", MonthlyPrice = 499000, LinkLimit = 5000, AnalyticsRetentionDays = 730, HasCustomDomain = true, HasAdvancedAnalytics = true, HasApiAccess = true }
        ]);

        var now = DateTime.UtcNow;
        var admin = new UserAccount
        {
            Id = Guid.NewGuid(),
            FullName = "System Admin",
            Email = "admin@demo.local",
            Password = "Admin123!",
            Role = "Admin",
            Status = "Active",
            EmailConfirmed = true,
            CreatedAtUtc = now.AddDays(-30),
            LastLoginAtUtc = now.AddMinutes(-15),
            CurrentPlanId = 3
        };

        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            FullName = "Demo User",
            Email = "user@demo.local",
            Password = "Demo123!",
            Role = "User",
            Status = "Active",
            EmailConfirmed = true,
            CreatedAtUtc = now.AddDays(-12),
            LastLoginAtUtc = now.AddHours(-5),
            CurrentPlanId = 2
        };

        _users.AddRange([admin, user]);

        _subscriptions.AddRange(
        [
            new Subscription { Id = Guid.NewGuid(), UserId = admin.Id, PlanId = 3, Status = "Active", StartAtUtc = now.AddMonths(-1), EndAtUtc = now.AddMonths(1) },
            new Subscription { Id = Guid.NewGuid(), UserId = user.Id, PlanId = 2, Status = "Active", StartAtUtc = now.AddMonths(-1), EndAtUtc = now.AddMonths(1) }
        ]);

        var links = new[]
        {
            new ShortLink
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Slug = "summer24",
                OriginalUrl = "https://example.com/summer-campaign",
                ShortUrl = "https://sho.rt/summer24",
                Status = "Active",
                Tag = "summer",
                CreatedAtUtc = now.AddDays(-7),
                UpdatedAtUtc = now.AddDays(-1)
            },
            new ShortLink
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Slug = "appdeal",
                OriginalUrl = "https://example.com/mobile-app",
                ShortUrl = "https://sho.rt/appdeal",
                Status = "Active",
                Tag = "app",
                ClickLimit = 1000,
                CreatedAtUtc = now.AddDays(-5),
                UpdatedAtUtc = now.AddHours(-8)
            }
        };

        _links.AddRange(links);

        for (var i = 0; i < 16; i++)
        {
            _clickEvents.Add(new ClickEvent
            {
                Id = Guid.NewGuid(),
                LinkId = links[0].Id,
                ClickedAtUtc = now.AddDays(-(i % 7)).AddHours(-i),
                IpAddress = $"203.0.113.{10 + i}",
                MaskedIp = "203.0.xxx.xxx",
                Country = i % 3 == 0 ? "VN" : "US",
                City = i % 3 == 0 ? "Ho Chi Minh City" : "California",
                Device = i % 2 == 0 ? "Mobile" : "Desktop",
                Browser = i % 2 == 0 ? "Chrome" : "Safari",
                OperatingSystem = i % 2 == 0 ? "Android" : "iOS",
                Referrer = i % 2 == 0 ? "facebook.com" : "google.com",
                UserAgent = i % 2 == 0 ? "Chrome Mobile" : "Safari",
                IsBot = i % 8 == 0,
                IsSuspicious = i % 8 == 0,
                RedirectStatus = "302"
            });
        }

        for (var i = 0; i < 9; i++)
        {
            _clickEvents.Add(new ClickEvent
            {
                Id = Guid.NewGuid(),
                LinkId = links[1].Id,
                ClickedAtUtc = now.AddDays(-(i % 5)).AddHours(-(2 * i)),
                IpAddress = $"198.51.100.{20 + i}",
                MaskedIp = "198.51.xxx.xxx",
                Country = i % 2 == 0 ? "VN" : "SG",
                City = i % 2 == 0 ? "Da Nang" : "Singapore",
                Device = i % 2 == 0 ? "Desktop" : "Mobile",
                Browser = "Chrome",
                OperatingSystem = i % 2 == 0 ? "Windows" : "Android",
                Referrer = i % 2 == 0 ? "direct" : "tiktok.com",
                UserAgent = "Mozilla/5.0 Chrome",
                IsBot = false,
                IsSuspicious = false,
                RedirectStatus = "302"
            });
        }

        AddAudit(admin.Id, "SeedData", "System", "initial", now.AddDays(-1));
    }

    private static string MaskIp(string ip)
    {
        var segments = ip.Split('.');
        if (segments.Length == 4)
        {
            return $"{segments[0]}.{segments[1]}.xxx.xxx";
        }

        return "masked";
    }
}

public sealed record AuthResult(string Token, object User);

public sealed record CreateLinkInput(string OriginalUrl, string? CustomSlug, string? Tag, DateTime? ExpiredAtUtc, int? ClickLimit, string? Password);

public sealed record UpdateLinkInput(string OriginalUrl, string? Tag, DateTime? ExpiredAtUtc, int? ClickLimit, string? Password);

public sealed class ResolveResult
{
    public bool Found { get; init; }
    public bool Allowed { get; init; }
    public bool RequiresPassword { get; init; }
    public string? Reason { get; init; }
    public string? TargetUrl { get; init; }

    public static ResolveResult NotFound() => new() { Found = false };
    public static ResolveResult Success(string targetUrl) => new() { Found = true, Allowed = true, TargetUrl = targetUrl };
    public static ResolveResult Denied(string reason) => new() { Found = true, Allowed = false, Reason = reason };
    public static ResolveResult PasswordRequired(string reason) => new() { Found = true, Allowed = false, RequiresPassword = true, Reason = reason };
}
