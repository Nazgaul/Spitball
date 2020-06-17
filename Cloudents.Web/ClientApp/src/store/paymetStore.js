import walletService from '../services/walletService.js';
import * as dialogNames from '../components/pages/global/dialogInjection/dialogNames.js'

import { router } from '../main.js';

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
            const stripePromise = window.Stripe(getters.getStripeToken);
            const stripe = await stripePromise;
            await stripe.redirectToCheckout({
               sessionId:  data.sessionId,
            });
        })
    },
    async subscribeToTutor({getters}, id) {
        let data = await walletService.subscribe(id);
        const stripePromise = window.Stripe(getters.getStripeToken);
        const stripe = await stripePromise;

        await stripe.redirectToCheckout({
            sessionId: data.sessionId,
        });
    },
    buyToken({dispatch ,commit}, points) {
        return walletService.buyTokens(points).then(({ data }) => {
            dispatch('updatePaymentLink',data.link)
            commit('setIsBuyPoints',true)
            router.push({query:{...router.currentRoute.query,dialog: dialogNames.Payment}})
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
        if(isStudyRoom){
            let isRoomNeedPayment = getters.getRoomIsNeedPayment;
            if(isRoomNeedPayment){
                dispatch('updateRoomIsNeedPayment',false)
                router.push({query:{...router.currentRoute.query,dialog:undefined}})
            }
        } else{
            dispatch('updateNeedPayment',false);
            router.push({query:{...router.currentRoute.query,dialog:undefined}})
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