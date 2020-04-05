import accountService from '../services/accountService';

const TUTOR_MIN_PRICE = 35;

const state = {
    tutorMinPrice: TUTOR_MIN_PRICE,
    becomeTutorObj: {
        image: '',
        firstName: '',
        lastName: '',
        price: TUTOR_MIN_PRICE,
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