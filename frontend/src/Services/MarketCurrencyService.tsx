import axios from "axios";
import { AssetGet } from "../Models/Asset";
import { MarketCurrencyGet } from "../Models/MarketCurrency";

const api = "http://localhost:5066/api/currency";

export const getPopularCurrenciesAPI = async (): Promise<MarketCurrencyGet[]> => {
  try {
    const response = await axios.get<MarketCurrencyGet[]>(`${api}/popular`);
    return response.data;
  } catch (error) {
    console.error("Error when receiving currencies:", error);
    throw error;
  }
};
