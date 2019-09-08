import accountService from '../services/accountService';

const state = {
    becomeTutorObj: {
        image: '',
        firstName: '',
        lastName: '',
        price: 50,
        description: '',
        bio: ''
    },
    tutorDialog: false,
};

const getters = {
    becomeTutorData: state => state.becomeTutorObj,
    becomeTutorDialog: state => state.tutorDialog,
}
const mutations = {
    changeDialogState(state, val) {
        state.tutorDialog = val;
    },
    assignFields(state, val) {
        state.becomeTutorObj = {...state.becomeTutorObj, ...val};
    }
};

const actions = {
    updateTutorDialog({commit, state}, val) {
        commit('changeDialogState', val);
    },
    updateTutorInfo({commit}, val) {
        commit('assignFields', val);
    },
    sendBecomeTutorData({state,dispatch}) {
        return accountService.becomeTutor(state.becomeTutorObj).then((res=>{
            dispatch('updateSelectedCalendarList')
            return Promise.resolve(res)
        }));
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};