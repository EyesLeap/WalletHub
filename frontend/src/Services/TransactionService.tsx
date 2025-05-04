import axios from "axios";
import { PortfolioGet, PortfolioPost } from "../Models/Portfolio";
import { handleError } from "../Helpers/ErrorHandler";
import { TransactionGet, TransactionPost, TransactionQuery } from "../Models/Transaction";

const api = process.env.REACT_APP_API_URL + "transactions/";



export const createTransactionAPI = async (portfolioId: number, transaction: TransactionPost) => {
  try {
    console.log("Sending transaction:", transaction);

    const response = await axios.post(
      `${api}${portfolioId}`,
      transaction
    );

    return response.data;
  } catch (error: any) {
    console.error("Error creating transaction:", error.response?.data || error.message);
  }
};

export const getTransactionByIdAPI = async (portfolioId: number, transactionId: number) => {
  try {
    const response = await axios.get(
      `${api}${portfolioId}/${transactionId}`
    );
    return response.data; 
  } catch (error) {
    handleError(error); 
  }
};

export const getPortfolioTransactionsAPI = async (
  portfolioId: number,
  queryParams?: TransactionQuery
) => {
  try {
    const response = await axios.get(`${api}${portfolioId}`, {
      params: queryParams,
    });
    return response.data;
  } catch (error) {
    handleError(error);
  }
};


export const deleteTransactionAPI = async (transactionId: number) => {
  try {
    const data = await axios.delete(`${api}${transactionId}`);
    return data;
  } catch (error) {
  }
};

