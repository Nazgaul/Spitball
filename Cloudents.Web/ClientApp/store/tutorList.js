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
    }
};

const actions = {
    getTutorList({commit, state}, objReq) {
        tutorListService.getTutorList(objReq)
                        .then((tutors) => {
                            commit('setTutors', tutors);
                        });
    }

};
export default {
    state,
    mutations,
    getters,
    actions
};