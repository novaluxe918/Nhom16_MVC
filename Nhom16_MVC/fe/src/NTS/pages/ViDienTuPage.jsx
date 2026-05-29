import { useState, useEffect } from 'react';
import axios from 'axios';
import { Wallet, Plus, ArrowDownToLine, X } from 'lucide-react';
import Header from '../components/Header';
import Footer from '../components/Footer';

const ViDienTuPage = () => {
    // 🌟 KHÔNG DÙNG MOCK DATA - Gộp toàn bộ thông tin ví lấy từ DB thật
    const [walletData, setWalletData] = useState({
        hoTen: 'Đang tải...',
        avatar: '',
        soDu: 0,
        tenNganHang: 'Đang tải...',
        soTaiKhoan: 'Chưa có',
        lichSu: []
    });
    const [isLoading, setIsLoading] = useState(true);

    const [showRutTienModal, setShowRutTienModal] = useState(false);
    const [rutTienForm, setRutTienForm] = useState({ soTien: '', tenNganHang: '', soTaiKhoan: '' });
    const [isProcessing, setIsProcessing] = useState(false);

    // ==========================================
    // EFFECT: LẤY 100% THÔNG TIN VÍ TỪ DATABASE (C#)
    // ==========================================
    useEffect(() => {
        const fetchThongTinVi = async () => {
            try {
                const token = localStorage.getItem('token');
                const response = await axios.get('https://localhost:7295/api/GiaoDich/lich-su-vi', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                if (response.data.success || response.data.Success) {
                    const data = response.data.data || response.data.Data;
                    setWalletData({
                        hoTen: data.hoTen || data.HoTen,
                        avatar: data.avatar || data.Avatar,
                        soDu: data.soDu !== undefined ? data.soDu : data.SoDu,
                        tenNganHang: data.tenNganHang || data.TenNganHang || "Chưa liên kết",
                        soTaiKhoan: data.soTaiKhoan || data.SoTaiKhoan || "Chưa có STK",
                        lichSu: data.lichSu || data.LichSu || []
                    });
                }
            } catch (error) {
                console.error("Lỗi lấy thông tin ví từ hệ thống Database:", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchThongTinVi();
    }, []);

    // ==========================================
    // CHỨC NĂNG NẠP TIỀN QUA VNPAY (CALL API THẬT)
    // ==========================================
    const handleNapTien = async () => {
        const amount = window.prompt("Nhập số tiền muốn nạp (Tối thiểu 10,000 VNĐ):", "50000");
        if (!amount || isNaN(amount) || parseInt(amount) < 10000) return;

        try {
            const token = localStorage.getItem('token');
            const response = await axios.post('https://localhost:7295/api/GiaoDich/nap-tien', {
                SoTien: parseInt(amount)
            }, { headers: { Authorization: `Bearer ${token}` } });

            if (response.data.success || response.data.Success) {
                window.location.href = response.data.url || response.data.Url;
            } else {
                alert(`Lỗi: ${response.data.message || response.data.Message}`);
            }
        } catch (error) {
            console.error("Lỗi kết nối API nạp tiền:", error);
            alert("Không thể kết nối đến chức năng nạp tiền lúc này.");
        }
    };

    // ==========================================
    // CHỨC NĂNG RÚT TIỀN (CALL API THẬT)
    // ==========================================
    const handleRutTien = async (e) => {
        e.preventDefault();
        if (parseInt(rutTienForm.soTien) < 50000) return;

        setIsProcessing(true);
        try {
            const token = localStorage.getItem('token');
            const payload = {
                SoTien: parseInt(rutTienForm.soTien),
                TenNganHang: rutTienForm.tenNganHang,
                SoTaiKhoan: rutTienForm.soTaiKhoan,
                MoTa: "Rút tiền về ngân hàng"
            };

            const response = await axios.post('https://localhost:7295/api/GiaoDich/rut-tien', payload, {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.data.success || response.data.Success) {
                alert("✅ Gửi yêu cầu rút tiền thành công! Vui lòng chờ phê duyệt.");
                setShowRutTienModal(false);
                setRutTienForm({ soTien: '', tenNganHang: '', soTaiKhoan: '' });
                window.location.reload();
            }
        } catch (error) {
            console.error("Lỗi kết nối API rút tiền:", error);
            const loiTuBackend = error.response?.data?.message || error.response?.data?.Message || error.message;
            alert("❌ Server C# báo lỗi: " + loiTuBackend);
        } finally {
            setIsProcessing(false);
        }
    };

    return (
        <div className="bg-[#f5f7f5] min-h-screen flex flex-col font-body text-[#2c2f2e] relative">
            <Header />

            <main className="pt-24 pb-20 px-4 md:px-8 max-w-7xl mx-auto w-full flex-grow">
                <div className="flex items-center gap-6 mb-8 bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
                    <div className="relative">
                        <img
                            src={walletData.avatar || `https://ui-avatars.com/api/?name=${walletData.hoTen || 'User'}&background=006b0a&color=fff&size=150`}
                            alt="Avatar"
                            className="w-24 h-24 rounded-2xl object-cover bg-gray-100"
                        />
                    </div>
                    <div>
                        <h1 className="text-3xl font-black mb-1">{walletData.hoTen}</h1>
                        <p className="text-gray-500">Người thuê sân (Mã tài khoản: User 2)</p>
                    </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
                    <div className="lg:col-span-8 space-y-8">

                        <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 relative">
                            <p className="text-sm font-bold text-gray-500 uppercase tracking-widest mb-2">Số dư hiện tại</p>
                            <div className="flex items-baseline gap-2 mb-8">
                                <span className="text-5xl font-black text-[#006b0a]">
                                    {isLoading ? '...' : (walletData.soDu || 0).toLocaleString()}
                                </span>
                                <span className="text-xl font-bold text-gray-400">VND</span>
                            </div>
                            <div className="flex gap-4">
                                <button onClick={handleNapTien} className="flex items-center gap-2 bg-[#006b0a] text-white px-6 py-3 rounded-xl font-bold hover:bg-[#005d07] transition shadow-md shadow-green-600/10">
                                    <Plus size={20} /> Nạp tiền
                                </button>
                                <button onClick={() => setShowRutTienModal(true)} className="flex items-center gap-2 bg-gray-100 text-gray-700 px-6 py-3 rounded-xl font-bold hover:bg-gray-200 transition">
                                    <ArrowDownToLine size={20} /> Rút tiền
                                </button>
                            </div>
                        </div>

                        <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
                            <div className="p-6 border-b border-gray-100">
                                <h3 className="text-xl font-black">Lịch sử giao dịch</h3>
                            </div>
                            <div className="overflow-x-auto">
                                {isLoading ? (
                                    <div className="p-10 text-center font-bold text-gray-400">Đang truy vấn dữ liệu từ Database...</div>
                                ) : (walletData.lichSu || []).length === 0 ? (
                                    <div className="p-10 text-center font-bold text-gray-400">Tài khoản chưa phát sinh giao dịch tài chính nào.</div>
                                ) : (
                                    <table className="w-full text-left border-collapse">
                                        <thead>
                                            <tr className="bg-gray-50 text-gray-500 text-xs uppercase tracking-wider">
                                                <th className="p-6 font-bold">Ngày giờ</th>
                                                <th className="p-6 font-bold">Hoạt động</th>
                                                <th className="p-6 font-bold">Số tiền</th>
                                                <th className="p-6 font-bold">Trạng thái</th>
                                            </tr>
                                        </thead>
                                        <tbody className="divide-y divide-gray-100">
                                            {(walletData.lichSu || []).map((tx, idx) => {
                                                const ngay = tx.ngayGiaoDich || tx.NgayGiaoDich;
                                                const hoatDong = tx.loaiHoatDong || tx.LoaiHoatDong;
                                                const soTien = tx.soTien !== undefined ? tx.soTien : tx.SoTien;
                                                const isPositive = tx.isPositive !== undefined ? tx.isPositive : tx.IsPositive;
                                                // Bọc lót an toàn: lỡ DB rỗng thì gán thành chuỗi rỗng để lệnh .includes() không bị lỗi sập
                                                const trangThai = tx.trangThai || tx.TrangThai || '';

                                                return (
                                                    <tr key={idx} className="hover:bg-gray-50/50 transition">
                                                        <td className="p-6 text-sm text-gray-600">{ngay}</td>
                                                        <td className="p-6 font-semibold text-sm">{hoatDong}</td>
                                                        <td className={`p-6 font-black text-sm ${isPositive ? 'text-[#006b0a]' : 'text-red-600'}`}>
                                                            {isPositive ? '+' : '-'}{(soTien || 0).toLocaleString()}đ
                                                        </td>
                                                        <td className="p-6">
                                                            <span className={`px-3 py-1 rounded-md text-[10px] font-black uppercase tracking-widest ${trangThai.includes('THANH_CONG') || trangThai.includes('DA_CHUYEN')
                                                                    ? 'bg-green-100 text-[#005406]'
                                                                    : 'bg-yellow-100 text-yellow-700'
                                                                }`}>
                                                                {trangThai}
                                                            </span>
                                                        </td>
                                                    </tr>
                                                );
                                            })}
                                        </tbody>
                                    </table>
                                )}
                            </div>
                        </div>
                    </div>

                    <div className="lg:col-span-4 space-y-6">
                        <div className="bg-white p-6 rounded-3xl shadow-sm border border-gray-100">
                            <h4 className="font-bold text-lg text-[#2c2f2e] mb-6">Ngân hàng mặc định</h4>

                            <div className="bg-gray-50 p-4 rounded-xl flex items-center gap-4 border border-gray-100">
                                <div className="w-12 h-12 bg-[#006b0a]/10 rounded-xl flex items-center justify-center font-black text-lg">
                                    🏦
                                </div>
                                <div>
                                    <p className="font-black text-sm text-[#2c2f2e]">{walletData.tenNganHang}</p>
                                    <p className="text-xs text-gray-500 font-mono tracking-wider mt-0.5">{walletData.soTaiKhoan}</p>
                                </div>
                            </div>
                        </div>

                        <div className="bg-[#2c2f2e] text-white p-8 rounded-3xl relative overflow-hidden">
                            <div className="relative z-10">
                                <h4 className="text-2xl font-black mb-2">Quản lý<br />Tài chính an toàn</h4>
                                <p className="text-gray-400 text-sm mt-2">Thông tin tài khoản thụ hưởng đồng bộ trực tiếp dựa trên lệnh giao dịch gần nhất.</p>
                            </div>
                            <Wallet className="absolute -right-8 -bottom-8 text-white opacity-10" size={150} strokeWidth={1} />
                        </div>
                    </div>
                </div>
            </main>
            <Footer />

            {showRutTienModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm px-4">
                    <div className="bg-white w-full max-w-md rounded-3xl shadow-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
                        <div className="p-6 border-b border-gray-100 flex justify-between items-center">
                            <h3 className="text-xl font-black text-[#006b0a]">Rút tiền</h3>
                            <button type="button" onClick={() => setShowRutTienModal(false)} className="text-gray-400 hover:text-red-500 transition">
                                <X size={24} />
                            </button>
                        </div>
                        <form onSubmit={handleRutTien} className="p-6 space-y-5">
                            <div>
                                <label className="text-xs font-bold text-gray-500 uppercase">Số tiền rút</label>
                                <input
                                    type="number"
                                    required
                                    min="50000"
                                    value={rutTienForm.soTien}
                                    onChange={e => setRutTienForm({ ...rutTienForm, soTien: e.target.value })}
                                    className="w-full bg-gray-50 border p-3 rounded-xl mt-2 font-bold focus:outline-none focus:ring-2 focus:ring-[#006b0a]"
                                    placeholder="Tối thiểu 50,000đ"
                                />
                            </div>
                            <div>
                                <label className="text-xs font-bold text-gray-500 uppercase">Ngân hàng</label>
                                <input
                                    type="text"
                                    required
                                    value={rutTienForm.tenNganHang}
                                    onChange={e => setRutTienForm({ ...rutTienForm, tenNganHang: e.target.value })}
                                    className="w-full bg-gray-50 border p-3 rounded-xl mt-2 focus:outline-none focus:ring-2 focus:ring-[#006b0a]"
                                    placeholder="Ví dụ: Vietcombank, MB Bank..."
                                />
                            </div>
                            <div>
                                <label className="text-xs font-bold text-gray-500 uppercase">Số tài khoản</label>
                                <input
                                    type="text"
                                    required
                                    value={rutTienForm.soTaiKhoan}
                                    onChange={e => setRutTienForm({ ...rutTienForm, soTaiKhoan: e.target.value })}
                                    className="w-full bg-gray-50 border p-3 rounded-xl mt-2 font-mono focus:outline-none focus:ring-2 focus:ring-[#006b0a]"
                                    placeholder="Nhập số tài khoản nhận tiền"
                                />
                            </div>
                            <div className="pt-4 flex gap-3">
                                <button type="button" onClick={() => setShowRutTienModal(false)} className="flex-1 py-3 bg-gray-100 text-gray-600 font-bold rounded-xl hover:bg-gray-200 transition">
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    disabled={isProcessing}
                                    className={`flex-1 py-3 font-bold rounded-xl transition ${isProcessing ? 'bg-gray-400 text-white cursor-wait' : 'bg-[#006b0a] text-white hover:bg-[#005d07]'}`}
                                >
                                    {isProcessing ? 'Đang xử lý...' : 'Xác nhận rút'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ViDienTuPage;