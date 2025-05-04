import React from "react";
import Table from "@mui/material/Table";
import {
  Paper,
  TableCell,
  TableHead,
  TableRow,
  TableContainer,
  TableBody,
  Typography,
  IconButton,
  Box,
} from "@mui/material";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrashAlt } from "@fortawesome/free-regular-svg-icons";
import { TransactionGet, TransactionType } from "../../Models/Transaction";

interface TransactionTableProps {
  transactions: TransactionGet[];
  onRemove: (id: number) => void;
}

const baseCellSx = {
  fontFamily: "'Lato', sans-serif",
  fontWeight: 700,
  fontSize: "1rem",
  borderBottom: "2px solid var(--color-border)",
  padding: "12px",
  lineHeight: 3,
};

const headerCellSx = {
  fontWeight: 800,
  fontSize: "0.9rem",
  fontFamily: "'IBM Plex Sans', sans-serif",
  color: "var(--color-text-muted)",
  padding: "8px 12px",
  borderBottom: "none",
};

const getTransactionTypeColor = (type: 1 | -1) => {
  if (type === TransactionType.Buy) return { color: "#37a356" };
  if (type === TransactionType.Sell) return { color: "var(--color-danger)" };
  return { color: "var(--color-text-muted)" };
};

const TransactionTable: React.FC<TransactionTableProps> = ({
  transactions,
  onRemove,
}) => { 
  return (
    <>
    

      <Paper
        sx={{
          boxShadow: "none",
          overflow: "hidden",
          background: "var(--gradient-light)",
          borderRadius: "6px",
        }}
      >
        <TableContainer>
          <Table sx={{ minWidth: 700, borderCollapse: "collapse" }}>
            <TableHead>
              <TableRow sx={{ borderBottom: "2px solid var(--color-border)" }}>
                <TableCell sx={headerCellSx} align="left">
                  DATE
                </TableCell>
                {["SYMBOL", "AMOUNT", "PRICE PER COIN", "TOTAL COST", "TYPE"].map(
                  (header, index) => (
                    <TableCell key={index} align="right" sx={headerCellSx}>
                      {header}
                    </TableCell>
                  )
                )}
                <TableCell sx={headerCellSx} align="center">
                  ACTIONS
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {transactions.map((transaction, index) => (
                <TableRow
                  key={index}
                  sx={{
                    "&:hover": {
                      backgroundColor: "rgba(50, 50, 50, 0.1)",
                    },
                  }}
                >
                  <TableCell sx={{ ...baseCellSx, color: "var(--color-text-muted)" }}>
                    <Typography
                      variant="body1"
                      sx={{
                        fontFamily: "'IBM Plex Sans', sans-serif",
                        fontWeight: 500,
                        fontSize: "1rem",
                        color: "var(--color-text-muted)",
                      }}
                    >
                      {new Date(transaction.createdAt).toLocaleDateString()}
                    </Typography>
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    {transaction.symbol}
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    {transaction.amount.toFixed(2)}
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    ${transaction.pricePerCoin.toFixed(2)}
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    ${transaction.totalCost.toFixed(2)}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, ...getTransactionTypeColor(transaction.transactionType) }}
                  >
                    {transaction.transactionType === TransactionType.Buy ? "BUY" : "SELL"}
                  </TableCell>

                  <TableCell align="center" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    <IconButton onClick={() => onRemove(transaction.id)}>
                      <FontAwesomeIcon
                        icon={faTrashAlt}
                        size="lg"
                        style={{ color: "var(--color-text-muted)" }}
                      />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </>
  );
};

export default TransactionTable;
