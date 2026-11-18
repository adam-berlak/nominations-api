using System.ComponentModel.DataAnnotations;

namespace NominationsApi.Models;

public record CreateNominationRequest(
    [Required, MaxLength(50)] string ShipperId,
    [Required, MaxLength(100)] string PipelineSegment,
    [Range(0, double.MaxValue, MinimumIsExclusive = true)] decimal RequestedVolume);

public record UpdateNominationRequest(
    [Required, MaxLength(50)] string ShipperId,
    [Required, MaxLength(100)] string PipelineSegment,
    [Range(0, double.MaxValue, MinimumIsExclusive = true)] decimal RequestedVolume,
    [Required, MaxLength(30)] string Status);

public record ScheduleRequest(
    [Range(0, double.MaxValue, MinimumIsExclusive = true)] decimal Capacity);

public record ScheduleResult(
    string PipelineSegment,
    decimal TotalRequested,
    decimal Capacity,
    string Outcome,
    IEnumerable<Nomination> Nominations);
