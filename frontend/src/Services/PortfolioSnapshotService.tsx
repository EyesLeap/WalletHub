import axios from "axios";
import { AssetGet } from "../Models/Asset";
import { PortfolioSnapshotGet, PortfolioSnapshotRange } from "../Models/PortfolioSnapshot";

const api = "http://localhost:5066/api/portfolio-snapshots/";




export const getPortfolioSnapshotsAPI = 
  async (portfolioId: number | null, range: PortfolioSnapshotRange) 
  : Promise<PortfolioSnapshotGet[]> =>  {
  try {
    const response = await axios.get(`${api}${portfolioId}`, {
       params: { snapshotsRange: range }  
      });
    return response.data;  
  } catch (error) {
    console.error("Ошибка получения снимков:", error);
    throw error;
  }
};