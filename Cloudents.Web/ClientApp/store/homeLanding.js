
import questionService from '../services/questionService'

const state = {
    subjects: questionService.getSubjects().then((response)=>{
        this.subjectList = response.data.map(a => a.subject)
    }),
    selectedSubject: ''
};
const mutations = {
    updateChoosenSubject(state, val){
        state.selectedSubject = val
    }
};
const getters = {
    getSelectedSubject: (state)=> state.selectedSubject,
    getSubjectsList: (state) => state.subjects
};
const actions = {
    updateSubject({commit}, data){
        commit('updateChoosenSubject',  data);
    },


};

export default {
    actions, state, mutations, getters
};