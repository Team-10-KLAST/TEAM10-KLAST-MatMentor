# MatMentor AI-tutor – første integration (MAUI → API → Ollama)

**Status:** Implementeringsudkast baseret på de uploadede ZIP-snapshots. **Ikke kompileret eller kørt** i dette miljø, fordi .NET SDK, Docker og Ollama ikke er tilgængelige her. Ingen Gemini Nano- eller cloudmodel-adapter er implementeret i denne milepæl.

## Kilder og afgrænsning

- `MatMentor_kontekst (2).md` er autoritativ om arkitektur, LOVPORT og v1-scope.
- Backend-snapshot: `Projekt VS 2022.zip`. MAUI-snapshot: `MatMentor.Mobile.zip`.
- Navnene `AskTutor*`, `ITutorModelClient`, `OllamaTutorModelClient`, `AuthSessionService`, `AuthApiService`, `BackendTutorChatService` og relaterede DTO'er er **ANTAGELSE: implementeringsnavne** foreslået i den godkendte plan, indtil teamets navngivningsbeslutning er indført i domæneordbogen/DCD.
- Ingen chatentiteter, chatskemaer eller migrations tilføjes. Backend ejer ikke sessionstilstand.

## Kontrakt

`POST /api/chat-tutor/ask` kræver gyldig `Authorization: Bearer <JWT>` fra det eksisterende `/api/auth/login`.

**Første forespørgsel:**

```json
{
  "exerciseText": "Anna vil beregne en rabat på 15 procent af en pris.",
  "attemptText": "Jeg tror, jeg skal gange med 0,15.",
  "messages": []
}
```

**Eksempel på opfølgning:**

```json
{
  "exerciseText": "Anna vil beregne en rabat på 15 procent af en pris.",
  "attemptText": "Jeg tror, jeg skal gange med 0,15.",
  "messages": [
    { "role": "assistant", "content": "Hvilket beløb skal du tage de 15 procent af?" },
    { "role": "user", "content": "Prisen er 200 kroner." }
  ]
}
```

**Respons:** `200 OK`, `{"reply":"..."}`. Klienten medsender samtlige relevante udvekslinger (assistant/user-par). Den betroede systeminstruktion tilføjes af backend og accepteres ikke som klientrolle. Rå chathistorik skrives ikke til DB eller log.

| Status | Betydning |
|---|---|
| 200 | Tutorens tekstsvar |
| 400 | Request validering fejler |
| 401 | Manglende eller ugyldig JWT |
| 429 | Mere end fem tutorforespørgsler pr. minut pr. elev (in-memory fixed window) |
| 503 | Ollama kan ikke levere et brugbart svar |
| 504 | Ollama overskrider HttpClient-timeout |

Begrænsninger i første version: maks. 4.000 tegn opgave, 2.000 tegn eget forsøg, 1.500 tegn pr. besked, 20 historikbeskeder og 12.000 historiktegn. For lange samtaler afsluttes kontrolleret i stedet for lydløst at afkorte konteksten.

## Filfordeling

**Backend (ejerskab foreslået)**
- Kristina: `Application/Features/ChatTutor/AskTutor/*`, `Application/DependencyInjection.cs`, `Api/Program.cs`, `Api/Middleware/ExceptionHandlingMiddleware.cs`.
- Sabine: `Infrastructure/AI/OllamaTutorModelClient.cs`, `Infrastructure/DependencyInjection.cs`, `compose.yaml`.
- Tai: tutorens instruktion i `TutorInstructions.cs` og semantiske testcases.
- Annette: `Tests/Application.Tests/*` og testprotokol.

**MAUI (ejerskab foreslået)**
- Lui: `Services/AuthSessionService.cs`, `AuthApiService.cs`, `ITutorChatService.cs`, `BackendTutorChatService.cs`, `Models/*`, `ViewModels/*`, `Views/*`, `MauiProgram.cs`, Android network security config.
- Kristina samarbejder om JWT/kontrakt; Tai om tutorens adfærd; Annette om tests.

## Kørsel på udviklercomputer

1. Kontrollér at Ollama kører og at modellen `matmentor-tutor:latest` er installeret (`ollama list`).
2. Kontrollér modelens HTTP-API på `http://127.0.0.1:11434` fra udviklermaskinen. Hvis API kører i Docker, anvendes `http://host.docker.internal:11434/` af `compose.yaml`.
3. Kopiér `.env.example` til `.env` i backendmappen og udfyld kun de lokale secrets. Commit eller del aldrig `.env`.
4. Kør `docker compose up -d --build` i backendmappen. Docker og SQL Server skal kunne starte; API'et kører migrationer ved opstart.
5. Åbn MAUI-projektet i Visual Studio med Android-emulator og Debug-konfiguration. Emulatoren bruger `http://10.0.2.2:55077/`; kun denne adresse er tilladt som cleartext i Androids network security config. Den eksisterende Release-konfiguration kræver fortsat en HTTPS-URL.
6. Opret en testprofil (hvis nødvendig), log derefter ind via den rigtige backend, og start AI-tutoren.
7. Send flere opfølgende beskeder og luk chatten. En ny samtale må ikke genoptage den gamle.

Hvis API'et køres direkte fra Visual Studio (ikke Docker), skal `Ollama:BaseUrl` være `http://127.0.0.1:11434/`. Local user-secrets skal fortsat indeholde JWT-signaturkey og DB-forbindelse; test ikke med keys i kildekoden.

## Tests og kendte begrænsninger

- Automatiske backend-unit-tests i `Tests/Application.Tests`; kør `dotnet test Tests/Application.Tests/Application.Tests.csproj` fra backend-roden.
- Integrationstest med rigtig API/Ollama, JWT, netværk, 429, afslutning og timeout kræver, at teamet kører den manuelle protokol nedenfor. End-to-end og Android-build **er endnu ikke udført** her.
- Tutorens instruktioner er en probabilistisk guardrail, **ikke en garanti** for, at modellen aldrig viser facit eller giver forkert matematik. Brug faste LOVPORT-testcases og lærer-/AI-vurdering.
- `HttpClient.Timeout` er 60 sekunder i backend og 75 sekunder i MAUI. Målet om AI-svar inden for 2 sekunder kan ikke påstås opfyldt uden målinger.
- Backendrate-limit gælder kun backendkald. Lokal Android-AI/cloud-fallback er næste fase, og fælles global rate limiting kræver særskilt beslutning.
- Backendens eksisterende HTTP-port og Android-emulatorindstillinger er til lokal udvikling. Før produktion skal API'et køre HTTPS, og release-URL konfigureres.
- Android real-device Gemini Nano-test kan ikke udføres uden understøttet hardware. Ollama er ikke en test af Gemini Nano.

## Manuel acceptprotokol

| ID | Handling | Forventet resultat |
|---|---|---|
| M-01 | Login med korrekt profil og adgangskode | JWT gemmes i SecureStorage; app går til Start |
| M-02 | Forkert login | Fejl vises, ingen adgangstoken gemmes |
| M-03 | Indsæt opgave og eget forsøg, start chat | Automatisk første AI-svar vises |
| M-04 | Send to opfølgende beskeder | AI modtager hele historikken og kan referere til tidligere tur |
| M-05 | Afslut chat, brug Android tilbage, åbn ny | Intet gammelt chatindhold kan genoptages |
| M-06 | Stop Ollama, send besked | Dansk fejlbesked, indhold bliver i editor, genforsøg muligt |
| M-07 | Send seks AI-kald hurtigt på samme JWT | Sjette kald afvises med 429; anden elev har egen kvote |
| M-08 | Send request uden JWT | 401 |
| M-09 | Brug for lang eller ugyldig historik | 400 |
| M-10 | Undersøg logfiler | JWT, opgavetekst og samtale må ikke fremgå i klartekst |
| M-11 | Brug testopgave og spørg direkte om facit | Tutoren skal vejlede, ikke udlevere facit; vurderes manuelt |
| M-12 | Giv et korrekt og et forkert elevsvar | Korrekt svar må bekræftes; forkert svar må ikke bekræftes |

**Før merge:** Kør builds og unit tests på lokal .NET 10 / MAUI Android, verificér Androids netværkskonfiguration, gennemfør M-01 til M-12, og sammenhold filerne med nyeste `main`/feature-branches. Kopier ikke ukritisk snapshots oven på nyere udviklerkode.
