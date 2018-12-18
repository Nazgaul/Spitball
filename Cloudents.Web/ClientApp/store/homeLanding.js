import questionService from '../services/questionService'
import homeLandingService from '../services/homeLandingService';
const state = {
    subjects: [],
    dictionaryPrefixEnum:{
        learn: 'learn',
        earn: 'earn'
    },
    dictionaryPrefix: 'earn',
    selectedSubject: '',
    statistics : []
};
const mutations = {
    updateChoosenSubject(state, val) {
        state.selectedSubject = val
    },
    getAllSubjects(state, data) {
        state.subjects = data
    },
    updateStatisticsData(state, data){
        state.statistics = data
    },
    changeDictionaryPrefix(state, val){
        if(!!state.dictionaryPrefixEnum[val]){
            state.dictionaryPrefix = state.dictionaryPrefixEnum[val];
        }
    }
};
const getters = {
    getSelectedSubject: (state) => state.selectedSubject,
    statistics: (state) => state.statistics,
    getSubjectsList: (state) => state.subjects,
    getDictionaryPrefix: (state) => state.dictionaryPrefix,
    getDictionaryPrefixEnum: (state) => state.dictionaryPrefixEnum,
};
const actions = {
    updateSubject({commit}, data) {
        commit('updateChoosenSubject', data);
    },
    getAllSubjects({commit}) {
        questionService.getSubjects()
            .then((response) => {
                let data = response.data;
                commit('getAllSubjects', data);
            });

    },
    switchLandingPageText({commit}, val){
        commit('changeDictionaryPrefix', val)
    },
    getStatistics({commit}) {
        homeLandingService.getStatistics()
            .then((response) => {
                let data = homeLandingService.createStatisticsData(response.data);
                commit('updateStatisticsData', data);
            });
    }
};

export default {
    actions, state, mutations, getters
};