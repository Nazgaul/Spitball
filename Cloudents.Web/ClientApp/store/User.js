//import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
import {LanguageService} from '../services/language/languageService';

const state = {
    user: {
        location: null,
    },
    cookieAccepted: global.localStorage.getItem("sb-acceptedCookies") === 'true',
    filters: "",
    sort: "",
};
const mutations = {
    [USER.UPDATE_FILTERS](state, payload) {
        state.filters = payload;
    },
    [USER.UPDATE_SORT](state, payload) {
        state.sort = payload;
    },
    [USER.ACCEPTED_COOKIE](state){
        state.cookieAccepted = true;
    }
};
const getters = {
    getCookieAccepted: (state, getters, {route}) => {
        if(route.name === 'tutoring'){
            return true
        }
        return state.cookieAccepted
    },
    getFilters (state) {
      return  state.filters;
    },
    getSort(state){
        return state.sort;
    }
};
const actions = {
    setCookieAccepted({ commit }){
        global.localStorage.setItem("sb-acceptedCookies", true);
        commit(USER.ACCEPTED_COOKIE);
    },
    updateFilters({ commit }, data) {
        commit(USER.UPDATE_FILTERS, data);
    },
    updateSort({ commit }, data) {
        commit(USER.UPDATE_SORT, data);
    },
    signalR_TutorEnterStudyRoom({ dispatch, rootState }, notificationObj){
        if(rootState.route.name !== 'tutoring' && rootState.route.name !== 'roomSettings'){
            let id = notificationObj.studyRoomId;
            let userName = notificationObj.userName;
            let location = global.location.origin;
            let textLang = LanguageService.getValueByKey('tutor_waiting_in_studyRoom');
            let textLangStudyRoom = LanguageService.getValueByKey('tutor_waiting_in_studyRoom_link');
            let textElm = `<a style="text-decoration: none;" href="${location}/studyroomSettings/${id}">
                <span>${userName} ${textLang}</span> <span style="text-decoration: underline;">${textLangStudyRoom}</span> 
            </a>`;
            let toasterObj = {
                toasterText: textElm,
                showToaster: true,
                toasterType: '',
                toasterTimeout: 3600000
            };
            dispatch('updateToasterParams', toasterObj);
        }
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}