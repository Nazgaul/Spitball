import walletService from '../services/walletService.js';

import { loadStripe } from '@stripe/stripe-js';
import * as componentConsts from '../components/pages/global/toasterInjection/componentConsts.js'

const state = {
    paymentURL: null,
    isBuyPoints: null
};

const mutations = {
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
    setIsBuyPoints(state,val){
        state.isBuyPoints = val;
    },
};

const getters = {
    getPaymentURL:state => state.paymentURL,
    getIsBuyPoints:state => state.isBuyPoints,
    getStripeToken: () => window.stripe
};

const actions = {
    buyPointsUS({getters}, points) {
        walletService.stripeTransaction(points).then(async ({data}) => {
            const stripePromise = loadStripe(getters.getStripeToken);
            const stripe = await stripePromise;
            //TODO - investigate error
            await stripe.redirectToCheckout({
               sessionId:  data.sessionId,
            });
        })
    },
    async subscribeToTutor({getters}, id) {
       var data =  await walletService.subscribe(id);
       const stripePromise = loadStripe(getters.getStripeToken);
       const stripe = await stripePromise;
           //TODO - investigate error
       await stripe.redirectToCheckout({
           sessionId: data.sessionId,
       });
        //var {data} =  await axios.post(`/Tutor/${id}/subscribe`);
    },
    buyToken({dispatch ,commit}, points) {
        return walletService.buyTokens(points).then(({ data }) => {
            dispatch('updatePaymentLink',data.link)
            commit('setIsBuyPoints',true)
            commit('addComponent',componentConsts.PAYMENT_DIALOG);
        }).catch(() => {
            global.localStorage.setItem("sb_transactionError", points);
            return Promise.reject()
        });
    },
    requestPaymentURL({dispatch,getters}){
        if(getters.getPaymentURL){
            return Promise.resolve()
        }else{
            return walletService.getPaymeLink().then(({ data }) => {
                dispatch('updatePaymentLink',data.link)
                return Promise.resolve()
            }).catch(() => {
                return Promise.reject()
            });
        } 
    },
    signalR_ReleasePaymeStatus({getters,dispatch,commit}){
        let isStudyRoom = getters.getRoomIdSession;
        commit('setIsBuyPoints',false)
        if(isStudyRoom){ // studyroom payment
            let isRoomNeedPayment = getters.getRoomIsNeedPayment;
            if(isRoomNeedPayment){
                dispatch('updateRoomIsNeedPayment',false)
                commit('removeComponent',componentConsts.PAYMENT_DIALOG)
            }
        } else{ // i think calendar payment... have to check
            dispatch('updateNeedPayment',false);
            commit('removeComponent',componentConsts.PAYMENT_DIALOG)
        }
    },
    updatePaymentLink({commit},link){
        commit('setPaymentURL',link);
    },
    updateIsBuyPoints({commit},val){
        commit('setIsBuyPoints',val)
    },
    updatePaypalBuyTokens(context,id){
        return walletService.paypalBuyTokens(id)
    },
    updatePaypalStudyRoom(context,model){
        return walletService.paypalStudyRoom(model)
    },
    getStripeSecret() {
        return walletService.getStripeSecret()
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}