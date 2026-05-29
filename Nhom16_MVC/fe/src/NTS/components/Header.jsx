import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ShoppingCart, Bell, User, Wallet, LogOut } from 'lucide-react';

const Header = () => {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <header className="flex justify-between items-center px-10 py-4 bg-white border-b border-gray-200 sticky top-0 z-50">
            {/* Logo */}
            <Link to="/" className="font-extrabold text-2xl text-[#006b0a] tracking-tight">
                SPORT ĐÀ NẴNG
            </Link>

            {/* Menu chính */}
            <nav className="hidden md:flex gap-8 text-sm font-semibold text-[#abaeac]">
                <Link to="/" className="text-[#006b0a] border-b-2 border-[#006b0a] pb-1">Sân bóng</Link>
                <Link to="/gioi-thieu" className="hover:text-[#006b0a] transition-colors">Giới thiệu</Link>
                <Link to="/ho-tro" className="hover:text-[#006b0a] transition-colors">Hỗ trợ</Link>
            </nav>

            {/* Cụm Tiện ích & Avatar */}
            <div className="flex items-center gap-6">
                <Link to="/lich-su-dat-san" title="Lịch sử đặt sân">
                    <ShoppingCart className="text-[#2c2f2e] hover:text-[#006b0a] cursor-pointer transition-colors hover:scale-110" size={22} />
                </Link>
                <Bell className="text-[#2c2f2e] hover:text-[#006b0a] cursor-pointer transition-colors" size={22} />

                <div className="relative">
                    <button
                        onClick={() => setIsMenuOpen(!isMenuOpen)}
                        className="flex items-center focus:outline-none"
                    >
                        <img
                            src="https://ui-avatars.com/api/?name=User&background=006b0a&color=fff"
                            alt="Avatar"
                            className="w-10 h-10 rounded-full border-2 border-[#59ee50] object-cover"
                        />
                    </button>

                    {isMenuOpen && (
                        <div className="absolute right-0 mt-3 w-48 bg-white border border-gray-100 rounded-xl shadow-lg overflow-hidden">
                            <Link to="/profile" className="flex items-center gap-3 p-3 hover:bg-[#f5f7f5] text-sm font-medium text-[#2c2f2e]">
                                <User size={16} className="text-[#006b0a]" /> Hồ sơ cá nhân
                            </Link>
                            <Link to="/wallet" className="flex items-center gap-3 p-3 hover:bg-[#f5f7f5] text-sm font-medium text-[#2c2f2e]">
                                <Wallet size={16} className="text-[#006b0a]" /> Ví điện tử
                            </Link>
                            <div className="border-t border-gray-100"></div>
                            <button
                                onClick={handleLogout}
                                className="flex items-center gap-3 p-3 w-full text-left hover:bg-red-50 text-sm font-medium text-red-600"
                            >
                                <LogOut size={16} /> Đăng xuất
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Header;