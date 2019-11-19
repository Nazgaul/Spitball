﻿import { LUIS } from "./mutation-types"

//BLITZ WTF
let luisTypes = ["job", "ask"];
let ACADEMIC = "academic";
const state = {
    job: {},
    ask: {},
    academic: {},
    currentText: "",
    currentVertical: "",
    filterCourses: []
};
let getLuisType = (type) => luisTypes.includes(type) ? type : ACADEMIC;
const mutations = {
    [LUIS.UPDATE_TERM](state, {vertical, data}) {
        state[getLuisType(vertical)] = data;
        state.currentVertical = vertical;
        state.currentText = data.text;
    },
    [LUIS.UPDATE_FILTER_COURSES](state, data) {
        state.filterCourses = data;
    },
    [LUIS.UPDATE_CURRENT_VERTICAL](state, data) {
        state.currentVertical = data;
        state.currentText = state[getLuisType(data)] ? state[getLuisType(data)].text : "";
    },
    [LUIS.CLEAN_DATA](state) {
        for (const prop of Object.keys(state)) {
            state[prop] = "";
        }
    }
};

const getters = {
    getVerticalData: state => (vertical) => state[getLuisType(vertical)] || {},
    currentText: state => state.currentText,
    // getCurrentVertical is used in the search store
    getCurrentVertical: state => state.currentVertical
};
const actions = {
    setFilteredCourses({commit}, data) {
        commit(LUIS.UPDATE_FILTER_COURSES, data);
    },
    setCurrentVertical({commit}, data) {
        commit(LUIS.UPDATE_CURRENT_VERTICAL, data);
    },
    cleanData({commit}) {
        commit(LUIS.CLEAN_DATA);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
