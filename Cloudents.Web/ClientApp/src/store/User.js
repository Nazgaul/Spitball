﻿import { USER } from './mutation-types'
import { i18n } from '../plugins/t-i18n'
const state = {
    cookieAccepted: global.localStorage.getItem("sb-acceptedCookies") === 'true',
};
const mutations = {
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
};
const actions = {
    setCookieAccepted({ commit }){
        global.localStorage.setItem("sb-acceptedCookies", true);
        commit(USER.ACCEPTED_COOKIE);
    },
    signalR_TutorEnterStudyRoom({ dispatch, rootState }, notificationObj){
        // TODO: clean reference to roomSetting (old studyRoomSetting)
        // TODO: check if we need it;
        if(rootState.route.name !== 'tutoring' && rootState.route.name !== 'roomSettings'){
            let id = notificationObj.studyRoomId;
            let userName = notificationObj.userName;
            let location = global.location.origin;
            let textLang = i18n.t('tutor_waiting_in_studyRoom');
            let textLangStudyRoom = i18n.t('tutor_waiting_in_studyRoom_link');
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