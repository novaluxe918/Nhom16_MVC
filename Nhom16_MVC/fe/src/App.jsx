import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './NTS/pages/HomePage';
import CumSanPage from './NTS/pages/CumSanPage';
import ChiTietSanPage from './NTS/pages/ChiTietSanPage';
import LichSuDatSanPage from './NTS/pages/LichSuDatSanPage';
import DanhGiaSanPage from './NTS/pages/DanhGiaSanPage';
import ViDienTuPage from './NTS/pages/ViDienTuPage';

function App() {
    return (
        // BrowserRouter: Bọc toàn bộ ứng dụng, khởi động radar theo dõi URL
        <BrowserRouter>
            {/* Routes: Nơi khai báo danh sách các ngã rẽ */}
            <Routes>

                {/* Route: Một ngã rẽ cụ thể. 
            path="/": Đường dẫn gốc (Trang chủ)
            element: Vẽ cái Component nào ra? */}
                <Route path="/" element={<HomePage />} />
                <Route path="/cum-san/:id" element={<CumSanPage />} />
                <Route path="/san-con/:id" element={<ChiTietSanPage />} />
                <Route path="/lich-su-dat-san" element={<LichSuDatSanPage />} />
                <Route path="/danh-gia/:id" element={<DanhGiaSanPage />} />
                <Route path="/wallet" element={<ViDienTuPage />} />


                {/* Ví dụ các trang bạn sẽ làm tiếp theo */}
                {/* <Route path="/profile" element={<ProfilePage />} /> */}
                {/* <Route path="/san-con/:id" element={<ChiTietSanPage />} /> */}

            </Routes>
        </BrowserRouter>
    );
}

export default App;
﻿// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import VerifyEmail from './pages/Auth/VerifyEmail';
import ForgotPassword from './pages/Auth/ForgotPassword';

// --- ĐƯỜNG DẪN CÁC TRANG CỦA CHỦ SÂN ---
import QuanLySanPage from './pages/Owner/QuanLySanPage';
import DonDatSanPage from './pages/Owner/DonDatSanPage';
import ThemSanBong from './pages/Owner/ThemSanBong';

// --- IMPORT CÁC TRANG CỦA ADMIN (Cập nhật đường dẫn cho đúng cấu trúc thư mục của bạn) ---
import DashboardOverview from './pages/Admin/DashboardOverview';
import UsersManagement from './pages/Admin/UsersManagement';
import StadiumsApproval from './pages/Admin/StadiumsApproval';
import FinancialManagement from './pages/Admin/FinancialManagement';
import RatingsManagement from './pages/Admin/RatingsManagement';

// Component bảo vệ route (Kiểm tra đăng nhập và phân quyền)
const ProtectedRoute = ({ children, roleRequired }) => {
    const token = localStorage.getItem('token');
    const userVaiTro = localStorage.getItem('userVaiTro');

    if (!token) return <Navigate to="/" replace />;
    if (roleRequired && userVaiTro !== roleRequired) return <Navigate to="/" replace />;

    return children;
};

export default function App() {
    return (
        <Router>
            <Routes>
                {/* Trang gốc mặc định bắt buộc vào Login trước */}
                <Route path="/" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/verify-email" element={<VerifyEmail />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />

                {/* ================= CÁC ROUTE DÀNH CHO ADMIN ================= */}
                <Route path="/admin" element={
                    <ProtectedRoute roleRequired="admin">
                        <DashboardOverview />
                    </ProtectedRoute>
                } />
                <Route path="/admin/users" element={
                    <ProtectedRoute roleRequired="admin">
                        <UsersManagement />
                    </ProtectedRoute>
                } />
                <Route path="/admin/stadiums" element={
                    <ProtectedRoute roleRequired="admin">
                        <StadiumsApproval />
                    </ProtectedRoute>
                } />
                <Route path="/admin/financial" element={
                    <ProtectedRoute roleRequired="admin">
                        <FinancialManagement />
                    </ProtectedRoute>
                } />
                <Route path="/admin/ratings" element={
                    <ProtectedRoute roleRequired="admin">
                        <RatingsManagement />
                    </ProtectedRoute>
                } />

                {/* ================= CÁC ROUTE DÀNH CHO CHỦ SÂN ================= */}
                <Route path="/quan-ly-san" element={
                    <ProtectedRoute roleRequired="chuSan">
                        <QuanLySanPage />
                    </ProtectedRoute>
                } />
                <Route path="/don-dat-san" element={
                    <ProtectedRoute roleRequired="chuSan">
                        <DonDatSanPage />
                    </ProtectedRoute>
                } />
                <Route path="/them-san" element={
                    <ProtectedRoute roleRequired="chuSan">
                        <ThemSanBong />
                    </ProtectedRoute>
                } />

                {/* Tự động chuyển hướng nếu người dùng gõ linh tinh */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
}
