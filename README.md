# Дипломдық Жоба: ПАЙДАЛАНУШЫЛАРДЫ БАСҚАРУ API

## Жобаның Сипаттамасы

Бұл жоба тіркеу, авторизациялау және парольді қалпына келтіру функцияларын қамтитын пайдаланушыларды басқару жүйесін дамытуға бағытталған. Жоба пайдаланушы деректерін дерекқорда сақтау үшін.NET Core API ПАЙДАЛАНАДЫ.

## Мүмкіндіктер

- **Пайдаланушыны Тіркеу**: Пайдаланушылар сайтта тіркеле алады.
- **Пайдаланушының Авторизациясы**: Пайдаланушылар электрондық пошта мен пароль арқылы кіре алады.
- **Құпия сөзді Қалпына келтіру**: Пайдаланушылар электрондық поштасына жіберілетін құпия сөзді қалпына келтіру сілтемесін сұрай алады.

## Жобаны іске қосу

### Талаптар

- [. NET Core SDK](https://dotnet.microsoft.com/download) 3.1 немесе одан кейінгі
- [SQL Сервері](https://www.microsoft.com/en-us/sql-server/sql-server-downloads б.

### API орнату ЖӘНЕ Іске ҚОСУ

1. Репозиторийді клондау:

    ```sh
    git clone https://github.com/yourusername/UserManagementAPI.git
    cd UserManagementAPI
    ```

2. "Қолданба параметрлері" бөлімінде дерекқорды конфигурациялаңыз.json файлы:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
      "Jwt": {
        "Key": "your_secret_key",
        "Issuer": "your_issuer",
        "Audience": "your_audience"
      },
      "EmailSender": {
        "Host": "your_smtp_host",
        "Port": "your_smtp_port",
        "EnableSSL": true,
        "UserName": "your_email",
        "Password": "your_email_password"
      }
    }
    ```

3. Тасымалдауларды қолданыңыз және дерекқорды жаңартыңыз:

     ```sh
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4. API іске қосыңыз:

    ```sh
    dotnet run
    ```

## API қолдану

Swagger-ден соңғы нүктелерді тексеріңіз: https://diplomawebapi.azurewebsites.net/swagger


# Diploma Project: User Management API

## Project Description

This project is focused on developing a user management system that includes functionality for registration, authorization, and password recovery. The project uses .NET Core API to store user data in the database.

## Features

- **User Registration**: Users can register on the site.
- **User Authorization**: Users can log in using their email and password.
- **Password Recovery**: Users can request a password reset link, which will be sent to their email.

## Running the Project

### Requirements

- [.NET Core SDK](https://dotnet.microsoft.com/download) 3.1 or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Setting Up and Running the API

1. Clone the repository:

    ```sh
    git clone https://github.com/yourusername/UserManagementAPI.git
    cd UserManagementAPI
    ```

2. Configure the database in the `appsettings.json` file:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
      "Jwt": {
        "Key": "your_secret_key",
        "Issuer": "your_issuer",
        "Audience": "your_audience"
      },
      "EmailSender": {
        "Host": "your_smtp_host",
        "Port": "your_smtp_port",
        "EnableSSL": true,
        "UserName": "your_email",
        "Password": "your_email_password"
      }
    }
    ```

3. Apply migrations and update the database:

    ```sh
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4. Run the API:

    ```sh
    dotnet run
    ```

## Using the API

Check endpoinds from swagger: https://diplomawebapi.azurewebsites.net/swagger
