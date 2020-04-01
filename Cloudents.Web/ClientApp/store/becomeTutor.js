import accountService from '../services/accountService';

const state = {
    tutorMinPrice: 35,
    becomeTutorObj: {
        image: '',
        firstName: '',
        lastName: '',
        price: 35,
        description: '',
        bio: ''
    },
};

const getters = {
    becomeTutorData: state => state.becomeTutorObj,
    getTutorMinPrice: state => state.tutorMinPrice,
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