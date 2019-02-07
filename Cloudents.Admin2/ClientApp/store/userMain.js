import UserMainService from '../components/userMainView/userMainService';

const state = {
    tokensDilaogState: false,
    userInfo: {},
    userQuestions: [],
    userAnswers: [],
    userDocuments: [],
    // userData: {
    //     // userInfo: {},
    //     // userQuestions: [],
    //     // userAnswers: [],
    //     // userDocuments: []
    // }
};
const mutations = {
    updateTokensDialog(state, val) {
        state.tokensDilaogState = val;
    },
    updateBalance(state, data) {
        if (state.userInfo && state.userInfo.balance) {
            state.userInfo.balance.value = state.userInfo.balance.value + data;
        }
    },
    updateStatus(state, val) {
        if (state.userInfo && state.userInfo.balance) {
            state.userInfo.status.value = val;
        }
    },
    setUserInfo(state, data) {
        state.userInfo = data;
    },
    setUserQuestions(state, data) {
        state.userQuestions = data;
    },
    setUserAnswers(state, data) {
        state.userAnswers = data;
    },
    setUserDocuments(state, data) {
        state.userDocuments = data;
    }
};
const getters = {
    getTokensDialogState: (state) => state.tokensDilaogState,
    getUserBalance: (state) => state.userBalance,
    UserInfo: (state) =>   state.userInfo,
    UserQuestions: (state) =>   state.userInfo,
    UserAnswers: (state) =>   state.userInfo,
    UserDocuments: (state) =>   state.userInfo

};
const actions = {
    setTokensDialogState({commit}, val) {
        commit('updateTokensDialog', val);

    },
    setUserCurrentBalance({commit}, data) {
        commit('updateBalance', data)
    },
    setUserCurrentStatus({commit}, val) {
        commit('updateStatus', val)
    },
    updateUserData({commit}, data) {
        commit('setUserData', data)
    },
    getUserData(context, id) {
       return UserMainService.getUserData(id).then((data) => {
                context.commit('setUserInfo', data)
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserQuestions(context, id) {
        UserMainService.getUserQuestions(id).then((data) => {
                context.commit('setUserQuestions', data)
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserAnswers(context, id) {
        UserMainService.getUserAnswers(id).then((data) => {
                context.commit('setUserAnswers', data)
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserDocuments(context, id) {
        UserMainService.getUserDocuments(id).then((data) => {
                context.commit('setUserDocuments', data)
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    }

};
export default {
    state, mutations, getters, actions
};