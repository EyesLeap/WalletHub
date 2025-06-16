import React, { useState, useEffect } from "react";
import { getAllPortfoliosAPI, createPortfolioAPI } from "../../Services/PortfolioService";
import ModalCreatePortfolio from "../ModalCreatePortfolio/ModalCreatePortfolio";
import './Sidebar.css'; 
import { FaWallet } from "react-icons/fa"
import SidebarSummary from "../SidebarSummary/SidebarSummary";
import { toast } from "react-toastify";

interface SidebarProps {
  onPortfolioClick: (id: number) => void; 
  activePortfolioId: number | null;
}

const Sidebar: React.FC<SidebarProps> = ({ onPortfolioClick, activePortfolioId }) => {
  const [portfolios, setPortfolios] = useState<
    { id: number; name: string; totalValueUSD: number }[]
  >([]);

  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const fetchPortfolios = async () => {
      try {
        const data = await getAllPortfoliosAPI(); 
        setPortfolios(data); 
      } catch (error) {
        console.error("Error in loading portfolio:", error);
      }
    };

    fetchPortfolios(); 
  }, [activePortfolioId]);

  const openModal = () => {
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
  };

  const createPortfolio = async (portfolioName: string) => {
    try {
      const newPortfolio = await createPortfolioAPI(portfolioName);
      if (newPortfolio) {  
        setPortfolios([...portfolios, newPortfolio]);
        closeModal();
      } else {
        
        console.error("Portfolio was not created");
      }
    } catch (error) {
      console.error("Error in creating portfolio", error);
    }
  };

  const totalValue = portfolios.reduce((sum, p) => sum + p.totalValueUSD, 0);

  return (
    <div>
      <nav className="sidebar">
        <div className="flex flex-col flex-grow">

          <SidebarSummary totalValueUSD={totalValue} />
          <h4 className="mt-4 text-sm font-bold">
            My Portfolios ({portfolios.length})
          </h4>

          <div className="mt-4 flex flex-col gap-2">
            {portfolios.map((portfolio) => (
              <button
                key={portfolio.id}
                onClick={() => onPortfolioClick(portfolio.id)}
                className={`portfolio-btn ${activePortfolioId === portfolio.id ? 'selected-portfolio' : ''}`}
              >
                <div className="flex items-center portfolio-name">
                  <FaWallet className="text-xl text-[var(--color-text-muted)] mr-2" />
                  {portfolio.name}
                </div>

                <div className="portfolio-total-value ml-7">
                  ${portfolio.totalValueUSD.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                </div>
              </button>
            ))}


            <button
              onClick={openModal}
              className="create-portfolio-btn"
            >
              + Create Portfolio
            </button>
          </div>
        </div>
      </nav>

      <ModalCreatePortfolio isOpen={isModalOpen} onClose={closeModal} onCreate={createPortfolio} />
    </div>
  );
};

export default Sidebar;
