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
  Box,
} from "@mui/material";
import { MarketCurrencyGet } from "../../Models/MarketCurrency";

interface MarketCurrencyTableProps {
  currencies: MarketCurrencyGet[];
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

const getColor = (value: number) => {
  if (value > 0) return { color: "var(--color-profit)" };
  if (value < 0) return { color: "var(--color-danger)" };
  return { color: "var(--color-text-muted)" };
};

const MarketCurrencyTable: React.FC<MarketCurrencyTableProps> = ({ currencies }) => {
  return (
    <>
      <Box sx={{ padding: "1rem" }}>
        <Typography
          variant="h2"
          sx={{
            fontFamily: "'Lato', sans-serif",
            fontSize: "2rem",
            fontWeight: "bold",
            color: "var(--color-text)",
          }}
        >
          Top Cryptocurrencies
        </Typography>
      </Box>

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
                <TableCell align="left" sx={headerCellSx}>
                  NAME
                </TableCell>
                <TableCell align="right" sx={headerCellSx}>
                  SYMBOL
                </TableCell>
                <TableCell align="right" sx={headerCellSx}>
                  PRICE (USD)
                </TableCell>
                <TableCell align="right" sx={headerCellSx}>
                  MARKET CAP
                </TableCell>
                <TableCell align="right" sx={headerCellSx}>
                  VOLUME 24H
                </TableCell>
                <TableCell align="right" sx={headerCellSx}>
                  CHANGE 24H
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {currencies.map((currency) => (
                <TableRow
                  key={currency.id}
                  sx={{
                    "&:hover": {
                      backgroundColor: "rgba(50, 50, 50, 0.1)",
                    },
                  }}
                >
                  <TableCell
                    sx={{
                      ...baseCellSx,
                      color: "var(--color-text)",
                      fontWeight: 700,
                    }}
                  >
                    {currency.name}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, color: "var(--color-text-muted)" }}
                  >
                    {currency.symbol}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, color: "var(--color-text)" }}
                  >
                    ${currency.price.toFixed(2)}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, color: "var(--color-text)" }}
                  >
                    ${currency.marketCap.toLocaleString()}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, color: "var(--color-text)" }}
                  >
                    ${currency.volume24h.toLocaleString()}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, ...getColor(currency.percentChange24h) }}
                  >
                    <Box display="flex" justifyContent="flex-end" alignItems="center" gap={0.5}>
                      {currency.percentChange24h > 0 && <span>▲</span>}
                      {currency.percentChange24h < 0 && <span>▼</span>}
                      <span>{Math.abs(currency.percentChange24h).toFixed(2)}%</span>
                    </Box>
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

export default MarketCurrencyTable;
