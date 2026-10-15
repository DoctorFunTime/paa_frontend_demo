# PrimeApp frontend demo

A separately branded copy of the current `paa_frontend` Blazor application. It keeps the same pages, layouts, models, and typed API clients, including the latest admissions, school rules, bursar and finance interface updates.

The demo uses the **same development API, authentication, and data** as the main development frontend. It contains no seeded students, grades, payments, attendance, or demo credentials. Changes made in this interface affect the shared development environment.

## Run locally

Requires the .NET 8 SDK. From this directory:

```sh
dotnet run --project paa_frontend/paa_frontend/paa_frontend.csproj --no-launch-profile --urls http://localhost:5188
```

Open `http://localhost:5188/login` and sign in with an account from the development environment. On this computer the SDK is at `/home/keith/dotnet/dotnet` if `dotnet` is not on PATH.

`ApiSettings:BaseUrl` is configured as `https://paa-dev-sandbox.up.railway.app`, matching the main development frontend. To change it for local testing, set `ApiSettings__BaseUrl` to the development gateway HTTPS URL. The demo host forwards only `/api/v1/*` to that gateway and serves the browser app. This keeps API calls on the demo origin so the gateway's current single frontend CORS setting does not need to change.

## CI/CD

The `devBranch` workflow matches the main frontend's build, Docker Hub image push, and Railway redeploy stages. This repository uses its **own** Docker Hub image, `<DOCKERHUB_USERNAME>/paa-frontend-demo`, with `latest` and commit SHA tags, and its **own** Railway service, `paa-frontend-demo`.

Before deployment, create the `paa-frontend-demo` Railway service in the same project as the development gateway, point it at the `paa-frontend-demo:latest` image, and configure the Railway project/service token as this repository's `RAILWAY_TOKEN` secret. Add `DOCKERHUB_USERNAME` and `DOCKERHUB_TOKEN` as this repository's GitHub Actions secrets. The Railway service should use an independent hostname and port configuration while targeting the same development API. Set `ApiSettings__BaseUrl` there if the gateway URL changes.

The workflow is triggered by pushes to `devBranch` and checks pull requests targeting that branch. It will fail at the image or Railway stage until those external credentials and service are configured.

## Verify

```sh
dotnet restore paa_frontend.sln
dotnet build paa_frontend.sln --no-restore --configuration Release
dotnet test paa_frontend.sln --no-build --configuration Release
```

The original frontend's compile warnings may also appear here. The development gateway must be reachable for login and data screens. The demo UI alone does not create separate records or permissions.
