# KeraShine ERP - Cosmetics Manufacturing ERP System

> Enterprise Resource Planning system for **KeraShine Cosmetics** - A complete solution for managing manufacturing, inventory, sales, and distribution of cosmetic products.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=flat-square)

## 🏭 Overview

KeraShine ERP is a custom-built ERP solution designed specifically for a cosmetic manufacturing company. It streamlines the entire business flow from raw material procurement to finished product sales.

**Company:** KeraShine - Hair Cosmetics Manufacturer (Karachi, Pakistan)
**Role:** Solo Full-Stack Developer / Founder

## ✨ Core Modules

### 1. Product & Manufacturing Management
- Product catalog with variants (Hair oils, Shampoos, Serums)
- Bill of Materials (BOM) for each product
- Batch manufacturing & production tracking
- Expiry & quality control management

### 2. Inventory & Warehouse
- Raw material inventory tracking
- Finished goods stock management
- Low-stock alerts & reorder points
- Warehouse location management

### 3. Sales & Order Management
- Order processing & invoicing
- Customer management (B2B & D2C)
- Distributor & retailer management
- Sales reporting & analytics

### 4. Finance & Accounting
- Purchase management
- Expense tracking
- Profit/Loss reports per product

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 9.0 MVC
- **Language:** C#
- **Database:** SQL Server + Entity Framework Core
- **ORM:** EF Core (Code First with Migrations)
- **Frontend:** Razor Views, Bootstrap, jQuery
- **Authentication:** ASP.NET Core Identity
- **Architecture:** MVC, Repository Pattern

## 🏗️ Architecture

Controllers/ -> Request handling & business logic routing
Models/ -> Domain entities & ViewModels
Data/ -> DbContext & EF Core configurations
Views/ -> Razor UI
Migrations/ -> Database schema evolution
wwwroot/ -> Static assets


## 🚀 Getting Started

### Prerequisites
-.NET 9.0 SDK
- SQL Server 2022 / LocalDB
- Visual Studio 2022

### Installation
```bash
# Clone the repo
git clone https://github.com/kerashinecosmetics-creator/kerashine-erp.git

# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run the project
dotnet run

📊 Key Features Implemented
Product CRUD with variants[x]
Inventory management[x]
Manufacturing batch tracking[x]
 Sales & Order module (In Progress)
 Barcode integration
 Advanced reporting dashboard
🔒 Security
Role-based access control (Admin, Production Manager, Sales)
Identity-based authentication
📈 Why this ERP?
Unlike generic ERPs (Odoo, SAP), this is built specifically for cosmetic manufacturing workflows - handling batch numbers, expiry dates, and cosmetic compliance.

Developer: Mustafa Hasan Ali -.NET Backend Developer
Contact: www.linkedin.com/in/mustafahasanali
