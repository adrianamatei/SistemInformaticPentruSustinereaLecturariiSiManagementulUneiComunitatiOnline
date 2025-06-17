# Sistem informatic pentru promovarea lecturii și managementul unei comunități online
Aplicația implementată oferă un mediu interactiv și sugestiv care pune la dispoziția utilizatorilor o platformă care să corespundă stadiului actual al tehnologiei, oferind o variantă digitalizată a cititului clasic, promovând lectura activă, socializarea între copii și evaluarea prin joc.
## Adresa repository-ului în care se află proiectul este: https://github.com/adrianamatei/AplicatieLicenta
## Codul sursă(fără fișierele binare):
- `Pages/` – pagini Razor pentru utilizatori (StartUser, VizualizareCarti, ChatBot etc.) și admin (CreareTest, VizualizareUtilizatori etc.)
- `Data/` – clasa `AppDbContext.cs`, entități și tabele many-to-many
- `Hubs/` – `ChatHub.cs` pentru funcționalitatea SignalR
- `Services/` – integrare cu AI (Gemini, Google Books), generare diplomă
- `wwwroot/` – fișiere statice (CSS, imagini, mesaje vocale)
- `wwwroot/teste/` – quiz-uri salvate în format XML
- Fișierul `README.md` – prezentul document
## Pentru a compila aplicația trebuie urmați următorii pași:
1. Deschide fișierul `AplicatieLicenta.sln` în **Visual Studio 2022** (sau o versiune mai nouă).
2. Asigură-te că este selectată configurația `Debug` (sau `Release`, după caz).
3. Din meniu, accesează `Build > Build Solution` sau apasă `Ctrl + Shift + B`.
4. Așteaptă ca aplicația să fie compilată complet, fără erori.
5. Dacă apar probleme de pachete, rulează comanda:
   ```bash
   dotnet restore
 ## Cerințe tehnice

- Visual Studio 2022 sau mai nou
- .NET 6 SDK sau compatibil
- Entity Framework Core Tools
- SQL Server LocalDB sau alt server compatibil
- Browser modern (Chrome, Edge, Firefox)

## Pașii pentru instalarea și lansarea aplicației sunt următorii:
1. Prima dată se clonează repository-ul prin comanda: git clone https://github.com/adrianamatei/AplicatieLicenta.git..
2. Pasul următor este deschiderea în Visual Studio a soluției AplicațieLicență.sln.
3. În cadrul aplicației, se verifică conexiunea cu baza de date și apoi se aplică migrările prin comanda: dotnet ef database update.
4. Pentru rulare se apasă F5/Ctrl+F5 sau se rulează comanda dotent run sau se apasă pe butonul de rulare din interfață și astfel aplicația va rula în browser.
## Utilizatorii țintă
Această aplicație este destinată către două categorii de utilizatori:
- Admin pentru realizarea managementului aplicației 
- Cititorilor care utilizează funcționalitățile aplicației pentru un mediu mai plăcut de lecturare
## Funcționalitățile principale ale aplicației sunt:
-  Vizualizare și filtrare cărți PDF și audio
-  Căutare avansată (inclusiv vocală)
-  Quiz-uri tematice salvate în XML și bază de date
-  Cluburi de lectură pe vârstă
-  Chat în timp real (text și vocal)
-  Avatar și clasament pe scoruri
-  Diplomă de final pentru utilizatorii aflați pe podium
-  Listă de cărți favorite
-  Panou admin complet




