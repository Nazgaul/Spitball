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
        if(!!route.meta && route.meta.hasOwnProperty('showMobileFooter') ){
            return true;
        }else{
            return false;
        }
    },

};

export default {
    state,
    getters,
}