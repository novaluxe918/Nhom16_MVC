# 🏟️ Nhom16 - Hệ Thống Quản Lý Sân Bóng Tại Đà Nẵng
---
# Thông tin Nhóm 16
    1.Phan Công Phước - 22115141122115
    2.Phan Mai Hoài Nhi - 22115141122112
    3.Nguyễn Thị Thương - 23115053122243

## 🚀 QUICK START (Chạy Nhanh)

### Yêu Cầu Hệ Thống
- **Node.js 18+** (Frontend)
- **.NET 8 SDK** (Backend)

### Chạy Backend (Terminal 1)
```bash
cd Nhom16_MVC/Nhom16_MVC
dotnet run
```
✅ Backend sẽ chạy tại: **https://localhost:7295**

### Chạy Frontend (Terminal 2)
```bash
cd Nhom16_MVC/Nhom16_MVC/fe
npm install
npm run dev
```
✅ Frontend sẽ chạy tại: **http://localhost:5173**

---

## 🔐 ĐĂNG NHẬP TEST

### Tài Khoản Sẵn Có (Mạnh từ DB)

**Người Thuê Sân**:
```
Email: an@gmail.com
Pass:  hash1
Số dư: 1,000,000 đ
```

**Chủ Sân**:
```
Email: chu1@gmail.com
Pass:  hash6
Số dư: 5,000,000 đ
```

**Quản Trị Viên**:
```
Email: admin@app.com
Pass:  hash0
```

---

## 🛠️ CÔNG NGHỆ SỬ DỤNG

### Backend
- **ASP.NET Core 8** - Web API Framework
- **Entity Framework Core** - ORM
- **PostgreSQL** (Neon Cloud) - Database
- **JWT** - Authentication
- **VNPay** - Payment Gateway

### Frontend  
- **ReactJS** - UI Library
- **Vite** - Build Tool (Lightning fast)
- **Tailwind CSS** - Styling
- **Axios** - HTTP Client

### DevOps
- **Git** - Version Control
- **GitHub** - Repository

---

## 📊 CÁC CHỨC NĂNG CHÍNH

### 1️⃣ **Xác Thực & Tài Khoản**
- Đăng ký tài khoản mới (Người thuê / Chủ sân)
- Xác thực email qua OTP
- Login & Logout an toàn

### 2️⃣ **Tìm Kiếm & Xem Sân**
- Tìm kiếm sân bóng trống theo ngày, giờ, loại sân
- Xem chi tiết sân (ảnh, giá, địa chỉ, đánh giá)
- Xem gợi ý sân top rated

### 3️⃣ **Đặt Sân & Quản Lý Lịch**
- Đặt sân bóng online
- Hủy đặt & hoàn tiền tự động
- Xem lịch sử đặt sân

### 4️⃣ **Đánh Giá & Bình Luận**
- Đánh giá sân (1-5 sao)
- Bình luận trải nghiệm
- Xem đánh giá từ người dùng khác

### 5️⃣ **Thanh Toán & Tài Chính**
- Nạp tiền qua VNPay
- Rút tiền về tài khoản ngân hàng
- Xem lịch sử giao dịch

### 6️⃣ **Quản Lý Giá (Chủ Sân)**
- Cập nhật giá sân chi tiết
- Xem lịch đặt của khách hàng
- Quản lý sân của mình

### 7️⃣ **Quản Trị Hệ Thống (Admin)**
- Duyệt sân bóng mới
- Quản lý đánh giá
- Xử lý yêu cầu rút tiền
- Thống kê hệ thống

---

## 📁 CẤU TRÚC DỰ ÁN

```
Nhom16_MVC/
├── Nhom16_MVC/                      # Backend (ASP.NET Core)
│   ├── Controllers/                 # API Endpoints
│   ├── Services/                    # Business Logic
│   ├── Repositories/                # Data Access
│   ├── Models/
│   │   ├── Entities/               # Database Models
│   │   └── DTOs/                   # Data Transfer Objects
│   ├── Data/
│   │   └── AppDbContext.cs         # Entity Framework Config
│   ├── Program.cs                  # Startup Config
│   ├── appsettings.json            # Configuration
│   └── fe/                         # Frontend (React)
│       ├── src/
│       │   ├── pages/              # Page Components
│       │   ├── components/         # Reusable Components
│       │   ├── services/           # API Services
│       │   ├── App.jsx             # Main App
│       │   └── main.jsx            # Entry Point
│       ├── package.json            # NPM Dependencies
│       ├── vite.config.js          # Vite Config
│       └── .env.development        # Environment Variables
├── SQL_SanBong_Moi.sql            # Database Schema & Seed Data
├── README.md                       # This file
├── SETUP.md                        # Detailed Setup Guide
└── .gitignore
```

---

## 🗄️ CÁCH KẾT NỐI DATABASE

### Database Hiện Tại
- **Provider**: PostgreSQL (Neon Cloud)
- **Host**: `ep-round-meadow-aozuay67-pooler.c-2.ap-southeast-1.aws.neon.tech`
- **Database**: `neondb`
- **Connection String** (trong `appsettings.json`):
```
Host=ep-round-meadow-aozuay67-pooler.c-2.ap-southeast-1.aws.neon.tech;
Database=neondb;
Username=neondb_owner;
Password=npg_LZkF4o6huAwt;
SSL Mode=Require;
Trust Server Certificate=true
```

### Cách Kết Nối

#### Option 1: Dùng Neon Console (Web)
1. Vào: https://console.neon.tech
2. Login & xem database trực tiếp
3. Query SQL online

#### Option 2: Dùng DBeaver (Desktop)
1. Tải DBeaver: https://dbeaver.io/
2. Create Connection:
   - Driver: PostgreSQL
   - Server: `ep-round-meadow-aozuay67-pooler.c-2.ap-southeast-1.aws.neon.tech`
   - Database: `neondb`
   - Username: `neondb_owner`
   - Password: `npg_LZkF4o6huAwt`
   - Port: 5432
   - SSL: Enable

#### Option 3: Dùng psql (CLI)
```bash
psql -h ep-round-meadow-aozuay67-pooler.c-2.ap-southeast-1.aws.neon.tech \
     -U neondb_owner \
     -d neondb \
     -W
```
(Password: `npg_LZkF4o6huAwt`)

### Database Schema
File **`SQL_SanBong_Moi.sql`** chứa:
- ✅ Toàn bộ schema (tables, views, triggers)
- ✅ Dữ liệu mẫu (sample data 10 dòng/bảng)
- ✅ Constraints & Foreign Keys
- ✅ Stored Procedures & Functions

---