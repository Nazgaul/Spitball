import UserMainService from '../components/userMainView/userMainService';

const state = {
    tokensDilaogState: false,
    userData: {
        // userInfo: {},
        // userQuestions: [],
        // userAnswers: [],
        // userDocuments: []
    }
};
const mutations = {
    updateTokensDialog(state, val) {
        state.tokensDilaogState = val;
    },
    updateBalance(state, data) {
        if (state.userData.userInfo && state.userData.userInfo.balance) {
            state.userData.userInfo.balance.value = state.userData.userInfo.balance.value + data;
        }
    },
    setUserData(state, data) {
        state.userData = data;
    }
};
const getters = {
    getTokensDialogState: (state) => state.tokensDilaogState,
    getUserBalance: (state) => state.userBalance,
    getUserObj: (state) => state.userData,

};
const actions = {
    setTokensDialogState({commit}, val) {
        commit('updateTokensDialog', val);

    },
    setUserCurrentBalance({commit}, data) {
        commit('updateBalance', data)
    },
    updateUserData({commit}, data) {
        commit('setUserData', data)
    },
    getUserData(context, id) {
        UserMainService.getUserData(id).then((data) => {
                let balance = data.userInfo.balance;
                //call mutation to update data
                context.commit('updateBalance', balance);
                context.commit('setUserData', data)
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