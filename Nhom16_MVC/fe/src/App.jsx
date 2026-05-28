import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './NTS/pages/HomePage';
import CumSanPage from './NTS/pages/CumSanPage';
import ChiTietSanPage from './NTS/pages/ChiTietSanPage';
import LichSuDatSanPage from './NTS/pages/LichSuDatSanPage';

function App() {
    return (
        // BrowserRouter: Bọc toàn bộ ứng dụng, khởi động radar theo dõi URL
        <BrowserRouter>
            {/* Routes: Nơi khai báo danh sách các ngã rẽ */}
            <Routes>

                {/* Route: Một ngã rẽ cụ thể. 
            path="/": Đường dẫn gốc (Trang chủ)
            element: Vẽ cái Component nào ra? */}
                <Route path="/" element={<HomePage />} />
                <Route path="/cum-san/:id" element={<CumSanPage />} />
                <Route path="/san-con/:id" element={<ChiTietSanPage />} />
                <Route path="/lich-su-dat-san" element={<LichSuDatSanPage />} />


                {/* Ví dụ các trang bạn sẽ làm tiếp theo */}
                {/* <Route path="/profile" element={<ProfilePage />} /> */}
                {/* <Route path="/san-con/:id" element={<ChiTietSanPage />} /> */}

            </Routes>
        </BrowserRouter>
    );
}

export default App;