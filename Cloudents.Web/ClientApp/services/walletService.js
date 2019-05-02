import { connectivityModule } from "./connectivity.module"


export default {
    getBalances: () => connectivityModule.http.get("/Wallet/balance"),
    getTransactions: () => connectivityModule.http.get("/Wallet/transaction"),
    buyTokens: (data) => connectivityModule.http.post("/Wallet/buyTokens", data),
    redeem: (amount) => connectivityModule.http.post("/Wallet/redeem", {amount}),
    getPaymeLink: () => connectivityModule.http.get("/Wallet/getPaymentLink"),
}