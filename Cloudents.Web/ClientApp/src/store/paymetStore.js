import walletService from '../services/walletService.js';

import * as componentConsts from '../components/pages/global/toasterInjection/componentConsts.js'

const state = {
    paymentURL: null,
};

const mutations = {
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
};

const getters = {
    getPaymentURL:state => state.paymentURL,
    getStripeToken: () => window.stripe
};

const actions = {
    async subscribeToTutor({getters}, id) {
        let data = await walletService.subscribe(id);
        const stripePromise = window.Stripe(getters.getStripeToken);
        const stripe = await stripePromise;

        await stripe.redirectToCheckout({
            sessionId: data.sessionId,
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
    // updatePaypalBuyTokens(context,id){
    //     return walletService.paypalBuyTokens(id)
    // },
    // updatePaypalStudyRoom(context,model){
    //     return walletService.paypalStudyRoom(model)
    // },
    // getStripeSecret() {
    //     return walletService.getStripeSecret()
    // }
    goStripe({getters},x){
        this._vm.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false").then(async() => {
            const stripePromise = window.Stripe(getters.getStripeToken);
            const stripe = await stripePromise;
            await stripe.redirectToCheckout({
                sessionId: x,
            });
        })
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}