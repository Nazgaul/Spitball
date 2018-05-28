﻿import settingsService from './../services/settingsService'
import { SEARCH, USER } from './mutation-types'
import * as consts from "./constants";
// export const MAX_HISTORY_LENGTH=5;
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true,
        location: null,
        pinnedCards: {},
        showSmartAppBanner: true,
        registrationStep: 0
    },
    facet: "",
    historyTermSet: [],
    historySet: {
        job: [],
        flashcard: [],
        ask: [],
        note: [],
        food: [],
        tutor: [],
        book: []
    }
};
const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload };
    },
    [USER.UPDATE_FACET](state, payload) {
        state.facet = payload;
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
    [USER.HIDE_SMART_BANNER](state) {
        state.user.showSmartAppBanner = false;
    },
    [USER.UPDATE_REGISTRATION_STEP](state, step) {
        state.user.registrationStep = step;
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
            return { latitude, longitude }
        } else { return location }
    },
    pinnedCards:
        state => state.user.pinnedCards,
    showSmartAppBanner:
        state => state.user.showSmartAppBanner,
    getRegistrationStep:
        state => state.user.registrationStep,
    getUniversity: state => {
        let obj = state.user.universityId || {};
        return obj.id;
    },
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
    getFacet: state => state.facet
};
const actions = {
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
    getUniversities(context, data) {
        return context.dispatch("updateLocation").then((location) => {
            return settingsService.getUniversity({ term: data.term, location });
        });
    },
    getCourses(context, { term }) {
        return settingsService.getCourse({ term, universityId: context.getters.getUniversity });
    },

    createCourse(context, { name }) {
        const university = context.getters.getUniversity;
        return settingsService.createCourse({ courseName: name, university }).then(({ data: body }) => {
            context.commit(USER.UPDATE_USER, { myCourses: [...context.getters.myCourses, { id: body.id, name: name }] });
        });
    },
    updateFirstTime({ commit }, val) {
        let ob = { [val]: false };
        commit(USER.UPDATE_USER, ob);
    },
    updateUniversity({ commit }, { id, name, image }) {
        commit(USER.UPDATE_USER, { universityId: { id, name, image } });
    },
    updatePinnedCards(context, data) {
        context.commit(USER.UPDATE_USER, { pinnedCards: { ...context.getters.pinnedCards, ...data } });
    },
    updateFacet({ commit }, data) {
        commit(USER.UPDATE_FACET, data)
    },
    hideSmartAppBanner({ commit }, data) {
        commit(USER.HIDE_SMART_BANNER);
    },
    updateRegistrationStep(context, data) {
        context.commit(USER.UPDATE_REGISTRATION_STEP, data);
    }

};
export default {
    state,
    mutations,
    getters,
    actions
}