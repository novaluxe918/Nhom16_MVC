import React, { useState, useEffect } from 'react';
import AdminLayout from './AdminLayout';
import { Search, RefreshCw, CheckCircle, XCircle } from 'lucide-react';
import './Admin.css';

export default function FinancialManagement() {
    const token = localStorage.getItem('token');
    const BASE_URL = "https://localhost:7295/api";

    const [withdrawals, setWithdrawals] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [loading, setLoading] = useState(true);

    const fetchPendingWithdrawals = async () => {
        setLoading(true);
        try {
            const res = await fetch(`${BASE_URL}/FinancialManagement/pending-withdrawals`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const data = await res.json();
                setWithdrawals(data || []); // Dữ liệu trả về thẳng mảng
            }
        } catch (error) {
            console.error("Lỗi lấy dữ liệu tài chính:", error);
        } finally { setLoading(false); }
    };

    useEffect(() => { fetchPendingWithdrawals(); }, []);

    // Xử lý Duyệt hoặc Từ chối giải ngân
    const handleProcessWithdrawal = async (maYeuCau, actionType) => {
        let lyDoTuChoi = "";
        if (actionType === "tu_choi") {
            lyDoTuChoi = prompt("Nhập lý do từ chối lệnh giải ngân này:");
            if (!lyDoTuChoi) {
                alert("Bạn phải nhập lý do từ chối!");
                return;
            }
        } else {
            if (!window.confirm("Xác nhận đã chuyển khoản thành công, tiến hành trừ tiền ví đối tác?")) return;
        }

        try {
            const res = await fetch(`${BASE_URL}/FinancialManagement/process-withdrawal`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({
                    maYeuCau: maYeuCau,
                    trangThaiMoi: actionType, // "da_chuyen" hoặc "tu_choi"
                    lyDoTuChoi: lyDoTuChoi
                })
            });
            const result = await res.json();
            alert(result.message || result.Message);
            fetchPendingWithdrawals();
        } catch (error) {
            console.error("Lỗi xử lý giao dịch:", error);
        }
    };

    const dynamicPendingCount = withdrawals.length;
    const dynamicTotalAmount = withdrawals.reduce((sum, item) => sum + (item.soTienRut || 0), 0);

    const filteredWithdrawals = withdrawals.filter(w =>
        w.tenNguoiDung?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        w.maYeuCau?.toString().includes(searchTerm)
    );

    return (
        <AdminLayout activeTab="finance">
            <h1 className="page-title">Quản lý giao dịch giải ngân</h1>
            <p className="page-subtitle" style={{ marginBottom: '24px' }}>Phê duyệt giải ngân tiền từ tài khoản ví của các chủ sân về ngân hàng.</p>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', marginBottom: '32px' }}>
                <div className="stat-card" style={{ borderTop: '4px solid #f59e0b' }}>
                    <div className="stat-label">YÊU CẦU CHỜ XỬ LÝ</div>
                    <div style={{ fontSize: '28px', fontWeight: 700, margin: '8px 0' }}>{dynamicPendingCount}</div>
                    <div style={{ fontSize: '13px', color: '#6b7280' }}>Tổng số tiền cần chi: <strong>{dynamicTotalAmount.toLocaleString()}đ</strong></div>
                </div>
                <div className="stat-card" style={{ borderTop: '4px solid #10b981' }}>
                    <div className="stat-label">TRẠNG THÁI KẾT NỐI</div>
                    <div style={{ fontSize: '18px', fontWeight: 700, margin: '16px 0 8px 0', color: '#10b981' }}>● POSTGRESQL CONNECTED</div>
                    <div style={{ fontSize: '12px', color: '#6b7280' }}>Hệ thống giao dịch tự động trừ ví khi giải ngân thành công.</div>
                </div>
            </div>

            <div className="table-container">
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                    <div style={{ position: 'relative' }}>
                        <Search size={18} style={{ position: 'absolute', left: '12px', top: '10px', color: '#9ca3af' }} />
                        <input type="text" placeholder="Tìm tên chủ sân, mã lệnh..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} style={{ padding: '10px 10px 10px 36px', borderRadius: '8px', border: '1px solid #d1d5db', width: '320px' }} />
                    </div>
                    <button onClick={fetchPendingWithdrawals} className="btn btn-outline"><RefreshCw size={16} /> Làm mới</button>
                </div>

                {loading ? <p>Đang tải dữ liệu giao dịch tài chính...</p> : (
                    <table>
                        <thead>
                            <tr>
                                <th>Mã Yêu Cầu</th>
                                <th>Tên Chủ Sân</th>
                                <th>Tài Khoản Thụ Hưởng</th>
                                <th>Số Tiền Rút</th>
                                <th>Trạng Thái</th>
                                <th style={{ textAlign: 'center' }}>Thao Tác Duyệt</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredWithdrawals.length === 0 ? (
                                <tr>
                                    <td colSpan="6" style={{ textAlign: 'center', color: '#6b7280', padding: '24px' }}>Không có lệnh rút tiền nào đang chờ giải ngân.</td>
                                </tr>
                            ) : (
                                filteredWithdrawals.map((w) => (
                                    <tr key={w.maYeuCau}>
                                        <td style={{ fontWeight: 600 }}>#REQ-{w.maYeuCau}</td>
                                        <td>
                                            <div style={{ fontWeight: 600 }}>{w.tenNguoiDung}</div>
                                            <div style={{ fontSize: '12px', color: '#6b7280' }}>{w.email}</div>
                                        </td>
                                        <td><span style={{ fontFamily: 'monospace', background: '#f1f5f9', padding: '4px 8px', borderRadius: '4px' }}>{w.thongTinNganHang}</span></td>
                                        <td style={{ color: '#b91c1c', fontWeight: 700 }}>{w.soTienRut?.toLocaleString()}đ</td>
                                        <td><span className="badge" style={{ background: '#fef3c7', color: '#d97706' }}>{w.trangThai}</span></td>
                                        <td style={{ textAlign: 'center' }}>
                                            <button onClick={() => handleProcessWithdrawal(w.maYeuCau, "da_chuyen")} className="btn btn-primary" style={{ padding: '6px 12px', fontSize: '12px', marginRight: '8px', background: '#16a34a' }} title="Xác nhận đã chuyển tiền thành công">
                                                Duyệt cấp tiền
                                            </button>
                                            <button onClick={() => handleProcessWithdrawal(w.maYeuCau, "tu_choi")} className="btn btn-outline" style={{ padding: '6px 12px', fontSize: '12px', color: '#dc2626', borderColor: '#dc2626' }} title="Từ chối yêu cầu">
                                                Từ chối
                                            </button>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                )}
            </div>
        </AdminLayout>
    );
}