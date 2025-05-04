import React, { useState } from "react";
import styles from "./ModalCreatePortfolio.module.css";

interface ModalCreatePortfolioProps {
  isOpen: boolean;
  onClose: () => void;
  onCreate: (portfolioName: string) => void;
}

const ModalCreatePortfolio: React.FC<ModalCreatePortfolioProps> = ({
  isOpen,
  onClose,
  onCreate,
}) => {
  const [portfolioName, setPortfolioName] = useState("");

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPortfolioName(event.target.value);
  };

  const handleCreate = () => {
    if (portfolioName.trim()) {
      onCreate(portfolioName);
      setPortfolioName("");
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className={styles.modalOverlay}>
      <div className={styles.modalContainer}>
        <button onClick={onClose} className={styles.closeButton}>
          ✕
        </button>

        <h3 className={styles.modalTitle}>Create New Portfolio</h3>

        <div className="mb-6">
          <label className={styles.inputLabel}>Portfolio Name</label>
          <input
            type="text"
            value={portfolioName}
            onChange={handleInputChange}
            placeholder="Enter portfolio name"
            className={styles.inputField}
          />
        </div>

        <div className={styles.buttonGroup}>
          <button onClick={onClose} className={styles.cancelButton}>
            Cancel
          </button>
          <button onClick={handleCreate} className={styles.createButton}>
            Create
          </button>
        </div>
      </div>
    </div>
  );
};

export default ModalCreatePortfolio;
