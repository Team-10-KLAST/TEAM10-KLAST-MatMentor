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
