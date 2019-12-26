import { connectivityModule } from "./connectivity.module"
import { LanguageService } from "./language/languageService";


function BalanceDetails(objInit){
    this.points = objInit.points;
    this.symbol = objInit.symbol;
    this.type = objInit.type;
    this.value = objInit.value;
    this.name = LanguageService.getValueByKey(`wallet_${objInit.type.toLowerCase()}`);
}

export default {
    getBalances: () => {
        return connectivityModule.http.get("/Wallet/balance").then(({data})=>{
            return data.map((item)=>{
                return new BalanceDetails(item);
            });
        });
    },
    getTransactions: () => connectivityModule.http.get("/Wallet/transaction"),
    buyTokens: (points) => connectivityModule.http.post("/Wallet/buyTokens", points),
    redeem: (amount) => connectivityModule.http.post("/Wallet/redeem", {amount}),
    getPaymeLink: () => connectivityModule.http.get("/Wallet/getPaymentLink")
}