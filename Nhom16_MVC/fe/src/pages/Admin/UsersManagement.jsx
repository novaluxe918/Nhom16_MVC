import React, { useState, useEffect } from 'react';
import AdminLayout from './AdminLayout';
import { Search, Download, Eye, RefreshCw, Lock, Unlock } from 'lucide-react';
import UserDetailModal from './UserDetailModal';
import './Admin.css';

export default function UsersManagement() {
    const token = localStorage.getItem('token');
    const BASE_URL = "https://localhost:7295/api";

    const [users, setUsers] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [loading, setLoading] = useState(true);

    const [selectedUser, setSelectedUser] = useState(null);
    const [showModal, setShowModal] = useState(false);

    const fetchUsers = async () => {
        setLoading(true);
        try {
            // Đúng route API của UserManagementController
            const res = await fetch(`${BASE_URL}/UserManagement/users`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` }
            });
            const result = await res.json();
            // Backend trả về đối tượng UserManagementResponse { success, message, data }
            if (result.success || result.Success) {
                setUsers(result.data || result.Data || []);
            }
        } catch (error) {
            console.error("Lỗi tải danh sách user:", error);
        } finally { setLoading(false); }
    };

    useEffect(() => { fetchUsers(); }, []);

    // Xử lý Khóa / Mở khóa tài khoản đồng bộ với ToggleLockDto của backend
    const handleToggleLock = async (user) => {
        const isCurrentlyLocked = user.trangThai === 'bi_khoa';
        let lockReasonType = "khac";
        let customReason = "";

        if (!isCurrentlyLocked) {
            const reasonChoice = prompt("Chọn lý do khóa tài khoản:\n1. SPAM (Hủy lịch bừa bãi)\n2. GIA_MAO (Thông tin giả mạo)\n3. KHAC (Lý do khác)\nNhập số 1, 2 hoặc 3:");
            if (!reasonChoice) return;

            if (reasonChoice === "1") lockReasonType = "spam";
            else if (reasonChoice === "2") lockReasonType = "gia_mao";
            else {
                customReason = prompt("Nhập lý do cụ thể:");
                if (!customReason) return;
            }
        } else {
            if (!window.confirm(`Bạn có chắc chắn muốn MỞ KHÓA cho tài khoản ${user.hoTen}?`)) return;
        }

        try {
            const res = await fetch(`${BASE_URL}/UserManagement/toggle-lock`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({
                    maNguoiDung: user.maNguoiDung,
                    isLocked: !isCurrentlyLocked,
                    lockReasonType: lockReasonType,
                    customReason: customReason
                })
            });
            const result = await res.json();
            alert(result.message || result.Message);
            fetchUsers();
        } catch (error) {
            console.error("Lỗi thực hiện lệnh khóa:", error);
        }
    };

    const filteredUsers = users.filter(u =>
        u.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        u.email?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <AdminLayout activeTab="users">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                <h1 className="page-title">Quản lý người dùng</h1>
                <button onClick={fetchUsers} className="btn btn-outline"><RefreshCw size={16} /> Làm mới</button>
            </div>

            <div className="table-container">
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                    <div style={{ position: 'relative' }}>
                        <Search size={18} style={{ position: 'absolute', left: '12px', top: '10px', color: '#9ca3af' }} />
                        <input
                            type="text"
                            placeholder="Tìm kiếm tên hoặc email..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            style={{ padding: '10px 10px 10px 36px', borderRadius: '8px', border: '1px solid #d1d5db', width: '300px' }}
                        />
                    </div>
                </div>

                {loading ? <p>Đang tải dữ liệu người dùng...</p> : (
                    <table>
                        <thead>
                            <tr>
                                <th>Họ và Tên</th>
                                <th>Email</th>
                                <th>Số điện thoại</th>
                                <th>Vai trò</th>
                                <th>Số dư ví</th>
                                <th>Trạng thái</th>
                                <th style={{ textAlign: 'center' }}>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.map((user) => (
                                <tr key={user.maNguoiDung}>
                                    <td style={{ fontWeight: 600 }}>{user.hoTen}</td>
                                    <td style={{ color: '#6b7280' }}>{user.email}</td>
                                    <td>{user.soDienThoai || '---'}</td>
                                    <td>
                                        <span className="badge" style={{ background: user.vaiTro === 'Chủ Sân' ? '#e0e7ff' : '#f1f5f9', color: user.vaiTro === 'Chủ Sân' ? '#4f46e5' : '#475569' }}>
                                            {user.vaiTro}
                                        </span>
                                    </td>
                                    <td style={{ fontWeight: '600' }}>{user.soDuTaiKhoan?.toLocaleString()}đ</td>
                                    <td>
                                        <span style={{ color: user.trangThai === 'bi_khoa' ? '#dc2626' : '#16a34a', fontWeight: 500 }}>
                                            ● {user.trangThai === 'bi_khoa' ? 'Bị khóa' : 'Hoạt động'}
                                        </span>
                                    </td>
                                    <td style={{ textAlign: 'center' }}>
                                        <button onClick={() => { setSelectedUser(user); setShowModal(true); }} className="btn-icon" title="Xem chi tiết" style={{ marginRight: '8px' }}>
                                            <Eye size={18} />
                                        </button>
                                        <button onClick={() => handleToggleLock(user)} className="btn-icon" style={{ color: user.trangThai === 'bi_khoa' ? '#16a34a' : '#dc2626' }} title={user.trangThai === 'bi_khoa' ? "Mở khóa" : "Khóa tài khoản"}>
                                            {user.trangThai === 'bi_khoa' ? <Unlock size={18} /> : <Lock size={18} />}
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>

            {showModal && selectedUser && (
                <UserDetailModal user={selectedUser} onClose={() => setShowModal(false)} />
            )}
        </AdminLayout>
    );
}