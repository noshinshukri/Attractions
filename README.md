# TravelApi / AppWebApi

## Hur C#-klasserna för databastabellerna är uppbyggda

Applikationen är skapad av Noshin Shukri.
---
Jag har byggt upp databastabellerna i tre delar:

### 1. Interface

Till exempel `IAttraction`, `IUser` och `IReview`.

Interfacet beskriver vilka properties och relationer objektet ska ha. Det gör att resten av programmet kan jobba mot interfacet istället för direkt mot databasklassen.

### 2. Modellklass

Till exempel `Attraction`, `User` och `Review`.

Modellklassen implementerar interfacet och innehåller den vanliga C#-logiken. Här finns till exempel `Seed()` som används för att skapa testdata.

OBS! Reviews är kommentarer som uppgiften efterfrågar. Varje attraktion kan han mellan 0 - 20 reviews och varje review kan ha en eller ingen kommentar.

### 3. Db-klass

Till exempel `DbAttraction`, `DbUser` och `DbReview`.

Db-klassen ärver från modellklassen och innehåller det som EF Core behöver för att koppla modellen till databasen. Här finns bland annat `[Key]` och navigation properties till andra tabeller.

Relationerna till andra tabeller ligger i `Db`-klassen, till exempel `DbReviews`, `DbAddress` och `DbCategories`.

De vanliga properties som `Reviews`, `Address` och `Categories` är istället `[NotMapped]` och pekar vidare till `Db`-versionerna. På så sätt kan resten av programmet använda de vanliga properties utan att behöva tänka på hur EF Core hanterar databasen.

Jag använder även `[JsonIgnore]` på vissa navigation properties. Det gör att vi slipper cirkulära referenser när objekten skickas som JSON. Till exempel att en `Review` hämtar sin `User`, som sedan hämtar alla sina `Reviews` igen.

### Seeded

Varje tabell har även en `Seeded`-flagga.

Den visar om datan är skapad automatiskt som testdata eller om den har lagts in via API:et.

Det gör att vi till exempel kan ta bort seedad testdata utan att ta bort riktig data.

### DTO

Jag använder även separata DTO-klasser för Create och Update.

API:et skickar alltså inte ut själva `Db`-objekten direkt. Istället används DTO:er som bara innehåller de fält som klienten ska kunna skapa eller ändra.

För relationer skickas till exempel ett `Guid` istället för hela objektet.

På det här sättet blir det en tydlig uppdelning mellan **modell, databas och API**. Databasspecifika saker ligger i `Db`-klasserna medan resten av applikationen kan jobba med modeller och interfaces.

---

# Skapa och starta AppWebApi

## 1. Skapa databasen

Öppna en Terminal i mappen `_scripts` och kör kommandot för den databas du vill använda.

### macOS

```bash
./database-rebuild-all.sh sql-attractions sqlserver docker root ../AppWebApi
./database-rebuild-all.sh sql-attractions mysql docker root ../AppWebApi
./database-rebuild-all.sh sql-attractions postgresql docker root ../AppWebApi
```

### Windows

```powershell
.\database-rebuild-all.ps1 sql-attractions sqlserver docker root ..\AppWebApi
.\database-rebuild-all.ps1 sql-attractions mysql docker root ..\AppWebApi
.\database-rebuild-all.ps1 sql-attractions postgresql docker root ..\AppWebApi
```

Kontrollera att det inte blir några fel när databasen byggs, migrationerna körs och databasen uppdateras.

## 2. Anslut till databasen

Öppna **Azure Data Studio** och anslut till databasen.

Connection string finns i **User Secrets**.

Vilken connection string som används beror på vilken databas som körs.

Exempel:

```text
sql-attractions.sqlserver.docker.root
```

## 3. Starta AppWebApi

Öppna en Terminal i mappen `AppWebApi`.

Starta projektet med:

```bash
dotnet run -lp https
```

Öppna sedan Swagger:

```text
https://localhost:7066/swagger
```

Testa att endpoints fungerar genom att köra:

* `Admin/Environment`
* `Admin/Version`
* `Admin/Log`

## 4. Kontrollera databasen

Använd **Azure Data Studio** för att titta på databasen och dess struktur.

Kontrollera vilka tabeller som har skapats och hur de är kopplade till varandra.

## 5. Skapa seed-data

Använd endpointen:

```text
Admin/Seed
```

Den fyller databasen med testdata.

Kontrollera sedan i **Azure Data Studio** att datan har skapats och finns i tabellerna.
