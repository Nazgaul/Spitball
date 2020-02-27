import walletService from '../services/walletService.js';
import { LanguageService } from '../services/language/languageService';
import { router } from '../main.js';

const state = {
    paymentURL: null,
    transactionId: null,
};

const mutations = {
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
    setIdTransaction(state, id) {
        state.transactionId = id;
    },
};

const getters = {
    getPaymentURL:state => state.paymentURL,
    getTransactionId: state => state.transactionId,
};

const actions = {
    buyToken({dispatch}, points) {
        walletService.buyTokens(points).then(({ data }) => {
            dispatch('updatePaymentLink',data.link)
            router.push({query:{...router.currentRoute.query,dialog:'payment'}})
        }).catch(() => {
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("buyTokens_failed_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
            global.localStorage.setItem("sb_transactionError", points);
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
                dispatch('updateToasterParams', {
                    toasterText: LanguageService.getValueByKey("buyTokens_failed_transaction"),
                    showToaster: true,
                    toasterTimeout: 5000
                });
                return Promise.reject()
            });
        } 
    },
    updateIdTransaction({commit}, id) {
        commit('setIdTransaction', id);
    },
    signalR_ReleasePaymeStatus({getters,dispatch}){
        let isStudyRoom = getters.getStudyRoomData;
        if(!!isStudyRoom){
            dispatch('releasePaymeStatus_studyRoom');
        } else{
            dispatch('updateNeedPayment',false);
            router.push({query:{...router.currentRoute.query,dialog:undefined}})
        }
    },
    updatePaymentLink({commit},link){
        commit('setPaymentURL',link);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}