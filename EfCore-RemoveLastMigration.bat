echo This only removes the migration from your disk! Not from the database
dotnet ef migrations remove --project .\PipelineCacher.Entities --startup-project .\PipelineCacher.MigrationsConsoleApp
