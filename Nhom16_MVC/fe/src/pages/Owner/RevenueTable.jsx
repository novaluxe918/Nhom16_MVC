const RevenueTable = ({ data }) => {

    return (
        <div className="bg-white p-5 rounded-2xl shadow overflow-auto">

            <h2 className="text-2xl font-bold mb-5">
                Danh sách doanh thu
            </h2>

            <table className="w-full">

                <thead>
                    <tr className="border-b">

                        <th className="py-3 text-left">
                            Mã sân
                        </th>

                        <th className="py-3 text-left">
                            Tên sân
                        </th>

                        <th className="py-3 text-left">
                            Lượt đặt
                        </th>

                        <th className="py-3 text-left">
                            Doanh thu
                        </th>

                    </tr>
                </thead>

                <tbody>

                    {data.map(item => (

                        <tr
                            key={item.maSanBong}
                            className="border-b hover:bg-gray-50"
                        >

                            <td className="py-4">
                                {item.maSanBong}
                            </td>

                            <td className="py-4">
                                {item.tenSan}
                            </td>

                            <td className="py-4">
                                {item.tongLuotDat}
                            </td>

                            <td className="py-4 font-bold text-green-600">
                                {item.tongDoanhThu
                                    .toLocaleString()}đ
                            </td>

                        </tr>

                    ))}

                </tbody>

            </table>

        </div>
    );
};

export default RevenueTable;