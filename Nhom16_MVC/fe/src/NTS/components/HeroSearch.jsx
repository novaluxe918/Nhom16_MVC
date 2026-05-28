import { useState, useEffect } from 'react';
import { Search, Calendar, MapPin, Users, Clock } from 'lucide-react';
import axios from 'axios';

const HeroSearch = ({ onSearch }) => {
    const [tenSan, setTenSan] = useState('');
    const [ngay, setNgay] = useState(''); 
    const [gioTu, setGioTu] = useState('');
    const [gioDen, setGioDen] = useState('');
    const [quan, setQuan] = useState('');
    const [loaiSan, setLoaiSan] = useState('');

    const [danhSachQuan, setDanhSachQuan] = useState([]);
    const [danhSachLoaiSan, setDanhSachLoaiSan] = useState([]);

    useEffect(() => {
        const fetchDropdownData = async () => {
            try {
                const [resQuan, resLoai] = await Promise.all([
                    axios.get('https://localhost:7295/api/SanBong/danh-sach-quan'),
                    axios.get('https://localhost:7295/api/SanBong/danh-sach-loai-san')
                ]);
                if (resQuan.data.success) setDanhSachQuan(resQuan.data.data);
                if (resLoai.data.success) setDanhSachLoaiSan(resLoai.data.data);
            } catch (error) {
                console.error("Lỗi tải data bộ lọc:", error);
            }
        };
        fetchDropdownData();
    }, []);

    const handleTriggerSearch = () => {
        // 1. Gửi dữ liệu lên component cha (HomePage) để gọi API
        onSearch({
            tenSan: tenSan,
            ngay: ngay,
            gioTu: gioTu,
            gioDen: gioDen,
            quan: quan,
            maLoaiSan: loaiSan === "" ? null : parseInt(loaiSan)
        });

        // 2. YÊU CẦU CỦA BẠN: Reset toàn bộ form về trạng thái trống
        setTenSan('');
        setNgay('');
        setGioTu('');
        setGioDen('');
        setQuan('');
        setLoaiSan('');
    };

    return (
        <div className="relative bg-[#006b0a] rounded-3xl mx-10 mt-8 mb-20 px-12 py-20 flex flex-col justify-center overflow-visible shadow-lg">
            <h1 className="text-white text-5xl font-extrabold mb-6 leading-tight drop-shadow-md">
                Tìm Sân Bóng <br /><span className="text-[#59ee50]">Đà Nẵng Của Bạn</span>
            </h1>

            {/* Search Bar - Floating */}
            <div className="absolute -bottom-10 left-12 right-12 bg-white rounded-full shadow-2xl flex items-center p-2 border border-gray-100">

                {/* Tên sân */}
                <div className="flex items-center flex-1 px-5 border-r border-gray-200">
                    <Search className="text-[#abaeac] mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-[#abaeac] uppercase">Sân Bóng</span>
                        <input type="text" placeholder="Tên sân..." className="outline-none text-sm font-bold text-[#2c2f2e] placeholder-gray-300" value={tenSan} onChange={e => setTenSan(e.target.value)} />
                    </div>
                </div>

                {/* Thời gian */}
                <div className="flex items-center flex-[1.5] px-5 border-r border-gray-200">
                    <Calendar className="text-[#abaeac] mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-[#abaeac] uppercase">Thời Gian Đá</span>
                        <div className="flex items-center text-sm font-bold text-[#2c2f2e] gap-2">
                            <input type="date" className="outline-none cursor-pointer" value={ngay} onChange={e => setNgay(e.target.value)} />
                            <Clock className="text-[#abaeac]" size={14} />
                            <input type="time" className="outline-none cursor-pointer" value={gioTu} onChange={e => setGioTu(e.target.value)} />
                            <span>-</span>
                            <input type="time" className="outline-none cursor-pointer" value={gioDen} onChange={e => setGioDen(e.target.value)} />
                        </div>
                    </div>
                </div>

                {/* Khu vực */}
                <div className="flex items-center flex-1 px-5 border-r border-gray-200">
                    <MapPin className="text-[#abaeac] mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-[#abaeac] uppercase">Khu Vực</span>
                        <select value={quan} onChange={e => setQuan(e.target.value)} className="outline-none text-sm font-bold text-[#2c2f2e] cursor-pointer appearance-none bg-transparent">
                            <option value="">Tất cả Quận</option>
                            {danhSachQuan.map((q, index) => (
                                <option key={index} value={q.quan}>
                                    {q.quan}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Loại sân */}
                <div className="flex items-center flex-1 px-5">
                    <Users className="text-[#abaeac] mr-3" size={20} />
                    <div className="flex flex-col w-full">
                        <span className="text-[10px] font-bold text-[#abaeac] uppercase">Loại Sân</span>
                        <select value={loaiSan} onChange={e => setLoaiSan(e.target.value)} className="outline-none text-sm font-bold text-[#2c2f2e] cursor-pointer appearance-none bg-transparent">
                            <option value="">Tất cả</option>
                            {danhSachLoaiSan.map(l => <option key={l.maLoaiSan} value={l.maLoaiSan}>{l.tenLoaiSan}</option>)}
                        </select>
                    </div>
                </div>

                <button onClick={handleTriggerSearch} className="bg-[#006b0a] text-white rounded-full hover:bg-[#59ee50] hover:text-[#006b0a] transition-all flex items-center justify-center h-14 w-14 shrink-0 shadow-md">
                    <Search size={22} strokeWidth={3} />
                </button>
            </div>
        </div>
    );
};

export default HeroSearch;