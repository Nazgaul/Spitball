import { LanguageService } from "../language/languageService";

const balanceNameByType = {
    'Earned': LanguageService.getValueByKey(`wallet_earned`),
    'Spent': LanguageService.getValueByKey(`wallet_spent`),
    'Total': LanguageService.getValueByKey(`wallet_total`),
}

export const Wallet = {
    Balance: function (objInit) {
        this.points = objInit.points;
        this.symbol = objInit.symbol;
        this.type = objInit.type;
        this.value = objInit.value;
        this.name = balanceNameByType[objInit.type];
    },
}