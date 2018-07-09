import axios from "axios";

export default {
    getBalances: () => axios.get("/Wallet/balance"),
    getTransactions: () => axios.get("/Wallet/transaction"),
    redeem: (amount) => (axios.post("/Wallet/redeem", {amount}))
}