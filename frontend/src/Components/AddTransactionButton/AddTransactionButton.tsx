import React from "react";
import styles from "./AddTransactionButton.module.css";
interface AddTransactionButtonProps {
  onClick: () => void;
}

const AddTransactionButton: React.FC<AddTransactionButtonProps> = ({ onClick }) => {
  return (
    <button
      onClick={onClick}
      className={styles.addTransactionButton}
    >
      + Add Transaction
    </button>
  );
};

export default AddTransactionButton;
