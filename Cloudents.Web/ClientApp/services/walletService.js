
import { connectivityModule } from "./connectivity.module"


export default {
    getBalances: () => connectivityModule.http.get("/Wallet/balance"),
    getTransactions: () => connectivityModule.http.get("/Wallet/transaction"),
    redeem: (amount) => connectivityModule.http.post("/Wallet/redeem", amount)
}