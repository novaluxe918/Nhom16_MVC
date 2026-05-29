import { BrowserRouter, Routes, Route } from "react-router-dom";
import DonDatSanPage from "./pages/Owner/DonDatSanPage";
import ThemSanBong from "./pages/Owner/ThemSanBong";
import QuanLySanPage from "./pages/Owner/QuanLySanPage";
import BaoCaoPage from "./pages/Owner/BaoCaoPage";



function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/dashboard" element={<BaoCaoPage />} />

        <Route path="/quan-ly-san" element={<QuanLySanPage />} />
        <Route path="/them-san" element={<ThemSanBong />} />
        {/* ✔ QUAN TRỌNG */}
        <Route path="/don-dat-san" element={<DonDatSanPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;