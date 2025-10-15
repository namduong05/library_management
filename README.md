# Library Management System

This solution provides a SQL Server database design and an ASP.NET Core MVC web application for managing a library. It supports book cataloguing, user management for librarians and readers, as well as borrowing and returning workflows.

## Projects

- **LibraryManagement.Web** – ASP.NET Core MVC application using Entity Framework Core with SQL Server.
- **database/LibraryManagement.sql** – SQL script to create the database schema and seed initial data.

## Getting Started

1. **Database**
   - Execute `database/LibraryManagement.sql` in SQL Server Management Studio or sqlcmd to create the `LibraryManagementDb` database, tables, relationships, and seed data for books, users, and an example loan.

2. **Web Application**
   - Restore and build dependencies:
     ```bash
     dotnet restore LibraryManagement.Web/LibraryManagement.Web.csproj
     dotnet build LibraryManagement.Web/LibraryManagement.Web.csproj
     ```
   - Update the connection string in `LibraryManagement.Web/appsettings.json` if you are not using the default `(localdb)\mssqllocaldb` instance.
   - Run the site:
     ```bash
     dotnet run --project LibraryManagement.Web/LibraryManagement.Web.csproj
     ```
   - The application will apply the schema (via `EnsureCreated`) and seed demo data on first launch.

## Features

- 📚 **Book management** – full CRUD operations, availability tracking, and loan history per book.
- 👥 **User management** – manage librarians and readers with role filtering and loan history per user.
- 🔄 **Loan management** – create borrow transactions, filter by status, and mark returns to keep inventory in sync.
- 📊 **Dashboard** – at-a-glance metrics for books, users, and active loans with quick links to key actions.
- 🎨 **Modern UI** – responsive Bootstrap 5 and Font Awesome styling for a clean, user-friendly experience.

## Tech Stack

- ASP.NET Core 7 MVC
- Entity Framework Core 7 (SQL Server provider)
- Bootstrap 5 / Font Awesome 6 for UI

## Screenshots

Run the project and explore the dashboard, books, users, and loans pages to see the interface in action.

