# IT Asset Management System 💻

A full-stack, beginner-friendly corporate **IT Asset Management System** designed for showcasing in software engineering portfolio and resume applications.

The system empowers IT administrators to manage employees, asset categories, hardware/software inventory, asset assignments, returns, maintenance logs, and immutable audit trails, while giving employees a dedicated portal to view assigned assets, request new hardware, and report issues.

---

## 🚀 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8 Web API
- **Language**: C# (.NET 8)
- **Database & ORM**: Entity Framework Core 8 Code-First (SQL Server LocalDB / SQLite)
- **Authentication**: JWT (JSON Web Tokens) with claims & SHA256 Password Hashing
- **Authorization**: Role-Based Access Control (`Admin`, `Employee`)
- **API Documentation**: OpenAPI / Swagger UI
- **Architecture**: Controller → Service Layer → Entity Framework Core → SQL Server

### React Frontend (Main Application)
- **Library**: React 18
- **Build Tool**: Vite
- **Routing**: React Router v6
- **HTTP Client**: Axios with Bearer token interceptor
- **UI & Styling**: Bootstrap 5 + Lucide Icons + Custom CSS Tokens
- **State Management**: React Context API (`AuthContext`)

### ASP.NET Core MVC Area
- **Framework**: ASP.NET Core 8 MVC
- **Rendering**: Razor Views (`.cshtml`)
- **State & Auth**: Cookie Authentication + Model Binding & Data Annotations

---

## 📂 Project Structure

```text
ITAssetManagement/
│
├── Backend/                       # ASP.NET Core 8 Web API
│   ├── Controllers/               # REST API Endpoints (Auth, Assets, Employees, Requests, etc.)
│   ├── Data/                      # EF Core DbContext & DbInitializer Seed Runner
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Helpers/                   # JWT Generator & SHA256 Password Hasher
│   ├── Middleware/                # Global Exception Handler Middleware
│   ├── Models/                    # Entity Models (User, Employee, Asset, Assignment, etc.)
│   ├── Services/                  # Service Layer (Business Logic & EF Core Queries)
│   ├── appsettings.json           # Connection strings & JWT secret settings
│   ├── Backend.csproj
│   └── Program.cs                 # Dependency Injection & Middleware Pipeline
│
├── ReactFrontend/                 # React SPA (Vite)
│   ├── src/
│   │   ├── components/            # Sidebar, Navbar, StatusBadge, ConfirmModal
│   │   ├── context/               # AuthContext for login/token state
│   │   ├── pages/                 # Admin & Employee Dashboard & CRUD Pages
│   │   ├── services/              # Axios API client
│   │   ├── App.jsx                # Protected Routes & Router Configuration
│   │   ├── index.css              # Custom styling system
│   │   └── main.jsx
│   ├── package.json
│   └── vite.config.js
│
├── MVCFrontend/                   # ASP.NET Core MVC Application
│   ├── Controllers/               # MVC Controllers (Assets, Employees, Assignments, etc.)
│   ├── Models/                    # Razor Form ViewModels
│   ├── Views/                     # Razor Views (.cshtml)
│   ├── appsettings.json
│   ├── MVCFrontend.csproj
│   └── Program.cs
│
└── README.md                      # Documentation & Study Guide
```

---

## 🔐 Demo Credentials

Use these accounts to test the application:

| Role | Email | Password | Access Capabilities |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@company.com` | `Password123!` | System Dashboard, Employee Directory, Asset Catalog, Assignments, Request Approval, Maintenance, Audit Logs |
| **Employee** | `employee@company.com` | `Password123!` | Personal Dashboard, My Assigned Assets, Request New Asset, Return Request |

---

## 🗄️ Database Schema & Relationships

The EF Core Code-First model consists of the following core tables:

1. `Users`: Stores login accounts, password hashes, roles (`Admin`/`Employee`), and foreign key link to `Employees`.
2. `Employees`: Employee directory details (Code, Name, Department, Designation, Phone, Status).
3. `Categories`: Hardware & software classifications (Laptops, Monitors, Mobile, Printers, Software Licenses).
4. `Assets`: Hardware/software inventory (AssetTag, SerialNumber, Brand, Model, PurchaseDate, PurchaseCost, Status, Location).
5. `AssetAssignments`: Links an `Asset` to an `Employee` (`Status = Active` or `Returned`), recording assigned date and return date.
6. `AssetRequests`: Records employee requests for asset categories (`Status = Pending, Approved, Rejected, Completed`).
7. `MaintenanceRecords`: Tracks repairs, service center vendors, maintenance costs, and resolutions.
8. `AssetHistories`: Immutable audit trail recording every asset lifecycle event (`Created`, `Assigned`, `Returned`, `MaintenanceStart`, `MaintenanceCompleted`, `Retired`).

---

## 💡 How Data Flows Through the Layers

```text
  React SPA / Razor View
          │  (HTTP REST Request / Razor Form Submit)
          ▼
   API / MVC Controller
          │  (Extracts claims, validates model state)
          ▼
     Service Layer
          │  (Enforces business rules, updates asset status)
          ▼
   Entity Framework Core
          │  (Executes LINQ queries & DbContext tracking)
          ▼
   SQL Server / Database
```

### Business Logic Enforcements:
- **Asset Assignment**: When assigned, the asset's status automatically updates to `Assigned`. Unavailable or retired assets cannot be assigned.
- **Asset Return**: When returned, the asset's status automatically resets to `Available`, and the active assignment record is archived.
- **Asset Maintenance**: Placing an asset under maintenance updates status to `UnderMaintenance`. When repair is marked complete, status reverts to `Available`.
- **Audit History**: Every mutation automatically generates an immutable `AssetHistory` audit entry.

---

## ⚡ How to Run the System

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+)](https://nodejs.org/)

---

### 1️⃣ Run Backend API
```bash
cd ITAssetManagement/Backend
dotnet run
```
- **API Base URL**: `http://localhost:5000`
- **Swagger Documentation**: `http://localhost:5000/swagger`

---

### 2️⃣ Run React Frontend
```bash
cd ITAssetManagement/ReactFrontend
npm install
npm run dev
```
- **React App URL**: `http://localhost:5173`
- Click **"Admin Demo"** or **"Employee Demo"** on the login page for 1-click access!

---

### 3️⃣ Run MVC Frontend
```bash
cd ITAssetManagement/MVCFrontend
dotnet run --urls "http://localhost:5001"
```
- **MVC App URL**: `http://localhost:5001`

---

## 🛠️ EF Core Migration Commands

To add new Entity Framework Core migrations or update database schema:

```bash
# Add a new migration
dotnet ef migrations add InitialCreate --project Backend/Backend.csproj

# Update database schema
dotnet ef database update --project Backend/Backend.csproj
```

---

## 📡 Main API Endpoints Summary

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Public | Authenticates user & returns JWT token |
| `GET` | `/api/dashboard/admin` | Admin | Returns system metrics & recent logs |
| `GET` | `/api/dashboard/employee` | Authorized | Returns employee personal metrics & assigned assets |
| `GET` | `/api/employees` | Authorized | Search & list company employees |
| `POST` | `/api/employees` | Admin | Add new employee |
| `GET` | `/api/assets` | Authorized | Search & filter asset catalog |
| `POST` | `/api/assets` | Admin | Register new IT asset |
| `POST` | `/api/assets/{id}/assign` | Admin | Assign asset to an employee |
| `POST` | `/api/assets/{id}/return` | Authorized | Process asset return |
| `POST` | `/api/assets/{id}/maintenance` | Authorized | Send asset to maintenance |
| `GET` | `/api/asset-requests` | Authorized | View asset requests |
| `POST` | `/api/asset-requests` | Employee | Submit new asset request |
| `POST` | `/api/asset-requests/{id}/approve` | Admin | Approve asset request |
| `GET` | `/api/asset-history` | Authorized | View system audit history |
