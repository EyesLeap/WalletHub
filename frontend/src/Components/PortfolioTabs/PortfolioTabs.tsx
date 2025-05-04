import React, { useState, useEffect } from "react";
import AssetTable from "../AssetTable/AssetTable";
import TransactionTable from "../TransactionTable/TransactionTable";
import { AssetTableValueGet } from "../../Models/Asset";
import { PortfolioTotalValueGet } from "../../Models/Portfolio";
import { getPortfolioTotalValueAPI } from "../../Services/PortfolioService";
import { TransactionGet, TransactionQuery } from "../../Models/Transaction";
import { deleteTransactionAPI, getPortfolioTransactionsAPI} from "../../Services/TransactionService";
import styles from './PortfolioTabs.module.css'
import { PortfolioSnapshotGet } from "../../Models/PortfolioSnapshot";
import PortfolioChart from "../PortfolioChart/PortfolioChart";
import TransactionFilters from "../TransactionFilters/TransactionFilters";

interface PortfolioTabsProps {
  activePortfolioId: number;
  transactionAddedTrigger: number;
  onTransactionRemoved: () => void;
  snapshots: PortfolioSnapshotGet[];
  latestPortfolioValue?: number;
}

const PortfolioTabs: React.FC<PortfolioTabsProps> = ({ activePortfolioId, 
  transactionAddedTrigger, 
  onTransactionRemoved,
  snapshots,
  latestPortfolioValue}) => {
  const [activeTab, setActiveTab] = useState<"holdings" | "transactions">("holdings");
  const [portfolioTotalValue, setPortfolioTotalValue] = useState<PortfolioTotalValueGet | null>(null);
  const [transactions, setTransactions] = useState<TransactionGet[] | null>(null);
  const [transactionQueryParams, setTransactionQueryParams] = useState<TransactionQuery>({});
  const [transactionTypeFilter, setTransactionTypeFilter] = useState<number | undefined>(undefined);
  const [assetSymbolFilter, setAssetSymbolFilter] = useState<string | undefined>(undefined);



  const handleRemoveTransaction = async (id: number) => {
    try {
      await deleteTransactionAPI(id);
      const updatedTransactions = await getPortfolioTransactionsAPI(activePortfolioId);
      setTransactions(updatedTransactions);
      onTransactionRemoved();
      console.log("Transaction removed successfully"); 
    } catch (error) {
      console.error("Error removing transaction:", error);
    }
  };

  useEffect(() => {
    if (!activePortfolioId) return;

    const fetchPortfolioTotalValue = async () => {
      try {
        const portfolioTotalValueResponse = await getPortfolioTotalValueAPI(activePortfolioId);
        setPortfolioTotalValue(portfolioTotalValueResponse);
      } catch (error) {
        console.error("Error fetching portfolio total value", error);
        setPortfolioTotalValue(null);
      } 
    };

    fetchPortfolioTotalValue();
  }, [activePortfolioId, transactionAddedTrigger]);

  useEffect(() => {
    if (!activePortfolioId) return;
    console.log(transactionAddedTrigger);
    const fetchTransactions = async () => {
      if (!activePortfolioId) return;
      
      try {
        const transactionsResponse = await getPortfolioTransactionsAPI(activePortfolioId, {
          transactionType: transactionTypeFilter,
          assetSymbol: assetSymbolFilter,
        });
        setTransactions(transactionsResponse);
      } catch (error) {
        console.error("Error fetching transactions", error);
        setTransactions(null);
      }
    };
    

    fetchTransactions();
  }, [activePortfolioId, transactionAddedTrigger, transactionTypeFilter, assetSymbolFilter]);

  return (
    <div className={styles.portfolioTabs}>
      <div className={styles.portfolioTabsNav}>
        <button
          className={`${styles.tabButton} ${activeTab === "holdings" ? styles.tabButtonActive : ""}`}
          onClick={() => setActiveTab("holdings")}
        >
          Holdings
        </button>
        <button
          className={`${styles.tabButton} ${activeTab === "transactions" ? styles.tabButtonActive : ""}`}
          onClick={() => setActiveTab("transactions")}
        >
          Transactions
        </button>
      </div>
     
      <div className={styles.tabContent}>
        {activeTab === "holdings" ? (
          <>
            <PortfolioChart
            snapshots={snapshots}
            latestPortfolioValue={portfolioTotalValue?.totalValueUSD}
          />
            <AssetTable assetValues={portfolioTotalValue?.assetValues || []} />
          </>
        ) : (
          <>
          <TransactionFilters 
            assets={portfolioTotalValue?.assetValues || []}
            onFiltersChange={({ transactionType, assetSymbol }) => {
              setTransactionTypeFilter(transactionType);
              setAssetSymbolFilter(assetSymbol);
            }}
          />
          <TransactionTable 
            transactions={transactions || []} 
            onRemove={handleRemoveTransaction} 
          />
        </>
        )}
      </div>
    </div>
  );
};

export default PortfolioTabs;
