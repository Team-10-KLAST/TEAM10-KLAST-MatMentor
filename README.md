# TEAM10-KLAST-MatMentor
4. semester eksamensprojekt

## Projektoversigt - backend

| Navn (projekttype) | Projekt-referencer | Ansvar | Fil-ansvar |
|---|---|---|---|
| Domain (Class Library) | Ingen | Domæneentiteter. Ren C#, ingen tekniske afhængigheder. | `{Entitet}.cs`: domæneentitet + forretningsregler der kan afgøres ud fra objektet selv (fx "username maks. 15 tegn"). |
| Application (Class Library) | Domain | Use cases, organiseret pr. feature og slice. `Common/` i roden indeholder tværgående kode, der ikke hører til én bestemt use case. NuGet: `Microsoft.Extensions.DependencyInjection.Abstractions`. `FrameworkReference`: `Microsoft.AspNetCore.App`. | `DependencyInjection.cs`: DI-registrering (`AddApplication()`).<br>`Common/Exceptions/*.cs`: fælles exception-typer.<br>`Request.cs`: input, matcher JSON fra klienten.<br>`Response.cs`: output. Handler samler selv felterne manuelt.<br>`Validator.cs`: simpel inputvalidering, ingen database-kald.<br>`I{UseCase}Repository.cs`: eget, smalt interface – kun denne use case.<br>`Handler.cs`: orkestrering – henter/gemmer via repository, kalder domænelogik, bygger Response.<br>`Endpoint.cs`: HTTP-ruten, kalder Validator og derefter Handler. |
| Infrastructure (Class Library) | Domain, Application | `AppDbContext` (EF Core) + repository-implementeringer. NuGet: `Microsoft.EntityFrameworkCore.SqlServer`. | `DependencyInjection.cs`: DI-registrering (`AddInfrastructure()`).<br>`AppDbContext.cs`: EF Core-context.<br>`{Entitet}Repository.cs`: implementering – kan dække flere use case-interfaces samtidig, se punkt 3. |
| Api (ASP.NET Core Web API) | Application, Infrastructure | Kun opstart. Ingen Controllers, ingen use-case-specifik kode. | `Program.cs`: registrerer services, mapper hver slices endpoint.<br>`Middleware/ExceptionHandlingMiddleware.cs`: fanger exceptions centralt, sætter korrekt HTTP-statuskode. |
 
MAUI-appen (frontend) er et selvstændigt projekt/repository uden nogen projekt-reference til backend'en.

## Mappestruktur 

```
Domain/
└── {Entitet}.cs
 
Application/
├── DependencyInjection.cs                → DI-registrering
├── Common/
│   └── Exceptions/
│       ├── NotFoundException.cs
│       └── ValidationException.cs
└── Features/
    └── {Feature}/
        └── {UseCase}/
            ├── {UseCase}Request.cs        → input
            ├── {UseCase}Response.cs       → output
            ├── {UseCase}Validator.cs      → validering
            ├── I{UseCase}Repository.cs    → interface
            ├── {UseCase}Handler.cs        → orkestrering
            └── {UseCase}Endpoint.cs       → HTTP-rute
 
Infrastructure/
├── DependencyInjection.cs                → DI-registrering
├── AppDbContext.cs                       → EF Core-context
└── Persistence/
    └── {Feature}/
        └── {Entitet}Repository.cs        → implementering
 
Api/
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs    → fejl → HTTP-statuskode
└── Program.cs
```

## Kom i gang lokalt med Docker
 
Backend og database kan køre lokalt i Docker, så ingen behøver at installere SQL Server. Databasen kører i en container med SQL Server 2022, og API'et opretter selv databasen og tabellerne ved opstart (`Migrate()` i `Program.cs`).
 
### Forudsætninger
- Docker Desktop, startet. SQL Server kræver mindst 2 GB RAM til Docker.
- .NET 10 SDK, hvis du vil køre API'et fra IDE'en.

### Første gang: lav din `.env`
Åbn terminalen i projektets rodmappe, og kør: 
```
cp .env.example .env
```
 
Åbn `.env`, og skriv et kodeord efter `MSSQL_SA_PASSWORD=`. Kodeordet bruges kun til din lokale database.
 
- Mindst 8 tegn fra 3 af 4 grupper: store bogstaver, små bogstaver, tal og symboler.
- Undgå `; $ ' " # !` og mellemrum.
- `.env` er i `.gitignore` og må aldrig committes.

### Mulighed A: Kør det hele i Docker
Åbn terminalen i projektets rodmappe, og kør: 
```
docker compose up -d --build
```
 
API'et kører på `http://127.0.0.1:55077`. Compose venter med at starte API'et, til databasen er klar. Du kan derefter teste med Postman eller et andet værktøj, fx:
```
POST http://127.0.0.1:55077/api/students
```
Og i body (i Postman: vælg Body -> Raw -> JSON):
```
   {
     "username": "Søren",
     "password": "hemmelig1",
     "parentEmail": "foraelder@example.com",
     "grade": 7,
     "interestIds": []
   }
```
 
### Mulighed B: Database i Docker, API fra IDE'en (til debugging)
Åbn terminalen i projektets rodmappe, og kør:
```
docker compose up -d db
```
 
Sæt connection stringen i dine user-secrets (kun første gang) med kodeordet fra din `.env`. Kør følgende i terminalen i projektets rodmappe:
 
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1,1433;Database=MatMentor;User Id=sa;Password=DIT_KODEORD;TrustServerCertificate=True" --project Api
```
 
Start derefter API'et fra IDE'en som normalt. Kører api-containeren, så stop den først med `docker compose stop api`, ellers er port 55077 optaget.
 
### Nyttige kommandoer
| Kommando | Hvad den gør |
|---|---|
| `docker compose ps` | Viser containerne, og om databasen er `healthy` |
| `docker compose logs api` | Viser API'ets log |
| `docker compose down` | Stopper alt. Data bevares |
| `docker compose down -v` | Stopper alt og sletter databasen. Næste opstart bygger den forfra |
 
### Secrets
- Ingen kodeord eller nøgler i `appsettings*.json`, `compose.yaml` eller andre filer i repoet.
- Lokalt bruges `.env` af Docker Compose, og user-secrets bruges af API'et, når det køres fra IDE'en.
- Del ikke output fra `docker compose config`, `docker inspect` eller `dotnet user-secrets list`. De viser kodeord i klartekst.

### Typiske fejl
- **`Login failed for user 'sa'`:** kodeordet i user-secrets og `.env` er forskelligt, eller kodeordet er ændret, efter databasen blev oprettet. SQL Server sætter kun kodeordet første gang. Kør `docker compose down -v`, og start igen.
- **Port 1433 eller 55077 er optaget:** en lokal SQL Server kører, eller API'et kører fra IDE'en samtidig med containeren.
- **Timeout, når du kører API'et fra IDE'en (Mulighed B):** din connection string i user-secrets bruger sandsynligvis `localhost`. Kør `user-secrets`-kommandoen fra Mulighed B igen, så der står `Server=127.0.0.1,1433`. Du kan se din nuværende værdi med `dotnet user-secrets list --project Api`.
- **Postman eller et andet værktøj hænger mod API'et:** brug `http://127.0.0.1:55077` i stedet for `localhost`.
 