using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominationsApi.Data;
using NominationsApi.Models;

namespace NominationsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NominationsController(NominationsDbContext db) : ControllerBase
{
    // GET api/nominations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Nomination>>> GetAll(CancellationToken ct)
    {
        var nominations = await db.Nominations
            .AsNoTracking()
            .OrderBy(n => n.Id)
            .ToListAsync(ct);

        return Ok(nominations);
    }

    // GET api/nominations/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Nomination>> GetById(int id, CancellationToken ct)
    {
        var nomination = await db.Nominations
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id, ct);

        return nomination is null ? NotFound() : Ok(nomination);
    }

    // POST api/nominations
    [HttpPost]
    public async Task<ActionResult<Nomination>> Create(CreateNominationRequest request, CancellationToken ct)
    {
        var nomination = new Nomination
        {
            ShipperId = request.ShipperId,
            PipelineSegment = request.PipelineSegment,
            RequestedVolume = request.RequestedVolume,
            Status = string.IsNullOrWhiteSpace(request.Status) ? NominationStatus.Pending : request.Status,
            CreatedAt = DateTime.UtcNow
        };

        db.Nominations.Add(nomination);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = nomination.Id }, nomination);
    }

    // PUT api/nominations/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateNominationRequest request, CancellationToken ct)
    {
        if (!NominationStatus.ValidValues.Contains(request.Status))
            return BadRequest($"Invalid status '{request.Status}'. Valid values: {string.Join(", ", NominationStatus.ValidValues)}.");

        var nomination = await db.Nominations.FindAsync([id], ct);
        if (nomination is null)
            return NotFound();

        nomination.ShipperId = request.ShipperId;
        nomination.PipelineSegment = request.PipelineSegment;
        nomination.RequestedVolume = request.RequestedVolume;
        nomination.Status = request.Status;

        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // DELETE api/nominations/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var nomination = await db.Nominations.FindAsync([id], ct);
        if (nomination is null)
            return NotFound();

        db.Nominations.Remove(nomination);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // POST api/nominations/schedule/MAINLINE-NORTH
    // Body: { "capacity": 1000.0 }
    // Confirms all pending nominations for the segment if total <= capacity.
    // Cuts each proportionally if total > capacity.
    [HttpPost("schedule/{segment}")]
    public async Task<ActionResult<ScheduleResult>> Schedule(
        string segment,
        ScheduleRequest request,
        CancellationToken ct)
    {
        var nominations = await db.Nominations
            .Where(n => n.PipelineSegment == segment && n.Status == NominationStatus.Pending)
            .ToListAsync(ct);

        if (nominations.Count == 0)
            return NotFound($"No pending nominations found for segment '{segment}'.");

        var totalRequested = nominations.Sum(n => n.RequestedVolume);
        string outcome;

        if (totalRequested <= request.Capacity)
        {
            foreach (var n in nominations)
                n.Status = NominationStatus.Confirmed;
            outcome = NominationStatus.Confirmed;
        }
        else
        {
            var ratio = request.Capacity / totalRequested;
            foreach (var n in nominations)
            {
                n.RequestedVolume = Math.Round(n.RequestedVolume * ratio, 4);
                n.Status = NominationStatus.Cut;
            }
            outcome = NominationStatus.Cut;
        }

        await db.SaveChangesAsync(ct);

        return Ok(new ScheduleResult(segment, totalRequested, request.Capacity, outcome, nominations));
    }
}
