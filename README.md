# Sanitizer

## Installation

1. Install .NET 8 SDK if not already installed.
2. Execute the SQL scripts found in `Sanitizer.Library/Setup/SQLScripts`. These scripts will create the database, stored procedures, and insert the initial words.
3. Edit the MSSql ConnectionString to point to your server in the `appsettings.json` file located in the `Sanitizer.Website` project.
4. Run the `Sanitizer.Website` project.

## Performance Enhancements

- Add caching

## Additional Enhancements

- Enhance error logging and exception handling
- Integrate FluentValidation for input validation
- Improve general appearance of the website
- Consider using an ORM like Entity Framework for easier data access, though weigh the performance implications
- Modularize Blazor website API calls into a controller for better reusability, especially if the system is expected to expand
- Don't use the new QuickGrid as it isn't super extensible. Loading indicator too hacky.
- Add some unit testing
