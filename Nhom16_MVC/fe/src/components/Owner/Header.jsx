import React from 'react';

function Header(props) {
  return (
    <div>
       <header className="flex justify-between items-center p-6 sticky top-0 bg-white shadow">

      <h1 className="text-2xl font-bold">
        Quản lý hệ thống sân
      </h1>

      <input
        type="text"
        placeholder="Tìm kiếm sân..."
        className="border rounded-full px-4 py-2"
      />

    </header>
    </div>
  );
}

export default Header;