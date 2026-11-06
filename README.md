# TaskCinema

Cinema MVC application with the existing customer authentication flow and SQL Server database.

## Run

1. Install the .NET 10 SDK.
2. Set `ConnectionStrings:CinemaProject` in `appsettings.json` to your SQL Server. The current example uses the default local instance (`Data Source=.`) and integrated Windows authentication.
3. Open `TaskCinema.csproj` in Visual Studio and run with the HTTPS profile, or use a terminal in this folder:

   ```powershell
   dotnet restore
   dotnet run
   ```

In Development, the app applies the EF Core migrations at startup. The database server must be running and the account in the connection string must be able to create/update the `CinemaProject` database.

## Customer account pages

- `/Customer/Account/Register`
- `/Customer/Account/Login`
- `/Customer/Account/ForgotPassword`

The project uses its existing `CustomerAccount` and `PasswordResetToken` tables and cookie authentication. The reset flow sends email through `Email:Smtp` settings in configuration.

Set email credentials with User Secrets from this folder:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Email:Smtp:Host" "smtp.your-provider.com"
dotnet user-secrets set "Email:Smtp:Port" "587"
dotnet user-secrets set "Email:Smtp:EnableSsl" "true"
dotnet user-secrets set "Email:Smtp:UserName" "your-smtp-user"
dotnet user-secrets set "Email:Smtp:Password" "your-smtp-password"
dotnet user-secrets set "Email:Smtp:FromAddress" "your-sender@example.com"
dotnet user-secrets set "Email:Smtp:FromName" "CineVerse"
```

## Repository structure

`Repositories/` contains the generic repository, repository interface, and Unit of Work. Customer movie, admin movie, category, and customer account services use the Unit of Work to access SQL Server.
