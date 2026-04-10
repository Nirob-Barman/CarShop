# 🚗 CarShop — Car Sales Platform

CarShop is a full-featured, production-ready car sales platform built with ASP.NET Core 8 MVC following **Clean Architecture**. It supports multi-gateway payments, role-based admin management, real-time notifications, promo codes, test drive bookings, and more.

---

## 📋 Features

### For Users
- **Browse & Search** — keyword, brand, price range, and sort filters on `/Car/AllCars`
- **Car Details** — image, specs, stock status, social share (WhatsApp, Facebook, Copy Link)
- **Star Ratings & Reviews** — 1–5 star rating with written review; one per user per car
- **Wishlist** — save favourite cars; most-wishlisted shown on homepage
- **Recently Viewed** — cookie-based, last 8 cars, 30-day expiry
- **Multi-Gateway Checkout** — Stripe, SSLCommerz, BKash, SurjoPay; select gateway at checkout
- **Order Management** — paginated order history, order details with printable receipt
- **Test Drive Bookings** — pick date/time and notes; track booking status
- **Stock Alerts** — get notified when an out-of-stock car is restocked
- **Promo Codes** — apply discount codes at checkout; public deals page at `/PromoCode/Deals`
- **In-App Notifications** — notification bell in navbar with unread badge (60s polling); mark as read / mark all
- **Email Notifications** — order placed, order cancelled, payment confirmed, stock alert
- **User Dashboard** — sidebar layout with avatar initials; My Orders, Wishlist, Test Drives, Stock Alerts, Notifications, Reviews, Profile, Change Password

### For Admins
- **Analytics Dashboard** — KPI cards, revenue, top cars, low-stock warnings, monthly orders chart
- **Car Management** — full CRUD with image upload, live preview, type/size validation
- **Brand Management** — create, edit, delete brands
- **Order Management** — view all orders, filter by status, cancel any order
- **User Management** — list users, assign roles, ban / unban accounts
- **Role Management** — create, rename, delete roles (Admin and User are protected)
- **Promo Code Management** — create, edit, toggle active, delete codes with usage/expiry limits
- **Test Drive Management** — view all bookings, update status (Pending → Confirmed → Completed / Cancelled)
- **Payment Gateway Management** — add/edit/toggle/delete gateways; credentials stored encrypted
- **Audit Logs** — full mutation history (old + new values JSON) for Car, Brand, Order, PromoCode, PaymentGateway, TestDrive
- **Bulk Import** — CSV import for car listings

### Technical
- **Clean Architecture** — Domain → Application → Infrastructure → Web; no layer skip
- **Unit of Work + Generic Repository** — `_unitOfWork.Repository<T>()` throughout; no direct DbContext injection
- **Result<T> wrapper** — consistent `Ok`/`Fail`/`FailField` returns from all services
- **Strategy pattern** — payment processors resolved by gateway slug via `PaymentProcessorFactory`
- **Config encryption** — gateway API keys encrypted at rest via ASP.NET Data Protection
- **Redis cache** — Upstash cloud (StackExchange.Redis)
- **Rate limiting** — 10 req/min/IP on auth POST endpoints (Login, Register, ForgotPassword)
- **Health check** — `/health` endpoint
- **SEO** — Open Graph + Twitter Card meta tags on Car Details
- **Homepage lazy loading** — Tier 2 sections loaded via IntersectionObserver with skeleton placeholders
- **Custom error pages** — 404 (`NotFound.cshtml`) and 500 (`Error.cshtml`)
- **External static files** — all page-scoped CSS/JS in `wwwroot/css/pages/` and `wwwroot/js/pages/`; no internal `<style>` or `<script>` blocks in views

---

## ⚙️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (SQLEXPRESS) |
| Auth | ASP.NET Identity |
| Cache | Redis (Upstash) via StackExchange.Redis |
| Email | Gmail SMTP (`smtp.gmail.com:587`) |
| Payments | Stripe SDK + SSLCommerz / BKash / SurjoPay via HttpClient |
| Frontend | Bootstrap 5, jQuery, SweetAlert2, Chart.js |
| Config encryption | ASP.NET Data Protection |

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (SQLEXPRESS)
- `dotnet-ef` tool — `dotnet tool install --global dotnet-ef`

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Nirob-Barman/CarShop.git
   cd CarShop
   ```

2. Configure **appsettings.json** (see Configuration section below).

3. Apply database migrations:

   ```bash
   dotnet ef database update --project CarShop.Infrastructure --startup-project CarShop.Web
   ```

4. Run the web app:

   ```bash
   dotnet run --project CarShop.Web
   ```

5. Open in browser: `https://localhost:5001`

---

## ⚙️ Configuration

### Database

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CarShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Email (Gmail SMTP)

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderName": "Car Shop",
  "SenderEmail": "noreply@carshop.com",
  "Username": "your-smtp-username",
  "Password": "your-smtp-password",
  "EnableSsl": true
}
```

> Requires a Gmail [App Password](https://myaccount.google.com/apppasswords), not your account password.

### Redis

```json
"Redis": {
  "ConnectionString": "<host>:<port>,password=<password>,ssl=True,abortConnect=False"
}
```

### Stripe

```json
"Stripe": {
  "PublishableKey": "pk_test_...",
  "SecretKey": "sk_test_..."
}
```

> SSLCommerz, BKash, and SurjoPay credentials are stored **encrypted in the database** — managed via Admin → Payment Gateways UI, not appsettings.

> ⚠️ Never commit real credentials. Use environment variables or secrets management in production.

---

## 📂 Solution Structure

```
CarShop/
├── CarShop.Domain/          # Entities only — no dependencies
├── CarShop.Application/     # Business logic, DTOs, interfaces, services
├── CarShop.Infrastructure/  # EF Core, Identity, email, Redis, payment processors
└── CarShop.Web/             # MVC controllers, Razor views, ViewModels, static files
    ├── wwwroot/css/pages/   # Page-scoped CSS (admin-layout, car-layout, checkout, order-print)
    └── wwwroot/js/pages/    # Page-scoped JS (29 files — one per feature/page)
```

---

## 👤 Roles

| Role | Access |
|---|---|
| **Admin** | Full CRUD on cars/brands; manage orders, test drives, promo codes, payment gateways, roles, users; analytics dashboard, audit logs, bulk import |
| **User** | Browse, buy, wishlist, review, test drive bookings, stock alerts, notifications |

---

## 🤝 Contributing

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Push and open a pull request.

---

## 📞 Support

Open an issue or email [nirob.barman.19@gmail.com](mailto:nirob.barman.19@gmail.com).

---

## 📜 License

MIT License — see the LICENSE file for details.
