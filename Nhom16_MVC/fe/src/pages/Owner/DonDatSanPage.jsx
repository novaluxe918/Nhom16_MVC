import { useEffect, useState } from "react";
import { getLichDat } from "../../services/datSanApi";
import Sidebar from "../../components/Owner/Sidebar";
import DonDatSanFilter from "./DonDatSanFilter";
import DonDatSanTable from "./DonDatSanTable";



const DonDatSanPage = () => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

    // 🔥 FIX FILTER CHO KHỚP BACKEND
    const [filter, setFilter] = useState({
        tuNgay: "",
        denNgay: "",
        trangThai: "",
        maSanChiTiet: ""
    });

    const chuSanId = 1;

    const fetchData = async () => {
        setLoading(true);

        try {
            const res = await getLichDat(chuSanId, filter);

            console.log("DATA:", res.data);

            setData(res.data ?? []);
        } catch (err) {
            console.log("API ERROR:", err);
        }

        setLoading(false);
    };

   
    useEffect(() => {
        const timer = setTimeout(() => {
            fetchData();
        }, 400);

        return () => clearTimeout(timer);
    }, [filter]);

    return (
        <div className="flex">
            <Sidebar />

            <div className="ml-72 p-6 w-full">
                <h1 className="text-2xl font-bold mb-4">
                    Quản lý đơn đặt sân
                </h1>

                {/* FILTER */}
                <DonDatSanFilter
                    filter={filter}
                    setFilter={setFilter}
                />

                {/* TABLE */}
                {loading ? (
                    <p className="text-gray-500">Đang tải dữ liệu...</p>
                ) : (
                    <DonDatSanTable data={data} />
                )}
            </div>
        </div>
    );
};

export default DonDatSanPage;