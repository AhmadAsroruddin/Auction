import React from 'react';
import { AiOutlineCar } from 'react-icons/ai';

const Navbar = () => {
  return (
    <header className="sticky top-0 z-50 flex justify-between items-center p-5 bg-white text-gray-800 shadow-md">
      <div className='flex items-center gap-2 text-3xl font-medium text-orange-600'>
        <AiOutlineCar size={34} />
        <div>Car Auction</div>
      </div>
      <div>Search</div>
      <div>Login</div>
    </header>
  );
};

export default Navbar;
