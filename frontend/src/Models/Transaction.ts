export enum TransactionType
{
  Buy = 1,
  Sell = -1
}

export type TransactionGet = {
  id: number;
  symbol: string;
  amount: number;
  pricePerCoin: number;
  totalCost: number;
  transactionType: TransactionType; 
  createdAt: string;
};

export type TransactionPost = {
  amount: number;
  symbol: string;
  pricePerCoin: number;
  transactionType: 1 | -1; 
  createdAt: string;
};


export type TransactionQuery = {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
  transactionType?: number;
  assetSymbol?: string;
}
