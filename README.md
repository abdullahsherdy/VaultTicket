# 🎟️ VaultTicket

A secure and modern Event Booking System built with ASP.NET Core API. It implements authentication, role-based access, refresh token support, password hashing, and encryption of sensitive data.

---

## 🚀 Features

- 🔐 JWT Authentication & Refresh Token system
- 👤 Role-based access (User/Admin)
- 🔒 Password Hashing using BCrypt
- 🗝️ AES Encryption for personal data (e.g. ticket info)
- 🎫 Event browsing and booking
- 🛠️ Admin dashboard to create/edit/delete events
- 🌐 Swagger UI for API testing

---

## 🧑‍💻 Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 7+)
- **Authentication:** JWT, Refresh Token
- **Database:** Entity Framework Core (SQL Server / SQLite)
- **Security:** BCrypt (passwords), AES (optional encryption)

---

## 📂 Project Structure

```bash
/VaultTicket
│
├── VaultTicket.API/          # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Models/
│   ├── DTOs/
│   ├── Services/
│   ├── Middleware/
│   ├── Helpers/                # JWT & Encryption utils
│   └── Program.cs / Startup.cs
│
│
└── README.md
```
---

## 🛡️ Security Implemented

- JWT-based access token with refresh token support
- BCrypt password hashing with salt
- Role-based API protection using `[Authorize(Roles = "Admin")]`
- AES encryption utility for encrypting sensitive info (optional)
- Secure storage practices

---

## 🧪 API Endpoints

| Method | Endpoint                  | Description                     |
|--------|---------------------------|---------------------------------|
| POST   | `/api/auth/register`      | Register new user               |
| POST   | `/api/auth/login`         | Login and get JWT + Refresh     |
| POST   | `/api/auth/refresh`       | Refresh JWT token               |
| GET    | `/api/events`             | List all events (public)        |
| POST   | `/api/events`             | Add event (Admin only)          |
| POST   | `/api/bookings`           | Book a seat (User only)         |

---

## 📦 TODO / Enhancements

- [ ] Email confirmation for registration
- [ ] QR code for ticket confirmation
- [ ] User profile page
- [ ] Responsive frontend with Bootstrap or Tailwind
