using Microsoft.AspNetCore.Http;

namespace WebShortlink.Backend.Application.Analytics;

public static class AnalyticsSourceHelper
{
    public static void ParseAnalyticsFromRequest(HttpRequest request, out string referrer, out string normalizedSource, out string? utmSource, out string? utmMedium, out string? utmCampaign)
    {
        referrer = request.Headers.Referer.ToString();

        utmSource = request.Query["utm_source"].ToString();
        if (string.IsNullOrWhiteSpace(utmSource)) utmSource = null;

        utmMedium = request.Query["utm_medium"].ToString();
        if (string.IsNullOrWhiteSpace(utmMedium)) utmMedium = null;

        utmCampaign = request.Query["utm_campaign"].ToString();
        if (string.IsNullOrWhiteSpace(utmCampaign)) utmCampaign = null;

        normalizedSource = "Direct";
        var lowerRef = referrer?.ToLowerInvariant() ?? "";

        var userAgent = request.Headers.UserAgent.ToString().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(utmSource))
        {
            var lowerUtm = utmSource.ToLowerInvariant();
            if (lowerUtm == "facebook" || lowerUtm == "fb") normalizedSource = "Facebook";
            else if (lowerUtm == "instagram" || lowerUtm == "ig") normalizedSource = "Instagram";
            else if (lowerUtm == "tiktok") normalizedSource = "TikTok";
            else if (lowerUtm == "google") normalizedSource = "Google";
            else if (lowerUtm == "youtube" || lowerUtm == "yt") normalizedSource = "YouTube";
            else if (lowerUtm == "zalo") normalizedSource = "Zalo";
            else 
            {
                // Capitalize first letter for display consistency
                normalizedSource = char.ToUpperInvariant(utmSource[0]) + utmSource.Substring(1).ToLowerInvariant();
            }
        }
        else if (lowerRef.Contains("facebook.com") || lowerRef.Contains("fb.com") || lowerRef.Contains("messenger.com") || userAgent.Contains("fban") || userAgent.Contains("fbav"))
        {
            normalizedSource = "Facebook";
        }
        else if (lowerRef.Contains("instagram.com") || userAgent.Contains("instagram"))
        {
            normalizedSource = "Instagram";
        }
        else if (lowerRef.Contains("tiktok.com") || userAgent.Contains("tiktok") || userAgent.Contains("bytedance"))
        {
            normalizedSource = "TikTok";
        }
        else if (lowerRef.Contains("google."))
        {
            normalizedSource = "Google";
        }
        else if (lowerRef.Contains("youtube.com") || lowerRef.Contains("youtu.be"))
        {
            normalizedSource = "YouTube";
        }
        else if (lowerRef.Contains("zalo.me") || lowerRef.Contains("zaloapp.com") || userAgent.Contains("zalo"))
        {
            normalizedSource = "Zalo";
        }
        else if (!string.IsNullOrWhiteSpace(referrer))
        {
            normalizedSource = "Other";
        }
    }
}
