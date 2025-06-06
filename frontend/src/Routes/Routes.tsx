import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import HomePage from "../Pages/HomePage/HomePage";
import LoginPage from "../Pages/LoginPage/LoginPage";
import RegisterPage from "../Pages/RegisterPage/RegisterPage";
import ProtectedRoute from "./ProtectedRoute";
import DashboardPage from "../Pages/DashboardPage/DashboardPage";
import NotFoundPage from "../Pages/NotFoundPage/NotFoundPage"; 
import CryptocurrenciesPage from "../Pages/CryptocurrenciesPage/CryptocurrenciesPage";
export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { path: "", element: <HomePage /> },
      { path: "login", element: <LoginPage /> },
      { path: "register", element: <RegisterPage /> },
      {
        path: "dashboard",
        element: (
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "cryptocurrencies",
        element: (
          <ProtectedRoute>
            <CryptocurrenciesPage />
          </ProtectedRoute>
        ),
      },
      { path: "*", element: <NotFoundPage /> }, 
    ],
  },
]);
