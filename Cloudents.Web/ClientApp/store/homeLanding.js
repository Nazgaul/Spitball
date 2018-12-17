import questionService from '../services/questionService'

const state = {
    subjects: [],
    selectedSubject: ''
};
const mutations = {
    updateChoosenSubject(state, val) {
        state.selectedSubject = val
    },
    getAllSubjects(state, data) {
        state.subjects = data
    }
};
const getters = {
    getSelectedSubject: (state) => state.selectedSubject,
    getSubjectsList: (state) => state.subjects
};
const actions = {
    updateSubject({commit}, data) {
        commit('updateChoosenSubject', data);
    },
    getAllSubjects({commit}) {
        questionService.getSubjects()
            .then((response) => {
                let data = response.data.map(a => a.subject);
                commit('getAllSubjects', data);
            });

    }


};

export default {
    actions, state, mutations, getters
};