import walletService from '../services/walletService.js';
import { LanguageService } from '../services/language/languageService';

const state = {
    showPaymentDialog: false,
    paymentURL: '',
    transactionId: null,
};

const mutations = {
    setPaymentDialogState(state,val){
        state.showPaymentDialog = val;
    },
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
    setIdTransaction(state, id) {
        state.transactionId = id;
    },
};

const getters = {
    getShowPaymeDialog: state => state.showPaymentDialog,
    getPaymentURL:state => state.paymentURL,
    getTransactionId: state => state.transactionId,
};

const actions = {
    buyToken({commit, dispatch}, points) {
        walletService.buyTokens(points).then(({ data }) => {
            commit('setPaymentURL',data.link);
            dispatch('updatePaymentDialogState',true);
        }).catch(() => {
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("buyTokens_failed_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
            dispatch('updatePaymentDialogState',false);
            global.localStorage.setItem("sb_transactionError", points);
        });
    },
    requestPaymentURL({commit,dispatch}){
        walletService.getPaymeLink().then(({ data }) => {
            commit('setPaymentURL',data.link);
            dispatch('updatePaymentDialogState',true);
        }).catch(() => {
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("buyTokens_failed_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
            dispatch('updatePaymentDialogState',false);
        });
    },
    updatePaymentDialogState({commit}, val){
        commit('setPaymentDialogState', val);
    },
    updateIdTransaction({commit}, id) {
        commit('setIdTransaction', id);
    },
    signalR_ReleasePaymeStatus({getters,dispatch}){
        let isStudyRoom = getters.getStudyRoomData;
        if(!!isStudyRoom){
            dispatch('releasePaymeStatus_studyRoom');
        } else{
            dispatch('updatePaymentDialogState',false);
            dispatch('updateNeedPayment',false);
        }
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}