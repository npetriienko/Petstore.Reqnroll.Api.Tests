# Petstore API Tests

BDD API tests for the Pet endpoints of the [Swagger Petstore](https://petstore.swagger.io/), written with Reqnroll, NUnit and RestSharp.

| Endpoint | Purpose |
|----------|---------|
| `POST /pet` | Add a new pet |
| `GET /pet/{petId}` | Find a pet by ID |
| `DELETE /pet/{petId}` | Delete a pet |

## Tech stack

- .NET 10
- [Reqnroll](https://reqnroll.net/) 3.3 on NUnit 4, with Microsoft dependency injection
- [RestSharp](https://restsharp.dev/) 114 with System.Text.Json
- [FluentAssertions](https://fluentassertions.com/) 6 for assertions
- [Bogus](https://github.com/bchavez/Bogus) 35 for test data
- Microsoft.Extensions.Configuration for settings

## Getting started

**Prerequisites:** the [.NET 10 SDK](https://dotnet.microsoft.com/download) and internet access to `petstore.swagger.io`.

Run all tests:

```bash
dotnet test
```

Run only the Pet features (every feature is tagged `@pet`):

```bash
dotnet test --filter "Category=pet"
```

Run a single feature:

```bash
dotnet test --filter "FullyQualifiedName~DeleteAPetFeature"
```

In Rider or Visual Studio, the scenarios appear in the test explorer as NUnit tests. For Rider, install the **Reqnroll** plugin to get Gherkin highlighting and navigation from steps to code.

## Configuration

Settings are read from [`config.json`](config.json), which is copied next to the test assembly:

```json
{
  "ClientSettings": {
    "BaseUrl": "https://petstore.swagger.io/v2",
    "TimeoutSeconds": 30
  }
}
```

| Setting | Meaning |
|---------|---------|
| `BaseUrl` | API root, including `/v2` |
| `TimeoutSeconds` | Timeout for each HTTP request |


The settings are checked at start-up. An invalid value makes every test fail straight away with a clear message, e.g. `ClientSettings:BaseUrl must be an absolute URL, but was 'not-a-url'`.

## Test scenarios

| Feature | Scenario | Checks |
|---------|----------|--------|
| [Add a pet](Features/Pet/AddPet.feature) | Add a pet with only the required fields | 200; response pet equals the request |
| | Add a pet with all fields | 200; response pet equals the request |
| [Get a pet by id](Features/Pet/GetPet.feature) | Get an existing pet | 200; response pet equals the created pet |
| | Get a pet that does not exist | 404; body `{"code": 1, "type": "error", "message": "Pet not found"}` |
| [Delete a pet](Features/Pet/DeletePet.feature) | Delete an existing pet | 200; `message` is the pet id; a following GET returns 404 |
| | Delete a pet that does not exist | 404; empty body |

"Equals" means a field-by-field comparison of the whole pet, including category, tags and photo URLs. A pet sent without tags is expected back with `tags: []`, which is what the server returns.

## Project structure

```
├── Framework/                       # plain C#, no Reqnroll references
│   ├── Config/                      # ClientSettings, ConfigurationException
│   ├── Data/                        # PetBuilder (test data)
│   └── Http/
│       ├── Clients/                 # PetstoreClient, PetController, HttpRequests, Endpoints
│       ├── Enums/                   # PetStatus
│       └── Models/                  # ApiResponse, Pets/{Pet, Category, Tag}
├── Context/                         # PetContext: per-scenario state
├── Hooks/                           # DependencyRegistration, PetHooks
├── Steps/                           # PetSteps, ResponseSteps
├── Features/Pet/                    # AddPet, GetPet, DeletePet
├── config.json                      # API settings
└── reqnroll.json                    # Reqnroll settings
```

Nothing under `Framework/` depends on Reqnroll, so the client, models and builder could be moved to their own library or used from plain NUnit tests. Everything Reqnroll-specific lives at the project root.

## How it works

**HTTP client.** `PetstoreClient` creates one RestSharp client from `ClientSettings` and exposes `PetController` with `AddPetAsync`, `GetPetByIdAsync` and `DeletePetByIdAsync`. Requests never throw on HTTP errors. Each method returns the full response, so the tests assert on the status code and body themselves. JSON uses camelCase, enums are sent as strings (`"available"`), and `null` fields are left out of requests.

**Dependency injection.** `DependencyRegistration` registers the services with Reqnroll's Microsoft DI plugin:

| Service | Lifetime |
|---------|----------|
| `ClientSettings`, `PetstoreClient`, `PetController` | One instance for the whole test run |
| `PetContext` | New instance for each scenario |
| Step and hook classes | Registered automatically by the plugin |

**Test data.** `PetBuilder` uses Bogus to fill every field with a random valid value. Override what a test cares about with the `With…` methods:

```csharp
var pet = new PetBuilder()
    .WithName("Rex")
    .WithStatus(PetStatus.Sold)
    .Build();
```

Passing `null` to `WithCategory`, `WithTags` or `WithStatus` leaves that field out of the request; this is how the "only required fields" scenario is built. Ids are random numbers between 1×10¹² and 9×10¹⁵, so tests don't collide on the shared public server.

**Steps and state.** Steps share data through `PetContext`: the scenario's `Pet`, the `LastResponse` (the most recent API call made by a step) and the ids of pets created on the server. `PetSteps` holds the pet-specific steps. `ResponseSteps` holds checks that work for any endpoint: status code, error body and empty body.

**Hooks.** After every scenario, `PetHooks`:
1. If the scenario failed, writes the last request (method, URL, body) and response (status, body) to the test output.
2. Deletes every pet the scenario created, even if it failed.

## Writing a new test

1. Add a scenario to a `.feature` file in `Features/Pet/`, reusing existing steps where possible:

   ```gherkin
   Scenario: Add a sold pet
     Given a valid pet with all fields
     When I add the pet
     Then the response status code should be 200
   ```

2. For a new step, add a method to `Steps/PetSteps.cs` (or `ResponseSteps.cs` for a generic response check). Use [cucumber expressions](https://docs.reqnroll.net/latest/automation/cucumber-expressions.html) such as `{int}` and `{string}`. Get dependencies through the constructor, and read and write shared state through `PetContext`.
3. For a pet that should exist on the server, add its id to `context.CreatedPetIds` so the hook deletes it afterwards.

Available steps:

| Type | Step |
|------|------|
| Given | `a valid pet with all fields` |
| Given | `a valid pet with only the required fields` |
| Given | `a pet id that does not exist` |
| Given | `an existing pet` |
| When | `I add the pet` |
| When | `I get the pet by its id` |
| When | `I delete the pet` |
| Then | `the response status code should be {int}` |
| Then | `the response pet should match the request` |
| Then | `the error response should have code {int}, type {string} and message {string}` |
| Then | `the response message should be the pet id` |
| Then | `the response body should be empty` |
| Then | `the pet should not be retrievable by its id` |


