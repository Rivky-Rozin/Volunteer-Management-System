# Volunteer Call Management System

A .NET 8 application for managing volunteer calls, assignments, and administration. The project is organized into several layers for maintainability and scalability.

## Project Structure

- **PL (Presentation Layer):**  
  WPF-based user interface for managing volunteers, calls, assignments, and authentication.

- **BL (Business Logic Layer):**  
  Implements core business rules, validation, and logic for calls, volunteers, assignments, and admin operations.

- **DalFacade (Data Access Layer Facade):**  
  Defines interfaces and data objects (DO) for CRUD operations and data contracts.

- **DalList / DalXml (Data Access Layer Implementations):**  
  Provides concrete implementations for data storage using in-memory lists and XML files.

## Features

- Volunteer management (add, update, view history)
- Call management (add, update, assign)
- Assignment tracking
- Admin controls
- Secure login and password update
- Extensible data access (List/XML)

## Getting Started

1. **Prerequisites:**  
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Visual Studio 2022

2. **Build & Run:**  
   - Open the solution in Visual Studio.
   - Set `PL` as the startup project.
   - Build and run.

## Usage

- Manage volunteers and calls through the WPF interface.
- Assign calls to volunteers and track their history.
- Admins can manage assignments and system configuration.

## Extending

- Add new data access implementations by extending `DalApi` interfaces.
- Business logic can be customized in the `BL` layer.

