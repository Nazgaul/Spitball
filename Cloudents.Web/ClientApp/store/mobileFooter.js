const state = {
    mobileFooterState: true,//defined on the meta object of the routes
    stater: 'feed',
    statesEnum: {
        feed: 'feed',
        earners: 'earners',
        promotions: 'promotions'
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
    getIsFeedTabActive: state => {
        return state.stater === state.statesEnum['feed'];
    },
    getFooterEnumsState: state => state.statesEnum,
    showMarketingBox: (state) => {
        return state.stater === state.statesEnum['promotions'];
    },
    showLeaderBoard: (state) => {
        return state.stater === state.statesEnum['earners'];
    },
    showMobileFeed: (state) => {
        return state.stater === state.statesEnum['feed'];
    },
    getCurrentActiveTabName:(state)=>{
        return state.stater;
    }

};

const mutations = {
    changeStater(state, val) {
        state.stater = val;
    }
};

const actions = {
    changeFooterActiveTab({commit, state}, stateEnum) {
        if (state.statesEnum[stateEnum]) {
            commit('changeStater', state.statesEnum[stateEnum]);
        }
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}