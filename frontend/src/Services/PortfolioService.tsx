
import axios from "axios";
import { PortfolioPost, PortfolioGet, PortfolioTotalValueGet, PortfolioDailyChangeGet } from "../Models/Portfolio";
import { AssetTableValueGet } from "../Models/Asset";
import { toast } from "react-toastify";

const api = "http://localhost:5066/api/portfolios/";

export const getPortfolioTotalValueAPI = async (portfolioId: number) => {
  try {
    const response = await axios.get<PortfolioTotalValueGet>(`${api}${portfolioId}/total-value`);
    return response.data;
  } catch (error) {
    console.error("Error in receiving total value", error);
    return null;
  }
};

export const getPortfolioDailyChangeAPI = async (portfolioId: number) => {
  try {
    const response = await axios.get<PortfolioDailyChangeGet>(`${api}${portfolioId}/daily-change`);
    return response.data;
  } catch (error) {
    console.error("Error in receiving daily change", error);
    return null;
  }
};

export const getAllPortfoliosAPI = async () => {
  try {
    const response = await axios.get<PortfolioGet[]>(api); 
    return response.data;
  } catch (error) {
    console.error("Error when receiving a portfolio:", error);
    return [];
  }
};

export const createPortfolioAPI = async (name: string) => {
  try {
    const response = await axios.post(api, { name });
    toast.success(`Portfolio "${name}" created successfully!`); 
    return response.data;
  } catch (error) { 
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 409) {
        toast.error("Portfolio with this name already exists."); 
      } else {
        toast.error("Something went wrong. Please try again.");   
      }
    }
    return null;
  }
};


export const updatePortfolioNameAPI = async (portfolioId: number, newPortfolioName: string) => {
  try {
    const response = await axios.patch(`${api}${portfolioId}`, { Name: newPortfolioName  });
    return response.data;  
  } catch (error) {
    toast.error("Error in updating a portfolio:");
    throw error;
  }
};





export const deletePortfolioAPI = async (portfolioId: number) => {
  try {
    const data = await axios.delete(`${api}${portfolioId}`);
    return data;
  } catch (error) {
  }
};


export const portfolioGetByIdAPI = async (portfolioId: number) => {
  try {
    const response = await axios.get<PortfolioGet>(`${api}${portfolioId}`);
    return response.data;
  } catch (error) {
    console.error("Error in receiving portfolio", error);
    return null;
  }
};

