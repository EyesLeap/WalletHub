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
import { AssetTableValueGet } from "../../Models/Asset";

interface AssetTableProps {
  assetValues: AssetTableValueGet[];
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

const AssetTable: React.FC<AssetTableProps> = ({ assetValues }) => {
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
          Assets
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
                {["PRICE", "24H%", "AMOUNT", "HOLDINGS", "AVG. BUY PRICE", "PROFIT/LOSS"].map(
                  (header, index) => (
                    <TableCell key={index} align="right" sx={headerCellSx}>
                      {header}
                    </TableCell>
                  )
                )}
              </TableRow>
            </TableHead>

            <TableBody>
              {assetValues.map((asset, index) => (
                <TableRow
                  key={index}
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
                    }}
                  >
                    <Box display="flex" alignItems="center">
                      <Typography
                        variant="body1"
                        sx={{
                          fontFamily: "'IBM Plex Sans', sans-serif",
                          fontWeight: 700,
                          fontSize: "1rem",
                          color: "var(--color-text)",
                        }}
                      >
                        {asset.name}
                      </Typography>
                      <Typography
                        variant="body1"
                        sx={{
                          fontFamily: "'IBM Plex Sans', sans-serif",
                          fontWeight: 500,
                          fontSize: "1rem",
                          color: "var(--color-text-muted)",
                          marginLeft: "0.5rem",
                        }}
                      >
                        {asset.symbol}
                      </Typography>
                    </Box>
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    ${asset.price.toFixed(2)}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, ...getColor(asset.percentChange24h || 0) }}
                  >
                    <Box display="flex" justifyContent="flex-end" alignItems="center" gap={0.5}>
                      {asset.percentChange24h > 0 && <span>▲</span>}
                      {asset.percentChange24h < 0 && <span>▼</span>}
                      <span>{Math.abs(asset.percentChange24h).toFixed(2)}%</span>
                    </Box>
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    {asset.amount.toFixed(2)}
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    ${asset.totalValue.toFixed(2)}
                  </TableCell>

                  <TableCell align="right" sx={{ ...baseCellSx, color: "var(--color-text)" }}>
                    ${asset.averagePurchasePrice.toFixed(2)}
                  </TableCell>

                  <TableCell
                    align="right"
                    sx={{ ...baseCellSx, ...getColor(asset.profitLoss) }}
                  >
                    {asset.profitLoss < 0
                      ? `- $${Math.abs(asset.profitLoss).toFixed(2)}`
                      : `$${asset.profitLoss.toFixed(2)}`}
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

export default AssetTable;
