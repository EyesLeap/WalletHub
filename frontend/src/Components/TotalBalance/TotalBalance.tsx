import { PortfolioDailyChangeGet } from "../../Models/Portfolio";

interface TotalBalanceProps {
  portfolioName: string;
  totalBalance: number;
  dailyChange: PortfolioDailyChangeGet;
}

const TotalBalance: React.FC<TotalBalanceProps> = ({
  portfolioName,
  totalBalance,
  dailyChange,
}) => {
  const isPositive = dailyChange.profitLoss >= 0;
  const sign = isPositive ? "+" : "-";
  const arrow = isPositive ? "▲" : "▼";
  const changeColor = isPositive
    ? "var(--color-profit)"
    : "var(--color-danger)";

  return (
    <div className="flex flex-col p-4">
      <div
        className="text-[#B0B0B0] text-lg"
        style={{ fontFamily: "IBM Plex Sans, sans-serif" }}
      >
        {portfolioName}
      </div>
      <div
        className="text-white font-bold text-4xl mt-2"
        style={{ fontFamily: "Lato, sans-serif" }}
      >
        ${totalBalance.toFixed(2)}
      </div>
      <div
        className="text-lg mt-2 flex items-center gap-1"
        style={{
          fontFamily: "Lato, sans-serif",
          color: changeColor,
        }}
      >
        {sign}${Math.abs(dailyChange.profitLoss).toFixed(2)}{" "}
        {arrow}
        {Math.abs(dailyChange.percentChange24h).toFixed(2)}% (24h)
      </div>
    </div>
  );
};

export default TotalBalance;
