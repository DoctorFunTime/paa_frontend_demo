# PrimeApp frontend demo

Standalone demo copied from `paa_frontend`. It preserves the Blazor WebAssembly client, ASP.NET static-file host, shared projects, namespaces, routes, components, portal layouts, models and typed service clients. It is not attached to a school.

## Run

Requires the .NET 8 SDK. From this directory:

```sh
dotnet run --project paa_frontend/paa_frontend/paa_frontend.csproj --no-launch-profile --urls http://localhost:5188
```

On this computer the SDK is available at `/home/keith/dotnet/dotnet` if `dotnet` is not on PATH. Open `http://localhost:5188/login`.

Choose Admin, Student, Teacher, Bursar or Super. Manual usernames are `admin`, `student`, `teacher`, `burser`, `super`; all use password `demo`. These are fictional demo identities, not secure production accounts.

## Demo behavior

- All typed API clients use `Services/Demo/DemoHttpHandler.cs`, a terminal in-memory handler with no network fallback. No gateway, database, school authentication service or payment provider is needed.
- `DemoStore.cs` seeds 24 fictional students, five teachers, admin/bursar profiles, classes, subjects, assessments, reports, attendance, accounts, payments and a timetable.
- Profile CRUD, attendance marking, report approval, simulated USD payments, and selected configuration edits update browser memory. Refresh or Reset demo restores the fixtures. Each tab has its own demo state.
- Switch role signs out of the current demo role. Demo storage keys have a separate `primeappDemo.` prefix.
- Payments never transfer money. Sample documents have no official validity.
- Service-dependent operations such as timetable optimization, bulk imports, password changes, real pass validation, mark entry and invoice batch generation return an explicit unavailable-in-demo response. They do not pretend to save successfully.
- Original placeholder/maintenance screens remain in place. Fonts and Tailwind styling retain the original public CDN dependencies, so this is not a fully offline visual build.

## Verify

```sh
dotnet build paa_frontend/paa_frontend/paa_frontend.csproj -m:1
dotnet run --project tests/DemoSmoke
```

The smoke executable checks all demo logins, rejected credentials, pagination, typed data deserialization, profile CRUD, simulated payment/account consistency, timetable data, reset, and blocked network fallback. The original frontend has existing compiler warnings. Package vulnerability checks require NuGet network access.
