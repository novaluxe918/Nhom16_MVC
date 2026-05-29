import axios from "axios";

const API = "http://localhost:5014/api/report";

export const getRevenueReport = async (chuSanId) => {
    const response = await axios.get(
        `${API}/revenue/${chuSanId}`
    );

    return response.data;
};