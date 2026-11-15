namespace NominationsApi.Models;

public class Nomination
{
    public int Id { get; set; }

    public string ShipperId { get; set; } = string.Empty;

    public string PipelineSegment { get; set; } = string.Empty;

    public decimal RequestedVolume { get; set; }

    public string Status { get; set; } = NominationStatus.Pending;

    public DateTime CreatedAt { get; set; }
}

public static class NominationStatus
{
    public const string Pending = "Pending";
}
