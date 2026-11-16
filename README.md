# Nominations API

A REST API for managing natural gas pipeline nominations. Shippers submit volume requests against pipeline segments; an operator schedules each segment by confirming nominations when capacity is sufficient or cutting them proportionally when demand exceeds the available capacity.

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/nominations` | List all nominations |
| GET | `/api/nominations/{id}` | Get a nomination by ID |
| POST | `/api/nominations` | Submit a new nomination |
| PUT | `/api/nominations/{id}` | Update a nomination |
| DELETE | `/api/nominations/{id}` | Delete a nomination |
| POST | `/api/nominations/schedule/{segment}` | Schedule all pending nominations for a segment |

## Scheduling

`POST /api/nominations/schedule/MAINLINE-NORTH`

```json
{ "capacity": 1000.0 }
```

If the total requested volume across all pending nominations for the segment is at or below the capacity, every nomination is **Confirmed**. If demand exceeds capacity, each nomination's volume is cut proportionally so the total fits within the capacity, and all are marked **Cut**.

```json
{
  "pipelineSegment": "MAINLINE-NORTH",
  "totalRequested": 1500.0,
  "capacity": 1000.0,
  "outcome": "Cut",
  "nominations": [
    { "id": 1, "shipperId": "SHIPPER-A", "requestedVolume": 400.0, "status": "Cut" },
    { "id": 2, "shipperId": "SHIPPER-B", "requestedVolume": 266.6667, "status": "Cut" }
  ]
}
```

## Tech Stack

- ASP.NET Core 11 Web API
- Entity Framework Core with SQLite
- Docker

## Run Locally

```bash
dotnet run --project NominationsApi
```

The API starts on `http://localhost:5034`. Open `/openapi/v1.json` or use the included `NominationsApi.http` file to explore endpoints.

## Run with Docker

```bash
docker build -t nominations-api .
docker run -p 8080:8080 nominations-api
```

## Run Tests

```bash
dotnet test
```

Tests use an in-memory database provider so no SQLite file or running server is needed.
