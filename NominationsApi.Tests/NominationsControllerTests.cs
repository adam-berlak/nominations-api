using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NominationsApi.Models;
using Xunit;

namespace NominationsApi.Tests;

public class NominationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NominationsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        }).CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsCreatedNomination()
    {
        await CreateNomination("SHIPPER-GA", "SEG-GA", 100m);

        var response = await _client.GetAsync("/api/nominations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var nominations = await response.Content.ReadFromJsonAsync<List<Nomination>>();
        Assert.NotNull(nominations);
        Assert.Contains(nominations, n => n.ShipperId == "SHIPPER-GA");
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithPendingStatus()
    {
        var request = new CreateNominationRequest("SHIPPER-CR", "SEG-CR", 500m, null);

        var response = await _client.PostAsJsonAsync("/api/nominations", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Nomination>();
        Assert.NotNull(created);
        Assert.Equal("SHIPPER-CR", created.ShipperId);
        Assert.Equal(NominationStatus.Pending, created.Status);
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/nominations/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingNomination_Returns204AndRemovesIt()
    {
        var created = await CreateNomination("SHIPPER-DEL", "SEG-DEL", 100m);

        var deleteResponse = await _client.DeleteAsync($"/api/nominations/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/nominations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_InvalidStatus_Returns400()
    {
        var created = await CreateNomination("SHIPPER-INV", "SEG-INV", 200m);

        var request = new UpdateNominationRequest("SHIPPER-INV", "SEG-INV", 200m, "InvalidStatus");
        var response = await _client.PutAsJsonAsync($"/api/nominations/{created.Id}", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Schedule_UnderCapacity_ConfirmsAllNominations()
    {
        var segment = "PEMBINA-NORTH-CONFIRM";
        await CreateNomination("S1", segment, 300m);
        await CreateNomination("S2", segment, 200m);

        var response = await _client.PostAsJsonAsync(
            $"/api/nominations/schedule/{segment}",
            new ScheduleRequest(1000m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ScheduleResult>();
        Assert.NotNull(result);
        Assert.Equal(NominationStatus.Confirmed, result.Outcome);
        Assert.Equal(500m, result.TotalRequested);
        Assert.All(result.Nominations, n => Assert.Equal(NominationStatus.Confirmed, n.Status));
    }

    [Fact]
    public async Task Schedule_OverCapacity_CutsProportionally()
    {
        var segment = "FOOTHILLS-CUT";
        await CreateNomination("SX", segment, 600m);
        await CreateNomination("SY", segment, 400m);

        var response = await _client.PostAsJsonAsync(
            $"/api/nominations/schedule/{segment}",
            new ScheduleRequest(500m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ScheduleResult>();
        Assert.NotNull(result);
        Assert.Equal(NominationStatus.Cut, result.Outcome);
        Assert.Equal(1000m, result.TotalRequested);
        Assert.All(result.Nominations, n => Assert.Equal(NominationStatus.Cut, n.Status));
        var totalAllocated = result.Nominations.Sum(n => n.RequestedVolume);
        Assert.True(totalAllocated <= 500m,
            $"Total allocated {totalAllocated} should not exceed capacity 500");
    }

    private async Task<Nomination> CreateNomination(string shipperId, string segment, decimal volume)
    {
        var request = new CreateNominationRequest(shipperId, segment, volume, null);
        var response = await _client.PostAsJsonAsync("/api/nominations", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Nomination>())!;
    }
}
