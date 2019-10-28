import walletService from '../services/walletService.js';
import { LanguageService } from '../services/language/languageService';

const state = {
    showPaymentDialog: false,
    paymentURL: '',
    tutorName: '',
    transactionId: null,
    dictionaryTitle: '',
};

const mutations = {
    setTutorName(state, name) {
        state.tutorName = name;
    },
    setPaymentDialogState(state,val){
        state.showPaymentDialog = val;
    },
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
    setIdTransaction(state, id) {
        state.transactionId = id;
    },
     setDictionaryTitle(state, val) {
         state.dictionaryTitle = val;
     }
};

const getters = {
    getTutorName: state => state.tutorName,
    getDictionaryTitle: state => state.dictionaryTitle,
    getShowPaymeDialog: state => state.showPaymentDialog,
    getPaymentURL:state => state.paymentURL,
    getTransactionId: state => state.transactionId,
};

const actions = {
    buyToken({commit, dispatch, getters}, points) {
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
    requestPaymentURL({commit,dispatch, getters}, paymeObj ){
        dispatch('updateTutorName', paymeObj.name);
        dispatch('updateDictionaryTitle', paymeObj.title);
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
    updateDictionaryTitle({commit}, title) {
        commit('setDictionaryTitle', title);
    },
    updateTutorName({commit}, name) {
        commit('setTutorName', name);
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