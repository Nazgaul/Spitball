import documentService from "../services/documentService";
import analyticsService from '../services/analytics.service';
import searchService from '../services/searchService';
import { LanguageService } from "../services/language/languageService";
import { Promise } from "q";

const state = {
    document: {},
    tutorList: [],
    btnLoading: false,
};

const getters = {
    getDocumentDetails: state => state.document,
    getTutorList: state => state.tutorList,
    getBtnLoading: state => state.btnLoading
};

const mutations = {
    setDocument(state, payload) {
        state.document = payload;        
    },
    clearDocumentItem(state) {
        state.document = {};
    },
    setTutorsList(state, payload) {
        state.tutorList = payload;
    },
    setNewDocumentPrice(state, price){
        state.document.details.price = price;
    },
    setBtnLoading(state, payload) {
        state.btnLoading = payload
    }
};

const actions = {
    documentRequest({commit}, id) {
        return documentService.getDocument(id).then((DocumentObj) => {
            commit('setDocument', DocumentObj)
            return true
        }, (err) => {
            return err
        })
    },
    downloadDocument({commit, getters, dispatch}, item) {
        let user = getters.accountUser
        
        if(!user) return dispatch('updateLoginDialogState', true)

        let {id, course, url} = item;     
        
        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_DOWNLOAD', `USER_ID: ${user.id}, DOC_ID: ${id}, DOC_COURSE:${course}`);
        global.location.href = url;
    },
    purchaseDocument({commit, getters, dispatch}, item) {
        commit('setBtnLoading', true);
        let userBalance = 0;
        let id = item.id ? item.id : '';
        if(!!getters.accountUser && getters.accountUser.balance){
            userBalance = getters.accountUser.balance
        }
        
        if(userBalance >= item.price) {
            return documentService.purchaseDocument(item.id).then((resp) => {
                console.log('purchased success', resp);
                analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_PURCHASED', item.price);
                dispatch('documentRequest', item.id);
                },
                (error) => {
                    console.log('purchased Error', error);
            }).finally(() => {
                setTimeout(() => {
                    commit('setBtnLoading', false);
                }, 500)
            })
        } else {
            commit('setBtnLoading', false);
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("resultNote_unsufficient_fund"),
                showToaster: true,
            });
        }
    },
    getTutorListCourse({ commit }, payload) {
        searchService.activateFunction.getTutors(payload).then(res => {
            commit('setTutorsList', res)
        })
    },
    clearDocument({commit}) {
        commit('clearDocumentItem')
    },
    setNewDocumentPrice({ commit }, price) {
        if(!!state.document && !!state.document.details){
            commit('setNewDocumentPrice', price);
        }
    }
};

export default {
    state,
    getters,
    mutations,
    actions
};