import questionService from '../services/questionService'

const state = {
    subjects: [],
    selectedSubject: '',
    dictionaryPrefixEnum:{
        learn: 'learn',
        earn: 'earn'
    },
    dictionaryPrefix: 'earn',
};
const mutations = {
    updateChoosenSubject(state, val) {
        state.selectedSubject = val
    },
    getAllSubjects(state, data) {
        state.subjects = data
    },
    changeDictionaryPrefix(state, val){
        if(!!state.dictionaryPrefixEnum[val]){
            state.dictionaryPrefix = state.dictionaryPrefixEnum[val];
        }
    }
};
const getters = {
    getSelectedSubject: (state) => state.selectedSubject,
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
                // let data = response.data.map(a => a.subject);
                let data = response.data;

                commit('getAllSubjects', data);
            });

    },
    switchLandingPageText({commit}, val){
        commit('changeDictionaryPrefix', val)
    }


};

export default {
    actions, state, mutations, getters
};