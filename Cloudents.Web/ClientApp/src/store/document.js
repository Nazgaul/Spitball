import documentService from "../services/documentService";
import analyticsService from '../services/analytics.service';
import { router } from '../main.js';
import {ITEM_DIALOG} from '../components/pages/global/toasterInjection/componentConsts'

const state = {
    document: {},
    btnLoading: false,
    showPurchaseConfirmation: false,
    documentLoaded: false,
    currentItemId: null,
    currentPage: 0
};

const getters = {
    _getDocumentLoaded: state => {
        let x = state.document?.id ? state.document : '';
        return typeof (x) !== "string";
    },
    getDocumentDetails: state => state.document,
    getDocumentName: (state,_getter)=>  {
        if (_getter._getDocumentLoaded) {
            return  state.document.title;
        }
        return ''
    },
    getDocumentPrice: (state,_getter) => {
        if (_getter._getDocumentLoaded) {
            return  state.document.price;
        }
        return 0
    },
    getIsPurchased: (state,_getter) => {
        return state.document?.isPurchased || _getter.getDocumentPrice === 0
    },
    getBtnLoading: (state, _getter) => {
        if (_getter._getDocumentLoaded) {
            return state.btnLoading
        }
        return false
    },
    getPurchaseConfirmation: state => state.showPurchaseConfirmation,
    getDocumentLoaded: state => state.documentLoaded,
    getDocumentPriceTypeFree: state => state.document?.priceType === 'Free',
    getDocumentPriceTypeSubscriber: state => state.document?.priceType === 'Subscriber',
    getDocumentPriceTypeHasPrice: state => state.document?.priceType === 'HasPrice',
    getDocumentUserName: state => state.document?.userName,
    getCurrentItemId: state => state.currentItemId,
    getCurrentPage: state => state.currentPage,
};

const mutations = {
    resetState(state){
        state.document = {};
        state.btnLoading = false;    
        state.showPurchaseConfirmation = false;
        state.documentLoaded = false;
    },
    setPurchaseConfirmation(state,val){
        state.showPurchaseConfirmation = val;
    },
    setDocument(state, payload) {
        state.document = payload;    
        state.documentLoaded = true;    
    },
    setBtnLoading(state, payload) {
        state.btnLoading = payload;
    },
    setCurrentItemId(state,itemId){
        state.currentItemId = itemId
    },
    setItemPage(state,page){
        state.currentPage = page;
    }
};

const actions = {
    updatePurchaseConfirmation({commit},val){
        commit('setPurchaseConfirmation',val);
    },
    documentRequest({commit}, id) {
        return documentService.getDocument(id).then((DocumentObj) => {
            commit('setDocument', DocumentObj);
            return;
        });
    },
    downloadDocument({getters}, item) {
        let user = getters.accountUser;
        if(!user) return router.push({query:{...router.currentRoute.query,dialog:'login'}});

        let {id} = item;     

        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_DOWNLOAD', `USER_ID: ${user.id}, DOC_ID: ${id}`);
    },
    purchaseDocument({commit, dispatch, state, getters}, item) {
        let cantBuyItem = getters.accountUser.balance < item.price;

        if(cantBuyItem) {
            return
        }

        commit('setBtnLoading', true);
            return documentService.purchaseDocument(item.id).then((resp) => {
                state.document.isPurchased = true;
                console.log('purchased success', resp);
                analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_PURCHASED', item.price);
                dispatch('documentRequest', item.id);
                },
                () => {
                    return Promise.reject()
            }).finally(() => {
                setTimeout(() => {
                    commit('setBtnLoading', false);
                }, 500);
            });
    },
    clearDocument({commit}){
        commit('resetState');
    },


    // new ITEM thing:
    updateCurrentItem({commit},itemId){
        if(itemId){
            commit('setCurrentItemId',itemId);
            commit('addComponent',ITEM_DIALOG);
        }else{
            commit('setCurrentItemId',null);
            commit('removeComponent',ITEM_DIALOG);
        }
    },
    updateItemPaging({commit,getters},isNext){
        let page;
        if(isNext){
            if(getters.getCurrentPage < getters.getDocumentDetails?.preview?.length -1){
                page = getters.getCurrentPage +1;
            }
        }else{
            page = getters.getCurrentPage -1;
        }
        commit('setItemPage',page)
    }
};

export default {
    state,
    getters,
    mutations,
    actions
};