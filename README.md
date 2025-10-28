Scribbler - Notes App

A simple notes app for creating, editing, and deleting notes.

What you need:
- .NET 8 SDK
- A code editor (like VS Code or Visual Studio)

How to run:

1. Get the code and open terminal in the project folder

2. Update the database connection in appsettings.Development.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\your-path\\scribblerdb.db"
  }
}

3. Run these commands:
dotnet restore
dotnet build  
dotnet run

4. Open your browser to: https://localhost:44349/

The app will:
- Create the database automatically
- Let you add, edit, and delete notes
- Save everything locally in a SQLite file

To check the database manually:
sqlite3 scribblerdb.db
.tables
SELECT * FROM Notes;

Features:
- View all notes
- Add new notes
- Edit existing notes  
- Delete notes
- Notes sorted by priority (high to low first)

API endpoints (if you need them):
GET /api/Notes - get all notes
POST /api/Notes - create note  
PUT /api/Notes/1 - update note
DELETE /api/Notes/1 - delete note