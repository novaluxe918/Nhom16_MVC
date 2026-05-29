import { useEffect, useState } from "react";
import { getAllSanBong } from "../../services/sanBongService";
import Sidebar from "../../components/Owner/Sidebar";
import Header from "../../components/Owner/Header";
import StatsCard from "../../components/Owner/StatsCard";
import { useNavigate } from "react-router-dom";
import SanBongCard from "../../components/Owner/SanBongCard";


const QuanLySanPage = () => {
    const [sanBongs, setSanBongs] = useState([]);
    const navigate  = useNavigate();
    useEffect(() => {
        fetchSanBong();
    }, []);

    const fetchSanBong = async () => {
        try {
            const response = await getAllSanBong();
            setSanBongs(response.data);
        } catch (error) {
            console.log(error);
        }
    };

    return (
        <div className="bg-gray-100 min-h-screen">
            <Sidebar />

            <main className="pl-72">

                <div className="p-12 space-y-10">
                    <section className="flex justify-between items-end">
                        <div>
                            <p className="uppercase text-green-600 font-bold tracking-widest">
                                Hệ thống sân vận động
                            </p>

                            <h1 className="text-5xl font-black mt-2">
                                Quản lý hệ thống sân bóng
                            </h1>
                        </div>

                        <button onClick={() => navigate("/them-san")} className="bg-gradient-to-r from-green-700 to-green-400 text-white px-8 py-4 rounded-xl font-bold">
                            + Thêm sân con mới
                        </button>
                    </section>

                    <section className="grid grid-cols-1 md:grid-cols-3 gap-6">
                        <StatsCard
                            title="Tổng cộng"
                            value={sanBongs.length}
                            description="Tổng số sân bóng"
                            color="text-black"
                        />

                        <StatsCard
                            title="Đã duyệt"
                            value={
                                sanBongs.filter(
                                    (item) => item.daduyet === true
                                ).length
                            }
                            description="Sân đang hoạt động"
                            color="text-green-600"
                        />

                        <StatsCard
                            title="Chưa duyệt"
                            value={
                                sanBongs.filter(
                                    (item) => item.daduyet === false
                                ).length
                            }
                            description="Sân chờ xét duyệt"
                            color="text-red-500"
                        />
                    </section>

                    <section className="space-y-6">
                        <h3 className="text-3xl font-bold">
                            Danh sách sân bóng
                        </h3>

                        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                            {sanBongs.map((item) => (
                                <SanBongCard
                                    key={item.masanbong}
                                    item={item}
                                />
                            ))}
                        </div>
                    </section>
                </div>
            </main>
        </div>
    );
};

export default QuanLySanPage;