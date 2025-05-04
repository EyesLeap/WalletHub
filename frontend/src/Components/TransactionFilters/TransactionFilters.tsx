import { Box, Button, Menu, MenuItem } from "@mui/material";
import { useState } from "react";
import { TransactionType } from "../../Models/Transaction";
import { AssetTableValueGet } from "../../Models/Asset";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";

interface TransactionFiltersProps {
  assets: AssetTableValueGet[];
  onFiltersChange: (filters: { transactionType?: number; assetSymbol?: string }) => void;
}

const TransactionFilters: React.FC<TransactionFiltersProps> = ({ assets, onFiltersChange }) => {
  const [typeAnchorEl, setTypeAnchorEl] = useState<null | HTMLElement>(null);
  const [assetAnchorEl, setAssetAnchorEl] = useState<null | HTMLElement>(null);

  const [selectedType, setSelectedType] = useState<TransactionType | undefined>(undefined);
  const [selectedAssetSymbol, setSelectedAssetSymbol] = useState<string | undefined>(undefined);

  const handleTypeClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setTypeAnchorEl(event.currentTarget);
  };

  const handleAssetClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAssetAnchorEl(event.currentTarget);
  };

  const handleTypeSelect = (type: TransactionType | undefined) => {
    setSelectedType(type);
    onFiltersChange({ transactionType: type, assetSymbol: selectedAssetSymbol });
    setTypeAnchorEl(null);
  };

  const handleAssetSelect = (symbol: string | undefined) => {
    setSelectedAssetSymbol(symbol);
    onFiltersChange({ transactionType: selectedType, assetSymbol: symbol });
    setAssetAnchorEl(null);
  };

  const getTypeButtonLabel = () => {
    if (selectedType === TransactionType.Buy) return "Buy";
    if (selectedType === TransactionType.Sell) return "Sell";
    return "All Types";
  };

  const getAssetButtonLabel = () => {
    return selectedAssetSymbol || "All Assets";
  };

  const buttonSx = {
    fontFamily: "var(--text-regular)",
    fontWeight: "bold",
    color: "var(--color-text)",
    borderColor: "var(--color-border)",
    borderRadius: "8px",
    background: "var(--color-options)",
    minWidth: "12rem",
    display: "flex",
    justifyContent: "space-between",
    paddingLeft: "1rem",
    paddingRight: "1rem",
    "&:hover": {
      opacity: 0.8,
      
    },
  };
  

  const menuPaperSx = {
    background: "var(--color-bg-light)",
    color: "var(--color-text)",
    fontFamily: "var(--text-regular)",
    minWidth: "12rem", 
  };

  const menuItemSx = {
    fontFamily: "var(--text-regular)",
    minWidth: "12rem",
  };

  return (
    <Box
      sx={{
        display: "flex",
        gap: "1rem", 
        paddingBottom: "1.5rem", 
        paddingTop: "1rem",      
      }}
    >
      <div>
        <Button variant="outlined" 
          onClick={handleTypeClick} 
          sx={buttonSx}
          endIcon={<ArrowDropDownIcon />}
        >
          {getTypeButtonLabel()}
        </Button>
        <Menu
          anchorEl={typeAnchorEl}
          open={Boolean(typeAnchorEl)}
          onClose={() => setTypeAnchorEl(null)}
          PaperProps={{ sx: menuPaperSx }}
        >
          <MenuItem onClick={() => handleTypeSelect(undefined)} sx={menuItemSx}>
            All
          </MenuItem>
          <MenuItem onClick={() => handleTypeSelect(TransactionType.Buy)} sx={menuItemSx}>
            Buy
          </MenuItem>
          <MenuItem onClick={() => handleTypeSelect(TransactionType.Sell)} sx={menuItemSx}>
            Sell
          </MenuItem>
        </Menu>
      </div>

      <div>
        <Button variant="outlined" 
            onClick={handleAssetClick} 
            sx={buttonSx} 
            endIcon={<ArrowDropDownIcon />}
          >
          {getAssetButtonLabel()}
        </Button>
        <Menu
          anchorEl={assetAnchorEl}
          open={Boolean(assetAnchorEl)}
          onClose={() => setAssetAnchorEl(null)}
          PaperProps={{ sx: menuPaperSx }}
          
        >
          <MenuItem onClick={() => handleAssetSelect(undefined)} sx={menuItemSx}>
            All
          </MenuItem>
          {assets.map((asset) => (
            <MenuItem
              key={asset.symbol}
              onClick={() => handleAssetSelect(asset.symbol)}
              sx={menuItemSx}
            >
              {asset.symbol}
            </MenuItem>
          ))}
        </Menu>
      </div>
    </Box>
  );
};

export default TransactionFilters;
