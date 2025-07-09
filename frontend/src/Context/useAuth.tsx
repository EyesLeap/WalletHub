import { createContext, useEffect, useState } from "react";
import { UserProfile } from "../Models/User";
import { useNavigate } from "react-router-dom";
import { loginAPI, registerAPI } from "../Services/AuthService";
import { toast } from "react-toastify";
import React from "react";
import axios from "axios";

type GoogleOAuthDto = {
  IdToken: string;
  AccessToken?: string;
};

type UserContextType = {
  user: UserProfile | null;
  token: string | null;
  registerUser: (email: string, username: string, password: string) => void;
  loginUser: (username: string, password: string) => void;
  loginWithGoogle: (googleData: GoogleOAuthDto) => Promise<void>;
  logout: () => void;
  isLoggedIn: () => boolean;
};

type Props = { children: React.ReactNode };

const UserContext = createContext<UserContextType>({} as UserContextType);

export const UserProvider = ({ children }: Props) => {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const user = localStorage.getItem("user");
    const token = localStorage.getItem("token");
    if (user && token) {
      setUser(JSON.parse(user));
      setToken(token);
      axios.defaults.headers.common["Authorization"] = "Bearer " + token;
    }
    setIsReady(true);
  }, []);

  const registerUser = async (
    email: string,
    username: string,
    password: string
  ) => {
    await registerAPI(email, username, password)
      .then((res) => {
        if (res) {
          localStorage.setItem("token", res?.data.token);
          const userObj = {
            userName: res?.data.userName,
            email: res?.data.email,
          };
          localStorage.setItem("user", JSON.stringify(userObj));
          setToken(res?.data.token!);
          setUser(userObj!);
          axios.defaults.headers.common["Authorization"] = "Bearer " + res?.data.token;
          toast.success("Welcome!");
          navigate("/dashboard");
        }
      })
      .catch((e) => toast.warning("Server error occured"));
  };

  const loginUser = async (username: string, password: string) => {
    await loginAPI(username, password)
      .then((res) => {
        if (res) {
          localStorage.setItem("token", res?.data.token);
          const userObj = {
            userName: res?.data.userName,
            email: res?.data.email,
          };
          localStorage.setItem("user", JSON.stringify(userObj));
          setToken(res?.data.token!);
          setUser(userObj!);
          axios.defaults.headers.common["Authorization"] = "Bearer " + res?.data.token;
          toast.success("Login Success!");
          navigate("/dashboard");
        }
      })
      .catch((e) => toast.warning("Server error occured"));
  };

  const loginWithGoogle = async (googleData: GoogleOAuthDto) => {
    try {
      const response = await axios.post(process.env.REACT_APP_API_URL + 'account/google/login', googleData);
      
      if (response.data) {
        localStorage.setItem("token", response.data.token);
        const userObj = {
          userName: response.data.userName,
          email: response.data.email,
        };
        localStorage.setItem("user", JSON.stringify(userObj));
        setToken(response.data.token);
        setUser(userObj);
        axios.defaults.headers.common["Authorization"] = "Bearer " + response.data.token;
        toast.success("Google Login Success!");
        navigate("/dashboard");
      }
    } catch (error) {
      console.error('Google login error:', error);
      throw error;
    }
  };

  const isLoggedIn = () => {
    return !!user;
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    delete axios.defaults.headers.common["Authorization"];
    navigate("/");
  };

  return (
    <UserContext.Provider
      value={{ loginUser, user, token, logout, isLoggedIn, registerUser, loginWithGoogle }}
    >
      {isReady ? children : null}
    </UserContext.Provider>
  );
};

export const useAuth = () => React.useContext(UserContext);