import { AssetGet, AssetTableValueGet } from "./Asset";
import { TransactionGet } from "./Transaction";

export type PortfolioGet = {
  id: number;
  name: string;
  appUserId: string;
  totalValueUSD: number;
  createdAt: string; 
  assets: AssetGet[]; 
  transactions: TransactionGet[];
};

export type PortfolioPost = {
  name: string;
};

export type PortfolioTotalValueGet = {
  totalValueUSD: number;
  assetValues: AssetTableValueGet[];
};

export type PortfolioDailyChangeGet = {
  profitLoss: number;
  percentChange24h: number;
};