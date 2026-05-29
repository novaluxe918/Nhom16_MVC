import { Outlet } from "react-router-dom";
import Sidebar from "../components/Owner/Sidebar";
import Header from "../components/Owner/Header";

const Layout = () => {
    return (
        <div className="flex">
            <Sidebar />

            <div className="ml-72 flex-1">
                <Header />

                <main className="bg-gray-50 min-h-screen">
                    <Outlet />
                </main>
            </div>
        </div>
    );
};

export default Layout;