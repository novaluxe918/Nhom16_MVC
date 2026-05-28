// File: src/components/SearchFilter.jsx
import { useState, useEffect } from 'react';
import { Search, Calendar, MapPin, Users } from 'lucide-react';
import axios from 'axios';

const SearchFilter = ({ onSearch }) => {
    const [tenSan, setTenSan] = useState('');
    const [ngay, setNgay] = useState(new Date().toISOString().split('T')[0]); // Mặc định ngày hôm nay
    const [gioTu, setGioTu] = useState('16:00');
    const [quan, setQuan] = useState('');
    const [loaiSan, setLoaiSan] = useState('');

    // State chứa data gọi từ Backend lên
    const [danhSachQuan, setDanhSachQuan] = useState([]);
    const [danhSachLoaiSan, setDanhSachLoaiSan] = useState([]);

    // Gọi API lấy dữ liệu Combobox khi vừa vào trang
    useEffect(() => {
        const loadComboboxData = async () => {
            try {
                // LƯU Ý: Đổi 7142 thành Cổng (Port) Backend của bạn
                const resQuan = await axios.get('https://localhost:7295/api/SanBong/danh-sach-quan');
                const resLoaiSan = await axios.get('https://localhost:7295/api/SanBong/danh-sach-loai-san');

                if (resQuan.data.success) setDanhSachQuan(resQuan.data.data);
                if (resLoaiSan.data.success) setDanhSachLoaiSan(resLoaiSan.data.data);
            } catch (error) {
                console.error("Lỗi lấy dữ liệu Combobox:", error);
            }
        };
        loadComboboxData();
    }, []);

    const handleTimKiem = () => {
        const gioBatDau = parseInt(gioTu.split(':')[0]);
        const gioDen = `${String(gioBatDau + 2).padStart(2, '0')}:00`;
        // Gom dữ liệu ném lên cho HomePage gọi API tìm kiếm
        onSearch({
            tenSan: tenSan,
            ngay: ngay,
            gioTu: gioTu,
            gioDen: gioDen,
            quan: quan,
            maLoaiSan: loaiSan === "" ? null : parseInt(loaiSan)
        });
    };

    return (
        <div className="relative bg-green-700 rounded-3xl mx-8 mt-6 mb-16 px-12 py-16 flex flex-col justify-center"
            style={{ backgroundImage: "linear-gradient(to right, #047857, #15803d)" }}>
            <h1 className="text-white text-5xl font-extrabold mb-4 leading-tight">Tìm Sân Bóng <br />Đà Nẵng Của Bạn</h1>

            {/* Khối nổi (Floating Pill) */}
            <div className="absolute -bottom-8 left-12 right-12 bg-white rounded-full shadow-2xl flex items-center p-2 border border-gray-100">

                <div className="flex items-center flex-1 px-4 border-r">
                    <Search className="text-gray-400 mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-gray-400 uppercase">Sân Mẹ</span>
                        <input type="text" placeholder="Nhập tên sân..." className="outline-none text-sm font-semibold text-gray-800" value={tenSan} onChange={(e) => setTenSan(e.target.value)} />
                    </div>
                </div>

                <div className="flex items-center flex-1 px-4 border-r">
                    <Calendar className="text-gray-400 mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-gray-400 uppercase">Thời Gian Đá</span>
                        <div className="flex items-center text-sm font-semibold text-gray-800">
                            <input type="date" className="outline-none w-28 cursor-pointer" value={ngay} onChange={(e) => setNgay(e.target.value)} />
                            <span className="mx-1">-</span>
                            <input type="time" className="outline-none cursor-pointer" value={gioTu} onChange={(e) => setGioTu(e.target.value)} />
                        </div>
                    </div>
                </div>

                {/* Nút đổ dữ liệu Tỉnh/Thành phố thật từ Backend */}
                <div className="flex items-center flex-1 px-4 border-r">
                    <MapPin className="text-gray-400 mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-gray-400 uppercase">Khu Vực</span>
                        <select value={quan} onChange={(e) => setQuan(e.target.value)} className="outline-none text-sm font-semibold text-gray-800 cursor-pointer appearance-none bg-transparent">
                            <option value="">Tất cả Quận</option>
                            {danhSachQuan?.map((q, idx) => <option key={idx} value={q}>{q}</option>)}
                        </select>
                    </div>
                </div>

                {/* Nút đổ dữ liệu Loại sân thật từ Backend */}
                <div className="flex items-center flex-1 px-4">
                    <Users className="text-gray-400 mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-gray-400 uppercase">Loại Sân</span>
                        <select value={loaiSan} onChange={(e) => setLoaiSan(e.target.value)} className="outline-none text-sm font-semibold text-gray-800 cursor-pointer appearance-none bg-transparent">
                            <option value="">Tất cả</option>
                            {danhSachLoaiSan?.map((l) => <option key={l.maLoaiSan} value={l.maLoaiSan}>{l.tenLoaiSan}</option>)}
                        </select>
                    </div>
                </div>

                <button onClick={handleTimKiem} className="bg-green-800 text-white p-4 rounded-full hover:bg-green-900 transition mr-1 flex items-center justify-center h-12 w-12 shrink-0">
                    <Search size={20} strokeWidth={3} />
                </button>
            </div>
        </div>
    );
};

export default SearchFilter;