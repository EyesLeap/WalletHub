import React from "react";
import { FaCoins } from "react-icons/fa";

interface SidebarSummaryProps {
  totalValueUSD: number;
}

const SidebarSummary: React.FC<SidebarSummaryProps> = ({ totalValueUSD }) => {
  return (
    <div className="flex items-center gap-3 mb-4">
      <FaCoins className="text-3xl text-yellow-400" />
      <div>
        <h2 className="text-xs font-bold text-[var(--color-text-muted)] uppercase tracking-wider">
          Total Value
        </h2>
        <p className="text-xl font-bold text-[var(--color-text)]">
          ${totalValueUSD.toLocaleString(undefined, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })}
        </p>
      </div>
    </div>
  );
};

export default SidebarSummary;
