export enum PortfolioSnapshotRange
{
  All,
  Last24Hours,
  Last7Days,
  Last30Days
}

export type PortfolioSnapshotGet = {
  id: number;
  portfolioId: number;
  createdAt: string;
  totalValueUSD: number;
};