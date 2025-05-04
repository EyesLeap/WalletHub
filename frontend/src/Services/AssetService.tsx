import axios from "axios";
import { AssetGet } from "../Models/Asset";

const api = process.env.REACT_APP_API_URL + "assets/";

export const getAssetsByIdAPI = async (portfolioId: number): Promise<AssetGet[]> => {
  try {
    const response = await axios.get<AssetGet[]>(`${api}${portfolioId}`);
    return response.data;
  } catch (error) {
    console.error("Error when receiving currencies:", error);
    throw error;
  }
};
