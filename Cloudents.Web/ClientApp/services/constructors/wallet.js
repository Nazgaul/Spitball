import { LanguageService } from "../language/languageService";

export const Wallet = {
    Balance: function (objInit) {
        this.points = objInit.points;
        this.symbol = objInit.symbol;
        this.type = objInit.type;
        this.value = objInit.value;
        this.name = LanguageService.getValueByKey(`wallet_${objInit.type.toLowerCase()}`);
    },
    Balances: function(objInit){
        this.Balances = objInit.map(item=> new Wallet.Balance(item))
    }
}