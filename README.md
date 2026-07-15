# Project Management System (Sibers Test Task)

A full-stack project management application featuring a 5-step Project Creation Wizard, Employee directory, Task board (Extra 1), and Role-Based Access Control (Extra 2) using ASP.NET Core Identity.

---

## Short video demonstrations

<details>
<summary><b>Demonstration 1: 5-Step Project Creation Wizard </b></summary>
<br>

This video demonstrates the step-by-step process of creating a new project.

https://github.com/user-attachments/assets/03d4f926-2e91-4ad9-9973-91a4d36f2459

</details>

<details>
<summary><b>Demonstration 2: Roles, Access Control & Task Board </b></summary>
<br>

This video demonstrates the operation of a role-based access control system.

https://github.com/user-attachments/assets/7ccb2f9b-59da-4220-a3f6-1c660c95f4b2

</details>

<details>
<summary><b>Demonstration 3: Employee Management & Secure Directory</b></summary>
<br>

This video demonstrates the process of adding a new employee and using the automatic secure password generator.

https://github.com/user-attachments/assets/003e2c98-b81f-4deb-9fc1-921fd269e5b8

</details>

---

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


