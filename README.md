﻿# 🚗 CarShop - Car Sales Website

CarShop is a modern, full-featured car sales website built with ASP.NET Core using the Clean Architecture pattern. It allows users to browse cars, filter by brand, place orders, manage their orders, and leave comments on car details.

---

## 📝 Project Overview

CarShop is a car sales platform designed to provide a seamless experience for both car buyers and sellers. It includes features like car listing, brand filtering, user authentication, car ordering, and comment management. The project follows the Clean Architecture principles to ensure a modular, scalable, and maintainable codebase.

### 📋 Key Features

* **Car Listings:** View all available cars with details like image, price, and brand.
* **Brand Filtering:** Filter cars based on brand for more precise browsing.
* **Car Details Page:** View detailed car information, including comments and stock availability.
* **User Authentication:** Register, login, and manage user accounts securely.
* **Email Notifications:** Sends confirmation emails for actions like user registration, orders, etc.
* **Place Orders:** Authenticated users can place orders for cars.
* **Order Management:** View, cancel, and manage previous orders.
* **Comments:** Add comments to car details pages.

---

## ⚙️ Technologies Used

* **Backend:** ASP.NET Core 8, EF Core, Clean Architecture
* **Frontend:** Bootstrap 5, jQuery, Razor Pages
* **Database:** Microsoft SQL Server
* **Authentication:** ASP.NET Identity
* **Alerts:** SweetAlert2

---

## 🚀 Getting Started

### Prerequisites

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 (or later)

### Installation

1. Clone the repository:

   ```bash
   https://github.com/Nirob-Barman/CarShop.git
   cd CarShop
   ```
2. Configure the database connection in **appsettings.json**.
3. Run database migrations:

   ```bash
   dotnet ef database update
   ```
4. Build and run the project:

   ```bash
   dotnet run
   ```
5. Open the project in your browser:

   ```
   https://localhost:5001
   ```

### Database Setup

Ensure the **DefaultConnection** string in **appsettings.json** is correctly configured for your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CarShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## ✉️ Email Configuration (SMTP)
To enable email features such as registration confirmations, password resets, or orher notifications, you must configure SMTP settings in your appsettings.json:
```
"EmailSettings": {
  "SmtpServer": "smtp.example.com",
  "Port": 587,
  "SenderName": "Car Shop",
  "SenderEmail": "noreply@carshop.com",
  "Username": "your-smtp-username",
  "Password": "your-smtp-password",
  "EnableSsl": true
}
```

---

## 📂 Project Structure

```
CarShop
│
├── CarShop.Domain             # Domain Models
├── CarShop.Application        # Business Logic & DTOs
├── CarShop.Infrastructure     # Data Access & Repositories
├── CarShop.Web                # ASP.NET Core MVC Project (Frontend)
└── README.md                  # Project Documentation
```

---

## 🔧 Key Components

### Models

* **Car:** Represents a car with title, description, price, brand, and quantity.
* **Brand:** Represents a car brand.
* **Order:** Represents a car purchase with quantity and user info.
* **Comment:** Represents user comments on car details pages.

### Controllers

* **AccountController:** Handles user authentication and profile management.
* **HomeController:** Manages car listings, filtering, and car details.
* **OrderController:** Handles placing and canceling orders.
* **CommentController:** Manages car comments.

---

## 🤝 Contribution Guidelines

* Fork the repository.
* Create a feature branch.
* Commit your changes.
* Push to the branch.
* Open a pull request.

---

## 📅 Future Improvements

* Add support for customer reviews and ratings.
* Implement car comparison functionality.
* Add a wish list for saving favorite cars.
* Integrate payment gateways for seamless transactions.
* Add real-time notifications for order updates.
* Implement advanced analytics and reports for admins.

---

## 📞 Support

For support, please open an issue or reach out via [nirob.barman.19@gmail.com](mailto:nirob.barman.19@gmail.com).

---

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.
