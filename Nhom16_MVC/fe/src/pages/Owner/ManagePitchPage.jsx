import React, { useEffect, useState } from 'react';
import { getAllSanBong } from '../../services/sanBongService';


function ManagePitchPage(props) {
    const [dsSan, setDsSan] = useState([]);
    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
    try {
        const res = await getAllSanBong();

        console.log("FULL RESPONSE:", res);
        console.log("DATA:", res.data);

        setDsSan(res.data);
    } catch (error) {
        console.log("Lỗi load dữ liệu:", error);
    }
};
    return (
        <div style={{ padding: "20px" }}>
            <h2>Danh sách sân bóng</h2>

            {dsSan.length === 0 ? (
                <p>Không có dữ liệu</p>
            ) : (
                dsSan.map((san) => (
                    <div
                        key={san.maSanBong}
                        style={{
                            border: "1px solid #ccc",
                            padding: "10px",
                            marginBottom: "10px",
                            borderRadius: "8px"
                        }}
                    >
                        <h3>{san.tenSan}</h3>
                        <p>📍 {san.diaChi}</p>
                    </div>
                ))
            )}
        </div>
    );
}

export default ManagePitchPage;