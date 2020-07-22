const state = {
    mobileFooterState: true,
    stater: 'feed',
    statesEnum: {
        feed: 'feed',
        tutors: 'tutors',
        profile: 'profile',
    }
};
const getters = {
    getMobileFooterState: (state, val, {route}) => {
        return !!route.meta && route.meta.hasOwnProperty('showMobileFooter');
    },

};

export default {
    state,
    getters,
}