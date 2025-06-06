import React, { useEffect, useState } from "react";

import { MarketCurrencyGet } from "../../Models/MarketCurrency";
import { getPopularCurrenciesAPI } from "../../Services/MarketCurrencyService";
import MarketCurrencyTable from "../../Components/MarketCurrencyTable/MarketCurrencyTable";

const CryptocurrenciesPage: React.FC = () => {
  const [currencies, setCurrencies] = useState<MarketCurrencyGet[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCurrencies = async () => {
      try {
        setLoading(true);
        const data = await getPopularCurrenciesAPI();
        setCurrencies(data);
      } catch (e: any) {
        setError(e.message || "Failed to load currencies");
      } finally {
        setLoading(false);
      }
    };

    fetchCurrencies();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <MarketCurrencyTable currencies={currencies} />
    </div>
  );
};

export default CryptocurrenciesPage;
