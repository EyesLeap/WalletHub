import React, { useEffect, useState } from "react";

interface ModalEditPortfolioProps {
  isOpen: boolean;
  onClose: () => void;
  onEdit: (newPortfolioName: string) => void;
  currentName: string;
}

const ModalEditPortfolio: React.FC<ModalEditPortfolioProps> = ({
  isOpen,
  onClose,
  onEdit,
  currentName,
}) => {
  const [portfolioName, setPortfolioName] = useState(currentName);

  useEffect(() => {
    setPortfolioName(currentName);
  }, [isOpen, currentName]); 

  const handleEdit = () => {
    if (portfolioName.trim()) {
      onEdit(portfolioName);
      onClose(); 
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 flex justify-center items-center bg-black bg-opacity-50 z-50">
      <div
        className="w-96 p-6 rounded-lg shadow-lg text-white relative"
        style={{
          background: "linear-gradient(to bottom right, rgb(71 74 71), rgb(46 50 46))",
        }}
      >
        <button
          onClick={onClose}
          className="absolute top-3 right-3 text-gray-400 hover:text-white text-xl"
        >
          ✕
        </button>

        <h3 className="text-lg font-semibold mb-4 text-center">Edit Portfolio Name</h3>

        <div className="mb-4">
          <label className="text-gray-400 text-sm">Portfolio Name</label>
          <input
            type="text"
            value={portfolioName}
            onChange={(e) => setPortfolioName(e.target.value)}
            className="w-full p-2 rounded focus:outline-none mt-1 text-white"
            style={{
              background: "linear-gradient(to bottom right, rgba(40, 40, 40, 0.9), rgba(20, 20, 20, 0.5))",
            }}
          />
        </div>

        <div className="flex justify-end gap-2">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            Cancel
          </button>
          <button
            onClick={handleEdit}
            className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            Save
          </button>
        </div>
      </div>
    </div>
  );
};

export default ModalEditPortfolio;
