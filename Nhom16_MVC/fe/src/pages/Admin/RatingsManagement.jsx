import React, { useState, useEffect } from 'react';
import AdminLayout from './AdminLayout';
import { Search, Star, Trash2, RefreshCw } from 'lucide-react';

export default function RatingsManagement() {
    const token = localStorage.getItem('token');
    const BASE_URL = "https://localhost:7295/api";
    const [ratings, setRatings] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [loading, setLoading] = useState(true);

    const fetchRatings = async () => {
        setLoading(true);
        try {
            const res = await fetch(`${BASE_URL}/RatingManagement/all-ratings`, {
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` }
            });
            const result = await res.json();
            if (result.success) {
                setRatings(result.data || []);
            }
        } catch (error) {
            console.error("Lỗi kết nối API:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchRatings(); }, []);

    const handleDeleteRating = async (maDanhGia) => {
        if (!window.confirm("Báo cáo: Bạn có chắc chắn gỡ bỏ vĩnh viễn đánh giá này?")) return;
        try {
            const res = await fetch(`${BASE_URL}/RatingManagement/delete-rating`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({ maDanhGia })
            });
            const result = await res.json();
            if (result.success) {
                alert("Đã gỡ bỏ đánh giá thành công!");
                fetchRatings();
            } else {
                alert(result.message || "Không thể xóa đánh giá này.");
            }
        } catch (error) {
            console.error("Lỗi khi thực hiện xóa:", error);
        }
    };

    // 🚀 ĐÃ SỬA: Chống sập màn hình (nhận cả chữ HOA/thường từ BE) & Sửa lỗi tìm kiếm .toLowerCase()
    const filtered = ratings.filter(r => {
        const tenKhach = (r.tenNguoiDung || r.TenNguoiDung || "").toString().toLowerCase();
        const tenSan = (r.tenSanBong || r.TenSanBong || "").toString().toLowerCase();
        const tuKhoa = searchTerm ? searchTerm.toLowerCase() : "";

        return tenKhach.includes(tuKhoa) || tenSan.includes(tuKhoa);
    });

    return (
        <AdminLayout activeTab="ratings">
            <h1 className="page-title">Kiểm duyệt đánh giá</h1>
            <p className="page-subtitle" style={{ marginBottom: '24px' }}>Quản lý chất lượng nhận xét từ khách hàng</p>

            <div className="table-container">
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                    <div style={{ position: 'relative' }}>
                        <Search size={18} style={{ position: 'absolute', left: '12px', top: '10px', color: '#9ca3af' }} />
                        <input
                            type="text"
                            placeholder="Tìm người dùng, tên sân..."
                            value={searchTerm}
                            onChange={e => setSearchTerm(e.target.value)}
                            style={{ padding: '10px 10px 10px 36px', borderRadius: '8px', border: '1px solid #d1d5db', width: '320px' }}
                        />
                    </div>
                    <button onClick={fetchRatings} className="btn btn-outline">
                        <RefreshCw size={16} /> Làm mới
                    </button>
                </div>

                {loading ? (
                    <p style={{ textAlign: 'center', padding: '20px', color: '#6b7280' }}>Đang tải dữ liệu kiểm duyệt...</p>
                ) : filtered.length === 0 ? (
                    <p style={{ textAlign: 'center', padding: '20px', color: '#6b7280' }}>Không tìm thấy đánh giá nào trùng khớp.</p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>Khách hàng</th>
                                <th>Sân bóng</th>
                                <th>Số sao</th>
                                <th>Nội dung</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filtered.map(r => {
                                // Bọc lót lấy data linh hoạt Hoa/Thường để không bao giờ bị undefined
                                const id = r.maDanhGia || r.MaDanhGia;
                                const khachHang = r.tenNguoiDung || r.TenNguoiDung;
                                const sanBong = r.tenSanBong || r.TenSanBong;
                                const soSao = r.soSao ?? r.SoSao ?? 0;
                                const noiDung = r.noiDung || r.NoiDung;

                                return (
                                    <tr key={id}>
                                        <td style={{ fontWeight: 600 }}>{khachHang}</td>
                                        <td style={{ color: '#166534', fontWeight: 500 }}>{sanBong}</td>
                                        <td>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '4px', color: '#f59e0b', fontWeight: 600 }}>
                                                <Star size={16} fill="#f59e0b" /> {soSao}
                                            </div>
                                        </td>
                                        <td style={{ fontStyle: 'italic', color: '#4b5563' }}>"{noiDung}"</td>
                                        <td>
                                            <button onClick={() => handleDeleteRating(id)} className="btn-icon reject">
                                                <Trash2 size={16} />
                                            </button>
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                )}
            </div>
        </AdminLayout>
    );
}