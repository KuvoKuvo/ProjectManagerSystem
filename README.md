# Project Management System (Sibers Test Task)

A full-stack project management application featuring a 5-step Project Creation Wizard, Employee directory, Task board (Extra 1), and Role-Based Access Control (Extra 2) using ASP.NET Core Identity.

## Tech Stack & Architecture

The solution strictly follows a **Three-Tier Architecture**:
- **ProjectManager.DAL (Data Access Layer):** EF Core with SQLite, Migrations, Entities, and Database Seeder.
- **ProjectManager.BLL (Business Logic Layer):** Services, DTOs, AutoMapper profiles, and validation logic.
- **ProjectManager.API (Presentation Layer - Backend):** ASP.NET Core Web API, controllers, and file upload handlers.
- **ProjectManager.Tests:** Unit tests targeting business logic services.
- **frontend (Presentation Layer - Frontend):** Modern Vue.js app using TypeScript, Tailwind CSS v4, Axios, Router, Pinia/Stores, and Composables.

---

## Default Administrator Credentials
On startup, the database is automatically seeded with a default administrator:
- **Email:** `admin@projectmanager.com`
- **Password:** `SecurePass123!`
- **Role:** `Director` (Full access to all modules)

---

## How to Run the Project

### 1. Run the Backend
1. Open your terminal and navigate to the backend API directory:
    ```bash
    cd backend/ProjectManager/ProjectManager.API

2. Run the application (migrations will be applied, and seed data will be created automatically):
    ```bash
    dotnet run

3. Running Tests
    ```bash
    dotnet test

### 2. Run the Frontend

1. In a new terminal window, navigate to the frontend directory:
    ```bash
    cd frontend

2. Install the required npm packages:
    ```bash
    npm install

3. Launch the Vite development server:
    ```bash
    npm run dev


