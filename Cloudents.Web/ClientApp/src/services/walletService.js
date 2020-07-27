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
    getPaymeLink() {
        return Api.get.paymentLink();
    },
    redeem(amount) {
        return Api.post.redeem(amount);
    },
    // async paypalBuyTokens(id){
    //     return await walletInstance.post('PayPal/buyTokens',{id})
    // },
    // async paypalStudyRoom(model){
    //     return await walletInstance.post('PayPal/StudyRoom',model)
    // },

    async getStripeSecret() {
        return await walletInstance.post('/stripe/StudyRoom');
    },
    async subscribe(id) {
        let {data} = await axios.post(`/Tutor/${id}/subscribe`);
        return data;
    }
}