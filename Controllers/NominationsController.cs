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
        var nomination = await db.Nominations.FindAsync([id], ct);
        if (nomination is null)
        {
            return NotFound();
        }

        nomination.ShipperId = request.ShipperId;
        nomination.PipelineSegment = request.PipelineSegment;
        nomination.RequestedVolume = request.RequestedVolume;
        nomination.Status = request.Status;
        // CreatedAt is intentionally left untouched.

        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}
