import { useState, useEffect } from 'react';
import axios from 'axios';
import Header from '../components/Header';
import HeroSearch from '../components/HeroSearch';
import FeaturedPitches from '../components/FeaturedPitches';
import AiBanner from '../components/AiBanner';
import Footer from '../components/Footer';

const HomePage = () => {
    const [sanBongs, setSanBongs] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    // State quản lý chế độ xem: 'PARENT' hoặc 'CHILD'
    const [viewMode, setViewMode] = useState('PARENT');

    // Hàm gọi API lấy Sân Mẹ
    const fetchSanMe = async (tenSan = "") => {
        setIsLoading(true);
        setViewMode('PARENT');
        try {
            const response = await axios.get(`https://localhost:7295/api/SanBong/danh-sach-san-me?tenSan=${tenSan}`);
            if (response.data.success) {
                setSanBongs(response.data.data);
            }
        } catch (error) {
            console.error("Lỗi tải sân mẹ:", error);
        } finally {
            setIsLoading(false);
        }
    };

    // Hàm gọi API lấy Sân Con rảnh
    const fetchSanConTrong = async (filters) => {
        setIsLoading(true);
        setViewMode('CHILD');
        try {
            const response = await axios.post('https://localhost:7295/api/SanBong/tim-kiem-san-trong', filters);

            if (response.data.success) {
                setSanBongs(response.data.data);
            } else {
                setSanBongs([]);
            }
        } catch (error) {
            console.error("Lỗi tải sân con:", error);
            setSanBongs([]);

            // Bắt lỗi từ Backend C# gửi lên và hiển thị cho User
            if (error.response && error.response.data && error.response.data.message) {
                alert("Lỗi từ hệ thống: " + error.response.data.message);
            } else {
                alert("Có lỗi xảy ra khi gọi API tìm sân. Vui lòng kiểm tra lại hệ thống.");
            }
        } finally {
            setIsLoading(false);
        }
    };

    // Logic phân luồng khi user bấm nút TÌM KIẾM
    const handleTrigerSearch = (filters) => {
        if (filters.ngay || filters.gioTu || filters.maLoaiSan || filters.quan) {
            if (!filters.ngay || !filters.gioTu || !filters.gioDen) {
                alert("Vui lòng chọn đầy đủ Ngày và Khung giờ để tìm sân trống!");
                return;
            }
            fetchSanConTrong(filters);
        } else {
            fetchSanMe(filters.tenSan);
        }
    };

    // Vừa vào trang -> Tự động load danh sách Sân Mẹ
    useEffect(() => {
        const loadInitialData = async () => {
            await fetchSanMe();
        };
        loadInitialData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <div className="min-h-screen flex flex-col bg-[#f5f7f5] font-sans">
            <Header />
            <main className="flex-grow w-full max-w-[1440px] mx-auto">
                <HeroSearch onSearch={handleTrigerSearch} />
                <FeaturedPitches sanBongs={sanBongs} isLoading={isLoading} viewMode={viewMode} />
                <AiBanner />
            </main>
            <Footer />
        </div>
    );
};

export default HomePage;