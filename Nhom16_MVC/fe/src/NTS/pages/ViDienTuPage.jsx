import  { useState } from 'react';
import axios from 'axios';
import { CreditCard, Wallet, Plus, ArrowDownToLine, History, Gift, MapPin, Mail, Phone, Share2 } from 'lucide-react';
import Header from '../components/Header';
import Footer from '../components/Footer';

const ViDienTuPage = () => {
    // Mock data - Sau này sẽ thay bằng dữ liệu gọi từ API GetProfile
    const [balance] = useState(1250000);
    const [transactions] = useState([
        { id: 1, date: '24/05/2026', type: 'Nạp tiền', amount: 500000, status: 'THÀNH CÔNG', isPositive: true },
        { id: 2, date: '22/05/2026', type: 'Đặt sân Tuyên Sơn', amount: -250000, status: 'THÀNH CÔNG', isPositive: false },
        { id: 3, date: '20/05/2026', type: 'Hoàn tiền (Hủy sân)', amount: 150000, status: 'CHỜ DUYỆT', isPositive: true },
    ]);

    // Xử lý gọi API Nạp tiền qua VNPay
    const handleNapTien = async () => {
        const amount = window.prompt("Nhập số tiền muốn nạp (VNĐ):", "100000");
        if (!amount || isNaN(amount) || amount < 10000) {
            alert("Số tiền không hợp lệ. Vui lòng nhập số lớn hơn 10,000đ.");
            return;
        }

        try {
            // C# Endpoint xử lý tạo URL VNPay
            const response = await axios.post('https://localhost:7295/api/ThanhToan/tao-url-vnpay', {
                soTien: parseInt(amount)
            }, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
            });

            if (response.data.success && response.data.paymentUrl) {
                // Chuyển hướng người dùng sang cổng thanh toán VNPay
                window.location.href = response.data.paymentUrl;
            }
        } catch (error) {
            console.error("Lỗi tạo thanh toán VNPay:", error);
            alert("Không thể kết nối đến cổng thanh toán lúc này.");
        }
    };

    return (
        <div className="bg-[#f5f7f5] min-h-screen flex flex-col font-body text-[#2c2f2e]">
            <Header />

            <main className="pt-24 pb-20 px-4 md:px-8 max-w-7xl mx-auto w-full flex-grow">
                {/* Header Profile */}
                <div className="flex items-center gap-6 mb-8 bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
                    <div className="relative">
                        <img src="https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=150&q=80" alt="Avatar" className="w-24 h-24 rounded-2xl object-cover" />
                        <span className="absolute -bottom-2 -right-2 bg-[#59ee50] text-[#005406] p-1.5 rounded-full border-2 border-white">
                            <Wallet size={16} />
                        </span>
                    </div>
                    <div>
                        <p className="text-xs font-bold text-[#006b0a] uppercase tracking-widest mb-1">Thành viên Vàng</p>
                        <h1 className="text-3xl font-black mb-1">Trần Anh Đức</h1>
                        <p className="text-gray-500">Người thuê sân</p>
                    </div>
                    <div className="ml-auto flex gap-3">
                        <button className="px-5 py-2.5 bg-gray-100 font-bold rounded-lg hover:bg-gray-200 transition">Sửa hồ sơ</button>
                        <button className="px-5 py-2.5 bg-[#006b0a] text-white font-bold rounded-lg hover:bg-[#005d07] transition shadow-lg shadow-green-600/20">Đặt sân ngay</button>
                    </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
                    {/* Cột Trái: Ví & Lịch sử */}
                    <div className="lg:col-span-8 space-y-8">
                        {/* Thẻ Ví */}
                        <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 relative overflow-hidden">
                            <Wallet className="absolute -right-6 -top-6 text-gray-50 opacity-50" size={200} strokeWidth={1} />
                            <div className="relative z-10">
                                <p className="text-sm font-bold text-gray-500 uppercase tracking-widest mb-2">Số dư hiện tại</p>
                                <div className="flex items-baseline gap-2 mb-8">
                                    <span className="text-5xl font-black text-[#006b0a]">{balance.toLocaleString()}</span>
                                    <span className="text-xl font-bold text-gray-400">VND</span>
                                </div>
                                <div className="flex gap-4">
                                    <button
                                        onClick={handleNapTien}
                                        className="flex items-center gap-2 bg-[#006b0a] text-white px-6 py-3 rounded-xl font-bold hover:bg-[#005d07] transition hover:-translate-y-1"
                                    >
                                        <Plus size={20} /> Nạp tiền
                                    </button>
                                    <button className="flex items-center gap-2 bg-gray-100 text-gray-700 px-6 py-3 rounded-xl font-bold hover:bg-gray-200 transition">
                                        <ArrowDownToLine size={20} /> Rút tiền
                                    </button>
                                </div>
                            </div>
                        </div>

                        {/* Lịch sử giao dịch */}
                        <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
                            <div className="p-6 flex justify-between items-center border-b border-gray-100">
                                <h3 className="text-xl font-black">Lịch sử giao dịch</h3>
                                <button className="flex items-center gap-2 text-sm font-bold text-[#006b0a] bg-[#59ee50]/20 px-4 py-2 rounded-lg hover:bg-[#59ee50]/30 transition">
                                    <History size={16} /> Tải báo cáo (CSV)
                                </button>
                            </div>
                            <div className="overflow-x-auto">
                                <table className="w-full text-left border-collapse">
                                    <thead>
                                        <tr className="bg-gray-50 text-gray-500 text-xs uppercase tracking-wider">
                                            <th className="p-6 font-bold">Ngày giao dịch</th>
                                            <th className="p-6 font-bold">Hoạt động</th>
                                            <th className="p-6 font-bold">Số tiền</th>
                                            <th className="p-6 font-bold">Trạng thái</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-gray-100">
                                        {transactions.map((tx) => (
                                            <tr key={tx.id} className="hover:bg-gray-50/50 transition">
                                                <td className="p-6 text-sm">{tx.date}</td>
                                                <td className="p-6 font-medium flex items-center gap-3">
                                                    <div className={`p-2 rounded-lg ${tx.isPositive ? 'bg-green-100 text-[#006b0a]' : 'bg-red-100 text-red-600'}`}>
                                                        {tx.isPositive ? <Plus size={16} /> : <CreditCard size={16} />}
                                                    </div>
                                                    {tx.type}
                                                </td>
                                                <td className={`p-6 font-black ${tx.isPositive ? 'text-[#006b0a]' : 'text-[#2c2f2e]'}`}>
                                                    {tx.isPositive ? '+' : ''}{tx.amount.toLocaleString()}đ
                                                </td>
                                                <td className="p-6">
                                                    <span className={`px-3 py-1 rounded-md text-[10px] font-black uppercase tracking-widest ${tx.status === 'THÀNH CÔNG' ? 'bg-[#59ee50]/30 text-[#005406]' : 'bg-gray-200 text-gray-600'
                                                        }`}>
                                                        {tx.status}
                                                    </span>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    {/* Cột Phải: Thông tin & Ngân hàng */}
                    <div className="lg:col-span-4 space-y-6">
                        {/* Thông tin cá nhân */}
                        <div className="bg-white p-6 rounded-3xl shadow-sm border border-gray-100">
                            <h4 className="font-bold text-lg mb-6">Thông tin cá nhân</h4>
                            <div className="space-y-6">
                                <div className="flex items-start gap-4">
                                    <Mail className="text-gray-400 mt-1" size={20} />
                                    <div>
                                        <p className="text-xs font-bold text-gray-400 uppercase">Email</p>
                                        <p className="font-medium text-sm">duc.tran@example.com</p>
                                    </div>
                                </div>
                                <div className="flex items-start gap-4">
                                    <Phone className="text-gray-400 mt-1" size={20} />
                                    <div>
                                        <p className="text-xs font-bold text-gray-400 uppercase">Số điện thoại</p>
                                        <p className="font-medium text-sm">+84 905 123 456</p>
                                    </div>
                                </div>
                                <div className="flex items-start gap-4">
                                    <MapPin className="text-gray-400 mt-1" size={20} />
                                    <div>
                                        <p className="text-xs font-bold text-gray-400 uppercase">Khu vực</p>
                                        <p className="font-medium text-sm">Hải Châu, Đà Nẵng</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Ngân hàng liên kết */}
                        <div className="bg-white p-6 rounded-3xl shadow-sm border border-gray-100">
                            <div className="flex justify-between items-center mb-6">
                                <h4 className="font-bold text-lg">Ngân hàng liên kết</h4>
                                <button className="w-8 h-8 rounded-full bg-[#59ee50] text-[#005406] flex items-center justify-center hover:scale-105 transition">
                                    <Plus size={18} />
                                </button>
                            </div>
                            <div className="space-y-3">
                                <div className="bg-gray-50 p-4 rounded-xl flex items-center gap-4 border border-gray-100">
                                    <div className="w-12 h-8 bg-[#005406] rounded flex items-center justify-center text-white font-black text-xs italic">VCB</div>
                                    <div>
                                        <p className="font-bold text-sm">Vietcombank</p>
                                        <p className="text-xs text-gray-500 font-mono">**** **** **** 8829</p>
                                    </div>
                                </div>
                                <div className="bg-gray-50 p-4 rounded-xl flex items-center gap-4 border border-gray-100">
                                    <div className="w-12 h-8 bg-red-600 rounded flex items-center justify-center text-white font-black text-xs italic">TCB</div>
                                    <div>
                                        <p className="font-bold text-sm">Techcombank</p>
                                        <p className="text-xs text-gray-500 font-mono">**** **** **** 4102</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Banner Mời bạn bè */}
                        <div className="bg-[#2c2f2e] text-white p-8 rounded-3xl relative overflow-hidden">
                            <div className="relative z-10">
                                <h4 className="text-2xl font-black mb-2">Mời bạn bè,<br />Nhận 50k vào ví</h4>
                                <button className="mt-4 flex items-center gap-2 bg-[#006b0a] px-5 py-2.5 rounded-lg font-bold hover:bg-[#59ee50] hover:text-[#005406] transition text-sm">
                                    <Share2 size={16} /> Chia sẻ ngay
                                </button>
                            </div>
                            <Gift className="absolute -right-8 -bottom-8 text-white opacity-10" size={150} strokeWidth={1} />
                        </div>
                    </div>
                </div>
            </main>
            <Footer />
        </div>
    );
};

export default ViDienTuPage;