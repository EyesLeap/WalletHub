import React from 'react';
import { Link } from 'react-router-dom';

const NotFoundPage = () => {
  return (
    <div className="flex flex-col items-center justify-center h-[calc(100vh-64px)] text-center p-4">
      <h1 className="text-6xl font-bold mb-4 text-white">404</h1>
      <p className="text-xl text-gray-400 mb-6">Page not found</p>
      <Link to="/" className="text-green-500 hover:underline text-lg">
        Back to main page
      </Link>
    </div>
  );
};

export default NotFoundPage;
