import React, { useState } from 'react';
import { X } from 'lucide-react';
import './Admin.css';

export default function WithdrawalRejectModal({ withdrawal, token, BASE_URL, onRefresh, onClose }) {
    const [rejectReason, setRejectReason] = useState("");

    const handleConfirmReject = async () => {
        if (!rejectReason.trim()) {
            alert("Hệ thống bắt buộc nhập lý do hủy lệnh rút tiền để gửi email thông báo đối chiếu!");
            return;
        }

        try {
            const response = await fetch(`${BASE_URL}/FinancialManagement/process-withdrawal`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    maYeuCau: withdrawal.maYeuCau,
                    trangThaiMoi: 'tu_choi',
                    lyDoTuChoi: rejectReason
                })
            });
            const result = await response.json();
            alert(result.message || "Đã từ chối yêu cầu rút tiền thành công!");
            if (result.success) {
                onRefresh();
                onClose();
            }
        } catch (error) {
            alert("Gặp lỗi trong quá trình thực thi từ chối giao dịch.");
            console.error(error);
        }
    };

    return (
        <div style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(15, 23, 42, 0.4)', backdropFilter: 'blur(4px)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1000 }}>
            <div style={{ backgroundColor: '#ffffff', borderRadius: '16px', width: '90%', maxWidth: '480px', display: 'flex', flexDirection: 'column', overflow: 'hidden', boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)', padding: '24px', position: 'relative' }}>

                {/* Nút đóng góc phải */}
                <button onClick={onClose} style={{ position: 'absolute', top: '20px', right: '20px', background: 'transparent', border: 'none', cursor: 'pointer', color: '#64748b' }}>
                    <X size={20} />
                </button>

                {/* Tiêu đề modal */}
                <h3 style={{ fontSize: '18px', fontWeight: '700', color: '#0f172a', margin: '0 0 12px 0' }}>
                    Từ chối yêu cầu #WD-{withdrawal.maYeuCau}
                </h3>

                {/* Thông tin người rút */}
                <p style={{ fontSize: '14px', color: '#475569', margin: '0 0 20px 0', lineHeight: '1.5' }}>
                    Hủy lệnh rút của đối tác <strong>{withdrawal.tenNguoiDung}</strong>.<br />
                    Số tiền đề xuất: <strong style={{ color: '#dc2626' }}>{withdrawal.soTienRut?.toLocaleString()}đ</strong>.
                </p>

                {/* Form nhập lý do */}
                <div style={{ marginBottom: '24px' }}>
                    <label style={{ display: 'block', fontSize: '12px', fontWeight: '600', marginBottom: '8px', color: '#334155', textTransform: 'uppercase', letterSpacing: '0.5px' }}>
                        Lý do hủy (Nội dung phản hồi qua email):
                    </label>
                    <textarea
                        placeholder="Ví dụ: Tên chủ tài khoản ngân hàng không khớp với hồ sơ đăng ký, phát hiện hành vi tạo lịch đặt ảo nghi vấn trục lợi..."
                        value={rejectReason}
                        onChange={(e) => setRejectReason(e.target.value)}
                        style={{ width: '100%', height: '100px', padding: '12px', borderRadius: '8px', border: '1px solid #cbd5e1', fontSize: '14px', resize: 'none', outline: 'none', boxSizing: 'border-box', fontFamily: 'inherit' }}
                    />
                </div>

                {/* Thanh điều hướng nút bấm */}
                <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px' }}>
                    <button onClick={onClose} className="btn btn-outline" style={{ padding: '10px 20px' }}>
                        Hủy quay lại
                    </button>
                    <button onClick={handleConfirmReject} className="btn" style={{ backgroundColor: '#dc2626', color: '#ffffff', padding: '10px 20px' }}>
                        Xác nhận hủy chi
                    </button>
                </div>
            </div>
        </div>
    );
}