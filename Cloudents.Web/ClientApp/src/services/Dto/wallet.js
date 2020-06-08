import { i18n } from '../../plugins/t-i18n'

// TODO clean!
const balanceNameByType = {
    'Earned': i18n.t(`wallet_earned`),
    'Spent': i18n.t(`wallet_spent`),
    'Total': i18n.t(`wallet_total`),
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