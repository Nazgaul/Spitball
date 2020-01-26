import Api from './Api/wallet';
import { Wallet } from './Constructors/wallet.js';

export default {
    async getBalances() {
        let { data } = await Api.get.balance()
        return new Wallet.Balances(data)
    },
    async getPaymeLink() {
        return await Api.get.paymentLink()
    },
    async redeem(amount) {
        return await Api.post.redeem(amount)
    },
    async buyTokens(points) {
        return await Api.post.buyTokens(points)
    },
    async getTransactions() {
        return await Api.get.taransactions()
    },
}