import tutorListService from "../services/tutorListService";

const state = {
    tutorList: [],
};
const getters = {
    tutorList: state => state.tutorList,
};

const mutations = {
    setTutors(state, tutors) {
        state.tutorList = tutors;
    },
    resetTutorList(state, val) {
        state.tutorList = val;
    }
};

const actions = {
    getTutorList({commit}, objReq) {
        commit('setTutors', []);
        tutorListService.getTutorList(objReq)
                        .then((tutors) => {
                            commit('setTutors', tutors);
                        });
    },
    resetList({commit}){
        commit('resetTutorList', []);
    }

};
export default {
    state,
    mutations,
    getters,
    actions
};