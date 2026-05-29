import axios from "axios";

const API = "http://localhost:5014/api/datsan";

export const getLichDat = async (chuSanId, filter) => {
    const params = {
        ngay: filter?.ngay,
        trangThai: filter?.trangThai,
        sanConId: filter?.sanConId,
    };

    return await axios.get(`${API}/chusan/${chuSanId}`, { params });
};