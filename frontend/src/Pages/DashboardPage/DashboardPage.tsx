import React, { useCallback, useEffect, useState } from "react";
import Sidebar from "../../Components/Sidebar/Sidebar";
import TotalBalance from "../../Components/TotalBalance/TotalBalance";
import PortfolioTabs from "../../Components/PortfolioTabs/PortfolioTabs";
import Spinner from "../../Components/Spinners/Spinner";
import AddTransactionButton from "../../Components/AddTransactionButton/AddTransactionButton";
import ModalAddTransaction from "../../Components/ModalAddTransaction/ModalAddTransaction";

import { portfolioGetByIdAPI, getPortfolioTotalValueAPI, getAllPortfoliosAPI, deletePortfolioAPI, updatePortfolioNameAPI, getPortfolioDailyChangeAPI } from "../../Services/PortfolioService";
import { createTransactionAPI, deleteTransactionAPI } from "../../Services/TransactionService";

import { PortfolioDailyChangeGet, PortfolioGet, PortfolioTotalValueGet } from "../../Models/Portfolio";
import { TransactionPost } from "../../Models/Transaction";
import MoreOptionsButton from "../../Components/MoreOptionsButton/MoreOptionsButton";
import ModalEditPortfolio from "../../Components/ModalEditPortfolio/ModalEditPortfolio";
import { PortfolioSnapshotGet, PortfolioSnapshotRange } from "../../Models/PortfolioSnapshot";
import { getPortfolioSnapshotsAPI } from "../../Services/PortfolioSnapshotService";
import PortfolioChart from "../../Components/PortfolioChart/PortfolioChart";

const DashboardPage = () => {
  const [activePortfolioId, setActivePortfolioId] = useState<number | null>(null);
  const [portfolios, setPortfolios] = useState<PortfolioGet[]>([]);
  const [portfolio, setPortfolio] = useState<PortfolioGet | null>(null);
  const [portfolioTotalValue, setPortfolioTotalValue] = useState<PortfolioTotalValueGet | null>(null);
  const [portfolioDailyChange, setPortfolioDailyChange] = useState<PortfolioDailyChangeGet | null>(null);
  const [snapshots, setSnapshots] = useState<PortfolioSnapshotGet[]>([]);
  const [snapshotsFilter, setSnapshotsFilter] = useState<PortfolioSnapshotRange>(PortfolioSnapshotRange.All);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [transactionAddedTrigger, setTransactionTrigger] = useState(0);
  const [modals, setModals] = useState({
    edit: false,
    delete: false,
    addTransaction: false,
  });

  const openModal = (name: keyof typeof modals) =>
  setModals((prev) => ({ ...prev, [name]: true }));

const closeModal = (name: keyof typeof modals) =>
  setModals((prev) => ({ ...prev, [name]: false }));


  

  const handleRemovePortfolio = async () => {
    if (!activePortfolioId) return;

    try {
      
      console.log("Portfolio removed successfully");
      deletePortfolioAPI(activePortfolioId);
      setPortfolios(portfolios.filter((portfolio) => portfolio.id !== activePortfolioId));
      setActivePortfolioId(null);
      setPortfolio(null);
      setPortfolioTotalValue(null);
    } catch (error) {
      console.error("Error removing portfolio:", error);
    }
  };

  const fetchPortfolioData = useCallback(async () => {
    if (!activePortfolioId) return;
  
    try {
      const portfolioResponse = await portfolioGetByIdAPI(activePortfolioId);
      const portfolioTotalValueResponse = await getPortfolioTotalValueAPI(activePortfolioId);
      const portfolioDailyChangeResponse = await getPortfolioDailyChangeAPI(activePortfolioId);
  
      setPortfolio(portfolioResponse);
      setPortfolioTotalValue(portfolioTotalValueResponse);
      setPortfolioDailyChange(portfolioDailyChangeResponse);
    } catch (error) {
      console.error("Error fetching portfolio data", error);
      setPortfolio(null);
      setPortfolioTotalValue(null);
      setPortfolioDailyChange(null);
    }
  }, [activePortfolioId]);

  const handleEditPortfolio = async (newPortfolioName: string) => {
    if (!activePortfolioId || !newPortfolioName.trim()) return; 
  
    try {
      await updatePortfolioNameAPI(activePortfolioId, newPortfolioName);
      console.log("Portfolio name updated successfully");
  
      await fetchPortfolioData(); 
      closeModal("edit");
  
    } catch (error) {
      console.error("Error updating portfolio name:", error);
    }
  };
  

  const handleCreateTransaction = async (transaction: TransactionPost) => {
    if (!activePortfolioId) return;

    try {
      await createTransactionAPI(activePortfolioId, transaction);
      console.log("Transaction created successfully");

      await fetchPortfolioData(); 
      closeModal("addTransaction");
      setTransactionTrigger(prev => prev + 1); 
    } catch (error) {
      console.error("Error creating transaction:", error);
    }
  };


  useEffect(() => {
    const fetchPortfolios = async () => {
      try {
          const data = await getAllPortfoliosAPI();
          setPortfolios(data);
    
        if (!activePortfolioId && data.length > 0) {
          setActivePortfolioId(data[0].id);
        }
      } catch (error) {
        console.error("Error fetching portfolios", error);
      }
    };

    fetchPortfolios();
  }, [activePortfolioId]);

  useEffect(() => {
    fetchPortfolioData();
  }, [fetchPortfolioData]);

  useEffect(() => {
    const fetchSnapshots = async () => {
      if (activePortfolioId === null || activePortfolioId === undefined) {
        return;
      }
      try {
        const data = await getPortfolioSnapshotsAPI(activePortfolioId, snapshotsFilter);
        setSnapshots(data);
      } catch (error) {
        console.error('Ошибка при получении снимков:', error);
      }
    };

    fetchSnapshots();
  }, [activePortfolioId, snapshotsFilter]);


  return (
    <div className="w-full h-full relative flex overflow-x-hidden overflow-y-hidden">
      <Sidebar activePortfolioId={activePortfolioId} onPortfolioClick={setActivePortfolioId} /> 
  
      <div className="relative md:ml-64 bg-blueGray-100 w-full h-full">
        <div className="relative pb-32 bg-lightBlue-500 h-full">
          <div className="px-4 md:px-6 mx-auto w-full">
            {portfolio ? (
              <>
                <div className="flex justify-between items-center mt-4">
                  <TotalBalance
                    portfolioName={portfolio.name}
                    totalBalance={portfolioTotalValue?.totalValueUSD || 0}
                    dailyChange={portfolioDailyChange || { profitLoss: 0, percentChange24h: 0 }}
                  />
                  <div className="flex space-x-4">
                    <AddTransactionButton onClick={() => openModal("addTransaction")} />
                    <MoreOptionsButton
                      onEdit={() => openModal("edit")}
                      onRemove={handleRemovePortfolio}
                    />
                  </div>
                </div>
                <PortfolioTabs
                  activePortfolioId={activePortfolioId || 0}
                  transactionAddedTrigger={transactionAddedTrigger}
                  snapshots={snapshots}
                  latestPortfolioValue={portfolioTotalValue?.totalValueUSD}
                  onTransactionRemoved={async () => {
                    await fetchPortfolioData();
                    const updatedSnapshots = await getPortfolioSnapshotsAPI(
                      activePortfolioId,
                      snapshotsFilter
                    );
                    setSnapshots(updatedSnapshots);
                  }}
                />
              </>
            ) : (
              <div className="flex justify-center items-center h-[calc(100vh-80px)]">
                <Spinner />
              </div>
            )}
          </div>
        </div>
      </div>
  
      <ModalAddTransaction
        isOpen={modals.addTransaction}
        onClose={() => closeModal("addTransaction")}
        onCreate={handleCreateTransaction}
      />
      <ModalEditPortfolio
        isOpen={modals.edit}
        onClose={() => closeModal("edit")}
        onEdit={handleEditPortfolio}
        currentName={portfolio?.name || ""}
      />
    </div>
  );
  
};

export default DashboardPage;
