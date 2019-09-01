import walletService from '../services/walletService.js'

const state = {
    showPaymentDialog: false,
    paymentURL: '',
}

const mutations ={
    setPaymentDialog(state,val){
        state.showPaymentDialog = val
    },
    setPaymentURL(state,url){
        state.paymentURL = url
    },
}

const getters ={
    getShowPaymentDialog:state => state.showPaymentDialog,
    getPaymentURL:state => state.paymentURL,
}

const actions ={
    requestPaymentURL({commit,dispatch}){
        walletService.getPaymeLink().then(({ data }) => {
            commit('setPaymentURL',data.link)
            dispatch('updatePaymentDialog',true)
        });
    },
    updatePaymentDialog({commit},val){
        commit('setPaymentDialog',val)
    },
    signalR_ReleasePaymeStatus({getters,dispatch}){
        let isStudyRoom = getters.getStudyRoomData
        if(!!isStudyRoom){
            dispatch('releasePaymeStatus_studyRoom')
        } else{
            dispatch('updatePaymentDialog',false)
            dispatch('updateNeedPayment',false)
        }
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}