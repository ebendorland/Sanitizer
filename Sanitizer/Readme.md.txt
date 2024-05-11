Install .NET 8 SDK if not already installed.
SQL scripts to run are found in Sanitizer.Library/Setup/SQLScripts. This will create the DB, the stored procs and insert the initial words.
After this, edit the MSsql ConnectionString to point to your server in the appsettings.json in the Sanitizer.Website project.
Run the Sanitizer.Website project.

What would I do to enhance the performance?
- I'd add some form of caching.

What additional enhancements would I add?
- Caching
- Error logging and better exception handling
- FluentValidation
- Make my data grid prettier
- Interacting with sql through something like EF would make things easier to work with, but not sure if the performance hit is worth it.
- Split out blazor website API calls into a controller for reusability should the system expand