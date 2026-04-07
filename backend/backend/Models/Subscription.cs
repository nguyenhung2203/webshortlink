public sealed class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PlanId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
}
