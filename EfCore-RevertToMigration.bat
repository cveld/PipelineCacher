echo Provide the previous migration as a parameter
dotnet ef database update %1 --project .\PipelineCacher.Entities --startup-project .\PipelineCacher.MigrationsConsoleApp
