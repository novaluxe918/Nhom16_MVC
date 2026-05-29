import React from 'react';

function Sidebar(props) {
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

        <button className="p-4 rounded-xl text-left hover:bg-gray-100">
          Bảng điều khiển
        </button>

        <button className="p-4 rounded-xl text-left hover:bg-gray-100">
          Đơn đặt sân
        </button>

        <button className="p-4 rounded-xl bg-green-200 font-bold">
          Quản lý sân
        </button>

        <button className="p-4 rounded-xl hover:bg-gray-100">
          Bảng giá
        </button>

      </nav>

      <button className="mt-auto bg-green-700 text-white py-4 rounded-xl">
        Thêm sân mới
      </button>

    </aside>
  );
}

export default Sidebar;