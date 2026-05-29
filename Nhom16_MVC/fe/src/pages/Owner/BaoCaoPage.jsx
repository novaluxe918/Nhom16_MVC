import { useEffect, useState } from "react";

import { getRevenueReport } from "../../services/reportApi";

import RevenueCards from "./RevenueCards";
import RevenueTable from "./RevenueTable";

import Sidebar from "../../components/Owner/Sidebar";

const BaoCaoPage = () => {

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {

        const fetchData = async () => {

            try {

                setLoading(true);

                const result = await getRevenueReport(6);

                setData(result);

            } catch (error) {

                console.log(error);

            } finally {

                setLoading(false);
            }
        };

        fetchData();

    }, []);

    return (

        <div className="flex min-h-screen bg-[#f4f7f6]">

            {/* Sidebar */}
            <Sidebar />

            {/* Main Content */}
            <div className="flex-1 ml-64 p-8">

                {/* Header */}
                <div className="bg-white rounded-3xl shadow-sm border border-gray-100 p-6 mb-8">

                    <div className="flex items-center justify-between">

                        <div>

                            <h1 className="text-3xl font-bold text-gray-800">
                                Báo Cáo Doanh Thu
                            </h1>

                            <p className="text-gray-500 mt-2">
                                Theo dõi doanh thu và hoạt động sân bóng
                            </p>

                        </div>

                        <div className="hidden md:flex items-center gap-3">

                            <div className="bg-green-100 text-green-700 px-4 py-2 rounded-xl font-medium">
                                Chủ sân
                            </div>

                        </div>

                    </div>

                </div>

                {/* Loading */}
                {loading ? (

                    <div className="bg-white rounded-3xl shadow-sm p-10 text-center">

                        <div className="flex flex-col items-center gap-4">

                            <div className="w-12 h-12 border-4 border-green-500 border-t-transparent rounded-full animate-spin"></div>

                            <p className="text-gray-500 text-lg">
                                Đang tải dữ liệu doanh thu...
                            </p>

                        </div>

                    </div>

                ) : (

                    <div className="space-y-8">

                        {/* Cards */}
                        <div className="bg-white rounded-3xl shadow-sm border border-gray-100 p-6">

                            <div className="mb-5">

                                <h2 className="text-xl font-semibold text-gray-800">
                                    Tổng Quan
                                </h2>

                                <p className="text-gray-500 text-sm mt-1">
                                    Thống kê nhanh doanh thu sân bóng
                                </p>

                            </div>

                            <RevenueCards data={data} />

                        </div>

                        {/* Table */}
                        <div className="bg-white rounded-3xl shadow-sm border border-gray-100 p-6">

                            <div className="mb-5">

                                <h2 className="text-xl font-semibold text-gray-800">
                                    Chi Tiết Doanh Thu
                                </h2>

                                <p className="text-gray-500 text-sm mt-1">
                                    Danh sách giao dịch và thống kê
                                </p>

                            </div>

                            <RevenueTable data={data} />

                        </div>

                    </div>

                )}

            </div>

        </div>
    );
};

export default BaoCaoPage;