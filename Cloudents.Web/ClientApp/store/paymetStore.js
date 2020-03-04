import walletService from '../services/walletService.js';
import { LanguageService } from '../services/language/languageService';
import { router } from '../main.js';

const state = {
    paymentURL: '',
    tutorName: '',
    transactionId: null,
};

const mutations = {
    setTutorName(state, name) {
        state.tutorName = name;
    },
    setPaymentURL(state,url){
        state.paymentURL = url;
    },
    setIdTransaction(state, id) {
        state.transactionId = id;
    },
};

const getters = {
    getTutorName: state => state.tutorName,
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
    requestPaymentURL({commit,dispatch}, paymeObj ){
        dispatch('updateTutorName', paymeObj.name);
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
    updatePaymentDialogState(context, val){
        if(val){
            router.push({query:{...router.currentRoute.query,dialog:'payment'}})
        }else{
            router.push({query:{...router.currentRoute.query,dialog:undefined}})
        }
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