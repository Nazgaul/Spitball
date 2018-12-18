import questionService from '../services/questionService'
import homeLandingService from '../services/homeLandingService';
const state = {
    subjects: [],
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
    }
};
const getters = {
    getSelectedSubject: (state) => state.selectedSubject,
    getSubjectsList: (state) => state.subjects,
    statistics: (state) => state.statistics

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