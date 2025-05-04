
export type AssetGet = {
  id: number;
  symbol: string;
  name: string;
  portfolioId: number;
  amount: number;
  averagePurchasePrice: number;
};

export type AssetPost = {
  portfolioId: number;
  amount: number;
  averagePurchasePrice: number;
}; 

export type AssetTableValueGet = {
  symbol: string;
  name: string;
  price: number;  
  amount: number;
  totalValue: number;
  averagePurchasePrice: number;
  percentChange1h: number;
  percentChange24h: number;
  percentChange7d: number;
  profitLoss:number;
};