import accountService from '../services/accountService';

const state = {
    becomeTutorObj: {
        image: '',
        firstName: '',
        lastName: '',
        price: 50,
        description: '',
        bio: ''
    }
};

const getters = {
    becomeTutorData: state => state.becomeTutorObj,
};
const mutations = {
    assignFields(state, val) {
        state.becomeTutorObj = {...state.becomeTutorObj, ...val};
    }
};

const actions = {
    updateTutorInfo({commit}, val) {
        commit('assignFields', val);
    },
    sendBecomeTutorData({state,dispatch}) {
        return accountService.becomeTutor(state.becomeTutorObj).then((res=>{
            dispatch('updateSelectedCalendarList');
            dispatch('updateAvailabilityCalendar');
            return Promise.resolve(res);
        }));
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};