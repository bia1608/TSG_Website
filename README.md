# TSG Website - Fullstack Project

This is a work in progress fullstack web application. The project integrates a robust **.NET Web API** backend with a modern, dynamic, and responsive **Angular** frontend.

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET (ASP.NET Core Web API)
- **Database:** PostgreSQL (utilizing Entity Framework Core for ORM)
- **Authentication:** JWT (JSON Web Tokens) with role-based authorization
- **Services:** Local SMTP mail service (configured on port `1025` for development testing)

### Frontend
- **Framework:** Angular v21+
- **Data Visualization:** Chart.js & ng2-charts
- **Styling:** Modern Custom CSS

---

## 📁 Project Structure

The project is split into two main directories:

1. **`TSG_Website/`** (.NET Backend)
   - Contains controllers, database migrations, models, services, DTOs, and configuration settings.
2. **`tsg-frontend/`** (Angular Frontend)
   - Contains components, pages, core services, guards, interceptors, and styles.

---

## 📋 Prerequisites

Before running the project locally:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or newer)
- [Node.js](https://nodejs.org/) (includes npm package manager)
- [PostgreSQL](https://www.postgresql.org/) running locally or accessible remotely
- A local SMTP testing client (highly recommended: [Mailpit](https://github.com/axllent/mailpit) or [Maildev] listening on port `1025`)

---

## 🚀 Setup and Run Guide

### 1. Database & Backend Configuration

1. Open a terminal inside the backend directory:
   ```bash
   cd TSG_Website
   ```
2. Configure your PostgreSQL connection string in `appsettings.json` (under `ConnectionStrings:DefaultConnection`):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=tsg_db;Username=postgres;Password=your_password"
   }
   ```
3. Apply the database migrations to create the necessary schema and tables:
   ```bash
   dotnet ef database update
   ```
4. Run the backend server:
   ```bash
   dotnet run
   ```
   *The API will start and will be accessible at `http://localhost:5119` (or the port specified in your console output).*

---

### 2. Frontend Configuration & Execution

1. Open a terminal inside the frontend directory:
   ```bash
   cd tsg-frontend
   ```
2. Install the required dependencies:
   ```bash
   npm install
   ```
3. Run the Angular development server:
   ```bash
   npm start
   ```
   *The frontend application will start and will be accessible in your web browser at: **`http://localhost:4200`**.*

---

## 📧 Email Service (Local Development)

The application is configured to send automated emails (e.g. registration confirmations, password credentials upon acceptance) through SMTP on `localhost:1025`.
To capture and view these emails locally:
1. Run **Mailpit** or **Maildev** on your machine.
2. Access their web dashboard (usually at `http://localhost:8025` for Mailpit) to inspect sent emails.