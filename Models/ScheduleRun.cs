namespace NominationsApi.Models;

public class ScheduleRun
{
    public int Id { get; set; }
    public string PipelineSegment { get; set; } = string.Empty;
    public DateTime RanAt { get; set; }
    public decimal Capacity { get; set; }
    public decimal TotalRequested { get; set; }
    public string Outcome { get; set; } = string.Empty;
}