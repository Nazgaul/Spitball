import leaderBoardService from '../services/leadersBoardService';

const state = {
    leaderBoardMobState: false,

    leaderBoardData:{
        leaders:[],
        total: 0
    }
};

const getters = {
    getLeaderBoardState: state => state.leaderBoardMobState,
    LeaderBoardData: state => state.leaderBoardData
};

const mutations = {
    setLeaderBoardState(state, val) {
        state.leaderBoardMobState = val
    },
    updateLeaders(state, data) {
        state.leaderBoardData = {...data};
    }
};

const actions = {
    updateLeaderBoardState({commit}, val) {
        commit('setLeaderBoardState', val);
    },
    getLeadeBoardData({commit, rootState}) {
        let data = {
            leaders: [],
            total: 0
        };
        console.log(rootState.route);
       return leaderBoardService.getLeaderBoardItems().then(
            (resp) => {
                data.leaders = resp.data.leaderBoard.map((leaderBoardService.createLeaderBoardItem));
                data.total = resp.data.sbl || resp.data.SBL;
                commit("updateLeaders", data)
            },
            (error) => {

            }
        )
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}