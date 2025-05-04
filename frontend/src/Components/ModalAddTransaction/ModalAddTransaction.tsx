import React, { useState } from "react";
import { TransactionPost } from "../../Models/Transaction";
import styles from "./ModalAddTransaction.module.css"; 

interface ModalAddTransactionProps {
  isOpen: boolean;
  onClose: () => void;
  onCreate: (transactionData: TransactionPost) => void;
}

const ModalAddTransaction: React.FC<ModalAddTransactionProps> = ({ isOpen, onClose, onCreate }) => {
  const [currencySymbol, setCurrencySymbol] = useState("BTC");
  const [amount, setAmount] = useState("");
  const [price, setPrice] = useState("0.00");
  const [createdAt, setCreatedAt] = useState(new Date().toISOString().slice(0, 16));
  const [transactionType, setTransactionType] = useState<1 | -1>(1);

  const handleCreate = () => {
    if (amount.trim() && currencySymbol.trim() && createdAt.trim()) {
      const transaction: TransactionPost = {
        amount: parseFloat(amount),
        symbol: currencySymbol,
        pricePerCoin: parseFloat(price),
        transactionType: transactionType,
        createdAt,
      };

      onCreate(transaction); 
      setAmount("");
      setCurrencySymbol("BTC");
      setCreatedAt(new Date().toISOString().slice(0, 16));
      onClose();
    }
  };

  const totalAmount = parseFloat(amount) * parseFloat(price);
  const totalSpentOrReceived = isNaN(totalAmount) ? 0 : totalAmount;

  if (!isOpen) return null;

  return (
    <div className={styles.modalOverlay}>
      <div className={styles.modalContainer}>
        <button onClick={onClose} className={styles.closeButton}>
          ✕
        </button>

        <h3 className="text-2xl font-semibold mb-4 text-left">Add Transaction</h3>

        <div className={styles.buttonGroup}>
          <button
            className={`${styles.buttonType} ${transactionType === 1 ? styles.buttonBuy : "text-gray-400"}`}
            onClick={() => setTransactionType(1)}
          >
            Buy
          </button>
          <button
            className={`${styles.buttonType} ${transactionType === -1 ? styles.buttonSell : "text-gray-400"}`}
            onClick={() => setTransactionType(-1)}
          >
            Sell
          </button>
        </div>

        <div className="mb-4">
          <label className={styles.inputLabel}>Cryptocurrency</label>
          <input
            type="text"
            value={currencySymbol}
            onChange={(e) => setCurrencySymbol(e.target.value)}
            className={styles.inputField}
          />
        </div>

        <div className="mb-4">
          <label className={styles.inputLabel}>Quantity</label>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            placeholder="0.00"
            className={styles.inputField}
          />
        </div>

        <div className="mb-4">
          <label className={styles.inputLabel}>Price Per Coin</label>
          <div className="flex items-center rounded p-2 mt-1" style={{
            background: "var(--color-input-bg)",
          }}>
            <span className="mr-2 text-gray-400">$</span>
            <input
              type="number"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              className="bg-transparent w-full text-white focus:outline-none"
            />
          </div>
        </div>

        <div className="mb-6">
          <label className={styles.inputLabel}>Date & Time</label>
          <input
            type="datetime-local"
            value={createdAt}
            onChange={(e) => setCreatedAt(e.target.value)}
            className={styles.inputField}
          />
        </div>

        <div className={styles.transactionSummary}>
          <p className="text-gray-400 text-sm">
            {transactionType === 1 ? "Total Spent" : "Total Received"}
          </p>
          <p className={styles.transactionTotal}>
            ${totalSpentOrReceived.toFixed(2)}
          </p>
        </div>

        <div className={styles.actionButtons}>
          <button onClick={onClose} className={styles.cancelButton}>
            Cancel
          </button>
          <button onClick={handleCreate} className={styles.createButton}>
            Add Transaction
          </button>
        </div>
      </div>
    </div>
  );
};

export default ModalAddTransaction;
