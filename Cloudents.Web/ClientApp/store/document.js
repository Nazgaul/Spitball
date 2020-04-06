import documentService from "../services/documentService";
import analyticsService from '../services/analytics.service';
import { LanguageService } from "../services/language/languageService";
import { router } from '../main.js';

const state = {
    document: {},
    itemsList:[],
    btnLoading: false,
    showPurchaseConfirmation: false,
    documentLoaded: false,
    toaster: false,
};

const getters = {
    _getDocumentLoaded: state => {
        let x = state.document.details || '';
        if (typeof(x) === "string") {
            return false;
        }
        return true;
    },
    getShowItemToaster: state => state.toaster,
    getDocumentDetails: state => state.document,
    getDocumentName: (state,_getter)=>  {
        if (_getter._getDocumentLoaded) {
            return  state.document.details.feedItem.title;
        }
        return ''
    },
    getDocumentPrice: (state,_getter) => {
        if (_getter._getDocumentLoaded) {
            return  state.document.details.price;
        }
        return 0
    },
    getIsPurchased: (state,_getter) => {
        if (_getter._getDocumentLoaded) {
            return  false
        }
        state.document.details?.isPurchased || _getter.getDocumentPrice === 0
    },
    getBtnLoading: (state, _getter) => {
        if (_getter._getDocumentLoaded) {
            return state.btnLoading
        }
        return false
    },
    getPurchaseConfirmation: state => state.showPurchaseConfirmation,
    getDocumentLoaded: state => state.documentLoaded,
    getRelatedDocuments: state => state.itemsList,
};

const mutations = {
    resetState(state){
        state.document = {};
        state.itemsList = [];
        state.btnLoading = false;    
        state.showPurchaseConfirmation = false;
        state.documentLoaded = false;
        state.toaster = false;
    },
    setPurchaseConfirmation(state,val){
        state.showPurchaseConfirmation = val;
    },
    setDocument(state, payload) {
        state.document = payload;    
        state.documentLoaded = true;    
    },
    setRelatedDocs(state, payload) {
        state.itemsList = payload;
    },
    setNewDocumentPrice(state, price){
        state.document.details.price = price;
    },
    setBtnLoading(state, payload) {
        state.btnLoading = payload;
    },
    setShowItemToaster(state, val) {
        state.toaster = val
    }
};

const actions = {
    updatePurchaseConfirmation({commit},val){
        commit('setPurchaseConfirmation',val);
    },
    documentRequest({commit}, id) {
        return documentService.getDocument(id).then((DocumentObj) => {
            commit('setDocument', DocumentObj);
            return true;
        }, (err) => {
            return err;
        });
    },
    downloadDocument({getters}, item) {
        let user = getters.accountUser;

        if(!user) return router.push({query:{...router.currentRoute.query,dialog:'login'}});

        let {id, course} = item;     

        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_DOWNLOAD', `USER_ID: ${user.id}, DOC_ID: ${id}, DOC_COURSE:${course}`);
    },
    purchaseDocument({commit, dispatch, state, getters}, item) {
        let cantBuyItem = getters.accountUser.balance < item.price;

        if(cantBuyItem) {
            dispatch('updateItemToaster', true);
            return
        }

        commit('setBtnLoading', true);
            return documentService.purchaseDocument(item.id).then((resp) => {
                state.document.isPurchased = true;
                console.log('purchased success', resp);
                analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_PURCHASED', item.price);
                dispatch('documentRequest', item.id);
                },
                (error) => {
                    console.log('purchased Error', error);
                    dispatch('updateToasterParams', {
                        toasterText: LanguageService.getValueByKey("resultNote_unsufficient_fund"),
                        showToaster: true,
                    });
            }).finally(() => {
                setTimeout(() => {
                    commit('setBtnLoading', false);
                }, 500);
            });
    },
    getStudyDocuments({commit}, {course,id}) {
        documentService.getStudyDocuments({course, documentId: id}).then(items => {
            commit('setRelatedDocs', items);
        });
    },
    setNewDocumentPrice({ commit }, price) {
        if(!!state.document && !!state.document.details){
            commit('setNewDocumentPrice', price);
        }
    },
    clearDocument({commit}){
        commit('resetState');
    },
    updateItemToaster({commit}, val){
        commit('setShowItemToaster', val);
    },
};

export default {
    state,
    getters,
    mutations,
    actions
};