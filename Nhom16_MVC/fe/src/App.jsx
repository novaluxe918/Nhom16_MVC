import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

// ================= THƯ VIỆN COMPONENT AUTH =================
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import VerifyEmail from './pages/Auth/VerifyEmail';
import ForgotPassword from './pages/Auth/ForgotPassword';

// ================= THƯ VIỆN COMPONENT NGƯỜI THUÊ SÂN =================
import HomePage from './NTS/pages/HomePage';
import CumSanPage from './NTS/pages/CumSanPage';
import ChiTietSanPage from './NTS/pages/ChiTietSanPage';
import LichSuDatSanPage from './NTS/pages/LichSuDatSanPage';
import DanhGiaSanPage from './NTS/pages/DanhGiaSanPage';
import ViDienTuPage from './NTS/pages/ViDienTuPage';

// ================= THƯ VIỆN COMPONENT CHỦ SÂN =================
import QuanLySanPage from './pages/Owner/QuanLySanPage';
import DonDatSanPage from './pages/Owner/DonDatSanPage';
import ThemSanBong from './pages/Owner/ThemSanBong';

// ================= THƯ VIỆN COMPONENT ADMIN =================
import DashboardOverview from './pages/Admin/DashboardOverview';
import UsersManagement from './pages/Admin/UsersManagement';
import StadiumsApproval from './pages/Admin/StadiumsApproval';
import FinancialManagement from './pages/Admin/FinancialManagement';
import RatingsManagement from './pages/Admin/RatingsManagement';

// 🛡️ Component bảo vệ route (Kiểm tra đăng nhập và phân quyền)
const ProtectedRoute = ({ children, roleRequired }) => {
    const token = localStorage.getItem('token');
    const userVaiTro = localStorage.getItem('userVaiTro');

    if (!token) return <Navigate to="/login" replace />;
    if (roleRequired && userVaiTro !== roleRequired) return <Navigate to="/" replace />;

    return children;
};

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                {/* ================= NGÃ RẼ CHO KHÁCH & NGƯỜI THUÊ SÂN (PUBLIC) ================= */}
                <Route path="/" element={<HomePage />} />
                <Route path="/cum-san/:id" element={<CumSanPage />} />
                <Route path="/san-con/:id" element={<ChiTietSanPage />} />
                <Route path="/lich-su-dat-san" element={<LichSuDatSanPage />} />
                <Route path="/danh-gia/:id" element={<DanhGiaSanPage />} />
                <Route path="/wallet" element={<ViDienTuPage />} />

                {/* ================= NGÃ RẼ ĐĂNG NHẬP / ĐĂNG KÝ ================= */}
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/verify-email" element={<VerifyEmail />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />

                {/* ================= NGÃ RẼ BẢO MẬT: ADMIN ================= */}
                <Route path="/admin" element={<ProtectedRoute roleRequired="admin"><DashboardOverview /></ProtectedRoute>} />
                <Route path="/admin/users" element={<ProtectedRoute roleRequired="admin"><UsersManagement /></ProtectedRoute>} />
                <Route path="/admin/stadiums" element={<ProtectedRoute roleRequired="admin"><StadiumsApproval /></ProtectedRoute>} />
                <Route path="/admin/financial" element={<ProtectedRoute roleRequired="admin"><FinancialManagement /></ProtectedRoute>} />
                <Route path="/admin/ratings" element={<ProtectedRoute roleRequired="admin"><RatingsManagement /></ProtectedRoute>} />

                {/* ================= NGÃ RẼ BẢO MẬT: CHỦ SÂN ================= */}
                <Route path="/quan-ly-san" element={<ProtectedRoute roleRequired="chuSan"><QuanLySanPage /></ProtectedRoute>} />
                <Route path="/don-dat-san" element={<ProtectedRoute roleRequired="chuSan"><DonDatSanPage /></ProtectedRoute>} />
                <Route path="/them-san" element={<ProtectedRoute roleRequired="chuSan"><ThemSanBong /></ProtectedRoute>} />

                {/* Tự động chuyển hướng về Trang chủ nếu người dùng gõ link tinh (Bẫy lỗi 404) */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}