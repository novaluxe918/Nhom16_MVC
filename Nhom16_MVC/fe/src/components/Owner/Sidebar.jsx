import { NavLink } from "react-router-dom";

function Sidebar() {
  const navItems = [
    { label: "Bảng điều khiển", to: "/dashboard" },
    { label: "Đơn đặt sân", to: "/don-dat-san" },
    { label: "Quản lý sân", to: "/quan-ly-san" },
    { label: "Bảng giá", to: "/bang-gia" },
  ];

  return (
    <aside className="h-screen w-72 fixed left-0 top-0 bg-white p-6 flex flex-col">

      <div className="mb-10">
        <h1 className="text-3xl font-black text-green-700">
          Đà Nẵng Stadium
        </h1>
        <p className="uppercase tracking-widest text-gray-500">
          Quản trị viên
        </p>
      </div>

      <nav className="flex flex-col gap-3">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `p-4 rounded-xl text-left transition-colors ${
                isActive
                  ? "bg-green-200 font-bold text-green-800"
                  : "hover:bg-gray-100 text-gray-700"
              }`
            }
          >
            {item.label}
          </NavLink>
        ))}
      </nav>

      <NavLink
        to="/them-san"
        className="mt-auto bg-green-700 text-white py-4 rounded-xl text-center font-bold hover:bg-green-800 transition-colors"
      >
        Thêm sân mới
      </NavLink>

    </aside>
  );
}

export default Sidebar;