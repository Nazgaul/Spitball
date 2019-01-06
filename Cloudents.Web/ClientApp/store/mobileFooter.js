const state = {
    mobileFooterState: true,
    stater: 'feed',
    statesEnum: {
        feed: 'feed',
        earners: 'earners',
        promotions: 'promotions',
    }
};
const getters = {
    getMobileFooterState: state => state.mobileFooterState,
    getFooterEnumsState: state => state.statesEnum,
    showMarketingBox: (state) => {
        return state.stater === state.statesEnum['promotions']
    },
    showLeaderBoard: (state) => {
        return state.stater === state.statesEnum['earners']
    },
    showMobileFeed: (state) => {
        return state.stater === state.statesEnum['feed']
    }

};

const mutations = {
    changeStater(state, val) {
        state.stater = val
    }
};

const actions = {

    changeFooterActiveTab({commit, state}, stateEnum) {
        if (state.statesEnum[stateEnum]) {
            commit('changeStater', state.statesEnum[stateEnum])
        }

    }

};
export default {
    state,
    mutations,
    getters,
    actions
}