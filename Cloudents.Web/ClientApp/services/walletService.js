import axios from 'axios';

const walletInstance = axios.create({
    baseURL:'/api/wallet'
})


import Api from './Api/wallet';
import { Wallet } from './Dto/wallet.js';

export default {
    async getBalances() {
        let { data } = await Api.get.balance()
        return data.map(item=> new Wallet.Balance(item))
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
    async paypalBuyTokens(id){ 
        return await walletInstance.post('PayPal/buyTokens',{id})
    },
    async paypalStudyRoom(model){ 
        return await walletInstance.post('PayPal/StudyRoom',model)
    },
}