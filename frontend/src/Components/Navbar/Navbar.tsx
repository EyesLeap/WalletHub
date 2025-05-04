import React from "react";
import { Link } from "react-router-dom";
import Logo from '../Navbar/wallet-hub-logo6.svg';
import "./Navbar.css";
import { useAuth } from "../../Context/useAuth";

interface Props {}

const Navbar = (props: Props) => {
  const { isLoggedIn, user, logout } = useAuth();
  return (
    <nav
      className="relative w-full p-4 border-2"
      style={{
        background: "var(--gradient-main)",
        borderColor: "var(--color-border)", 
      }}
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-20">
          <Link to="/">
          <img src={Logo} alt="Logo" style={{ height: '3rem' }} />
          </Link>
          <div className="hidden font-bold lg:flex">
            <Link to="/dashboard" className="navbar-link">
              Dashboard
            </Link>
          </div>
          <div className="hidden font-bold lg:flex">
            <Link to="/dashboard" className="navbar-link">
              Cryptocurrencies
            </Link>
          </div>
        </div>
        {isLoggedIn() ? (
          <div className="hidden lg:flex items-center space-x-6 text-white">
            <div className="hover:text-darkBlue">
              Welcome, {user?.userName}
            </div>
            <button
              onClick={logout}
              className="navbar-button py-3 px-6 rounded-md transition"
            >
              Logout
            </button>
          </div>
        ) : (
          <div className="hidden lg:flex items-center space-x-6 text-white">
            <Link to="/login" className="navbar-link">
              Login
            </Link>
            <Link to="/register" className="navbar-button px-8 py-3 font-bold rounded">
              Signup
            </Link>
          </div>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
