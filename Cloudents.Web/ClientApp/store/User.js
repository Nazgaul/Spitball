//import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
import * as consts from './constants'
import {LanguageService} from '../services/language/languageService';

const state = {
    user: {
        //universityId: null,
        myCourses: [],
        isFirst: true,
        location: null,
        pinnedCards: {}

    },
    cookieAccepted: global.localStorage.getItem("sb-acceptedCookies") === 'true',
    filters: "",
    sort: "",
    historyTermSet: [],
    historySet: {
        ask: [],
        note: [],
        tutor: []
    }
};
const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload };
    },
    [USER.UPDATE_FILTERS](state, payload) {
        state.filters = payload;
    },
    [USER.UPDATE_SORT](state, payload) {
        state.sort = payload;
    },
    [USER.UPDATE_SEARCH_SET_VERTICAL](state, { term, vertical }) {
        if (!term || !term.trim()) return;
        if (!state.historySet[vertical]) { state.historySet[vertical] = []; }
        let currentList = state.historySet[vertical];
        let currentTermIndex = currentList.findIndex(i => i.term === term);
        if (currentTermIndex > -1) {
            currentList = [...currentList.slice(0, currentTermIndex), ...currentList.slice(currentTermIndex, consts.MAX_HISTORY_LENGTH)];
        }
        currentList.push({ term, date: new Date().getTime() });

        //if we reached to the limit drop the first one
        if (currentList.length > consts.MAX_VERTICAL_HISTORY_LENGTH) {
            state.historySet[vertical] = currentList.slice(1);
        }
    },
    [USER.UPDATE_SEARCH_SET](state, term) {
        if (!term || !term.trim()) return;
        let set = new Set(state.historyTermSet);
        if (set.has(term)) {
            set.delete(term);
            state.historyTermSet = [...set];
        }

        if (!state.historyTermSet) state.historyTermSet = [];

        state.historyTermSet.push(term);

        //if we reached to the limit drop the first one
        if (state.historyTermSet.length > consts.MAX_HISTORY_LENGTH) {
            state.historyTermSet = state.historyTermSet.slice(1);
        }
    },

    [USER.ACCEPTED_COOKIE](state){
        state.cookieAccepted = true;
    }
};
const getters = {
    historyTermSet: state => state.historyTermSet,
    allHistorySet: state => {
        let sortedList = [].concat(...Object.values(state.historySet)).sort((a, b) => b.date - a.date).map(i => i.term);
        return sortedList.filter((val, i, arr) => arr.findIndex(b => b === val) === i).slice(0, consts.MAX_HISTORY_LENGTH);
    },
    getVerticalHistory: state => (vertical) => state.historySet[vertical].map(i => i.term).reverse(),
    isFirst: state => state.user.isFirst,
    location: state => {
        let location = state.user.location;
        if (location && location.constructor === String) {
            let [latitude, longitude] = location.split(',');
            return { latitude, longitude };
        } else { return location }
    },
    pinnedCards: state => state.user.pinnedCards,

    getCookieAccepted: (state, getters, {route}) => {
        if(route.name === 'tutoring'){
            console.warn('DEBUG: 3 getCookieAccepted route.name === tutoring')
            return true
        }
        return state.cookieAccepted
    },
    // getRegistrationStep:
    //     state => state.user.registrationStep,
    // getUniversity: state => {
    //     let obj = state.user.universityId || {};
    //     return obj.id;
    // },
    getUniversityName: state => {
        let obj = state.user.universityId || {};
        return obj.name;
    },
    getUniversityImage: state => {
        let obj = state.user.universityId || {};
        return obj.image;
    },
    myCourses: state => state.user.myCourses,
    myCoursesId: state => (state.user.myCourses.length ? state.user.myCourses.map(i => i.id) : []),
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
    updateHistorySet({ commit }, term) {
        commit(USER.UPDATE_SEARCH_SET, term);
    },
    updateHistorySetVertical({ commit }, term) {
        commit(USER.UPDATE_SEARCH_SET_VERTICAL, term);
    },
    updateLocation(context) {
        return new Promise((resolve) => {
            if (!context.getters.location && navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(({ coords }) => {
                    coords = coords || {};
                    context.commit(USER.UPDATE_USER, { location: { latitude: coords.latitude, longitude: coords.longitude } });
                    resolve(context.getters.location);
                }, () => { resolve(context.getters.location) });
            }
            else {
                resolve(context.getters.location);
            }
        });
    },
    updateFirstTime({ commit }, val) {
        let ob = { [val]: false };
        commit(USER.UPDATE_USER, ob);
    },
    updatePinnedCards(context, data) {
        context.commit(USER.UPDATE_USER, { pinnedCards: { ...context.getters.pinnedCards, ...data } });
    },
    updateFilters({ commit }, data) {
        commit(USER.UPDATE_FILTERS, data);
    },
    updateSort({ commit }, data) {
        commit(USER.UPDATE_SORT, data);
    },
    signalR_TutorEnterStudyRoom({ dispatch, rootState }, notificationObj){
        console.warn('DEBUG: 1 signalR_TutorEnterStudyRoom')
        if(rootState.route.name !== 'tutoring' && rootState.route.name !== 'roomSettings'){
            console.warn('DEBUG: 2 route.name !== tutoring && route.name !== roomSettings')
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