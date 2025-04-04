#  Yara Choice – Cosmetic Clinic API

## 📌 Overview
Yara Choice is a **scalable, secure, and high-performance API** built with **ASP.NET Core 9.0**. It provides seamless management of **doctors, patients, appointments, treatments, and product inventory** while following modern software architecture principles.

<div>
 <img src="https://imgur.com/PhXbSa6.jpg" alt="" />
</div>

## 🏗 Project Architecture & Structure
The project follows **Clean Architecture**, ensuring maintainability and scalability.

### **🔹 Layered Architecture**
- **API Layer** – Handles HTTP requests, controllers, and routing (**cosmeticClinic.Backend**).
- **Business Layer (BL)** – Encapsulates business logic (**cosmeticClinic.Business**).
- **Data Layer (DAL)** – Manages database operations (**MongoDB**) using the **Repository Pattern**.
- **Mapping Layer** – Uses **AutoMapper** for DTO-Entity conversion (**cosmeticClinic.Mappers**).
- **Validation Layer** – Implements **FluentValidation** for data integrity (**cosmeticClinic.Validation**).
- **Configuration Layer** – Centralized settings management (**cosmeticClinic.Settings**).

---

## **⚙ Key Features & Technologies**

### **📡 Real-Time Communication – SignalR**
- **Live chat & notifications** between Admins, Doctors, and Patients.
- Secure and scalable **WebSockets-based** communication.
<div>
 <img src="https://imgur.com/KJazyaH.jpg" alt="" />
</div>

### **🔐 Secure Authentication – JWT & Role-Based Authorization**
- **JWT-based authentication** (JwtBearerDefaults.AuthenticationScheme).
- **Role-based access control** (Admin, Doctor, Patient).
- **Custom PermissionAuthorizationHandler** for fine-grained security.

### **📧 Email Services – Account Access & Password Recovery**
- Sends **account verification emails** upon registration.
- Supports **password reset requests** with secure links.
- Uses **SMTP & Gmail App Password** for secure email delivery.
<div>
 <img src="https://imgur.com/pHVwJyf.jpg" alt="" />
</div>

### **📑 API Documentation – Swagger UI & OpenAPI**
- Auto-generated **Swagger documentation** for all endpoints.
- Interactive UI to test API methods easily.
- Simplifies **frontend and third-party integration**.

### **⚙ Configuration Management**
- Centralized settings stored in `appsettings.json`.
- Uses **strongly typed configuration classes** (`JwtSettings`, `MongoDbSettings`, `EmailService`).
- Supports **staging and production environments**.

### **🔄 AutoMapper – DTO & Entity Mapping**
- **Automatically maps** API request DTOs to database entities.
- Keeps **controllers clean** and **reduces boilerplate code**.
- Prevents **exposing database structures** in API responses.

### **✅ FluentValidation – Strong Data Validation**
- Ensures **data integrity before database storage**.
- Provides **detailed validation error messages**.
- Prevents **invalid or missing data** from breaking the system.

---

## **🚀 Future Enhancements**
- **🌐 React Admin Panel** – A modern dashboard for clinic management.
- **📅 Patient & Doctor Portal** – Appointment booking, chat, and medical history.
- **🔗 Full API Integration** – Seamless connectivity using **Swagger documentation**.

---

### 📝 Mind Map

I have created a detailed mind map that shows the high-level structure and flow of the project. You can view it using the link below:

[**Project Mind Map**](https://lucid.app/lucidspark/ad92bb2f-7236-42c5-b299-d00e826f0bae/edit?viewport_loc=-2005%2C-1350%2C2932%2C2619%2C0_0&invitationId=inv_dc69401b-04a0-4d32-bbcd-eadcd7755a31)
