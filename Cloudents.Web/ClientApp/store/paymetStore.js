import walletService from '../services/walletService.js';
import { LanguageService } from '../services/language/languageService';
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
};

const actions = {
    buyToken({dispatch ,commit}, points) {
        walletService.buyTokens(points).then(({ data }) => {
            dispatch('updatePaymentLink',data.link)
            commit('setIsBuyPoints',true)
            router.push({query:{...router.currentRoute.query,dialog: dialogNames.Payment}})
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
    signalR_ReleasePaymeStatus({getters,dispatch,commit}){
        let isStudyRoom = getters.getStudyRoomData;
        commit('setIsBuyPoints',false)
        if(!!isStudyRoom){
            dispatch('releasePaymeStatus_studyRoom');
            router.push({query:{...router.currentRoute.query,dialog:undefined}})
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
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}