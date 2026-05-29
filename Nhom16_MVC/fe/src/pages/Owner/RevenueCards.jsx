const RevenueCards = ({ data }) => {

    const tongDoanhThu =
        data.reduce(
            (sum, item) =>
                sum + item.tongDoanhThu,
            0
        );

    const tongLuotDat =
        data.reduce(
            (sum, item) =>
                sum + item.tongLuotDat,
            0
        );

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-5">

            <div className="bg-white p-5 rounded-2xl shadow">
                <p className="text-gray-500">
                    Tổng doanh thu
                </p>

                <h2 className="text-3xl font-bold mt-2">
                    {tongDoanhThu.toLocaleString()}đ
                </h2>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow">
                <p className="text-gray-500">
                    Tổng lượt đặt
                </p>

                <h2 className="text-3xl font-bold mt-2">
                    {tongLuotDat}
                </h2>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow">
                <p className="text-gray-500">
                    Tổng số sân
                </p>

                <h2 className="text-3xl font-bold mt-2">
                    {data.length}
                </h2>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow">
                <p className="text-gray-500">
                    Hiệu suất
                </p>

                <h2 className="text-3xl font-bold mt-2">
                    72%
                </h2>
            </div>

        </div>
    );
};

export default RevenueCards;