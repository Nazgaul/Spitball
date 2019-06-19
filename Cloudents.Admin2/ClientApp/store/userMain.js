import UserMainService from '../components/userMainView/userMainService';

const quantityPerPage = 200;
const state = {
    tokensDilaogState: false,
    suspendDialog: false,
    userInfo: {},
    userQuestions: [],
    userAnswers: [],
    userDocuments: [],
    userPurchasedDocs: [],
    userConversations: [],
    userSessions: [],
    filterVal: 'ok',
    loader : false

};
const mutations = {

    clearUserData(state) {
        state.userInfo = {};
        state.userQuestions = [];
        state.userAnswers = [];
        state.userDocuments = [];
        state.userPurchasedDocs = [];
        state.userConversations = [];
        state.userSessions = [];
    },
    setShowLoader(state, val) {
        state.loader = val;
    },
    updateTokensDialog(state, val) {
        state.tokensDilaogState = val;
    },
    updateSuspendDialog(state, val) {
        state.suspendDialog = val;
    },
    updateBalance(state, data) {
        if (state.userInfo && state.userInfo.balance) {
            state.userInfo.balance.value = state.userInfo.balance.value + data;
        }
    },
    setPhoneConfirmStatus(state) {
        if (state.userInfo && state.userInfo.phoneNumberConfirmed) {
            state.userInfo.phoneNumberConfirmed.value = 'Yes';
            state.userInfo.phoneNumber.showButton = false;
        }
    },
    updateStatus(state, val) {
        if (state.userInfo && state.userInfo.status) {
            state.userInfo.status.value = val;
        }
    },
    updateTutorSate(state, val){
        //state.userInfo.isTutor = val;
        state.userInfo.isTutor.value = val
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
    setUserPurchasedDocs(state, data) {
        state.userPurchasedDocs = data;
    },
    setUserDocuments(state, data) {
        state.userDocuments = data;
    },
    setUserConversations(state, data) {
        state.userConversations = data;
    },
    setUserSessions(state, data) {
        state.userSessions = data;
    },
    removeQuestion(state, index) {
        state.userQuestions.splice(index, 1);
    },
    removeAnswer(state, index) {
        state.userAnswers.splice(index, 1);
    },
    removeDocument(state, index) {
        state.userDocuments.splice(index, 1);
    },
    setFilterStr(state, strVal) {
        state.filterVal = strVal;
    }
};
const getters = {
    getShowLoader: (state) => state.loader,
    filterValue: (state) => state.filterVal,
    getTokensDialogState: (state) => state.tokensDilaogState,
    suspendDialogState: (state) => state.suspendDialog,
    getUserBalance: (state) => state.userBalance,
    userInfo: (state) => state.userInfo,
    userQuestions: (state) => state.userQuestions,
    userAnswers: (state) => state.userAnswers,
    userDocuments: (state) => state.userDocuments,
    userPurchasedDocuments: (state) => state.userPurchasedDocs,
    userConversations: (state) => state.userConversations,
    userSessions: (state) => state.userSessions
};
const actions = {
    updateFilterValue({commit}, val) {
        commit('setFilterStr', val);
    },
    setShowLoader({ commit }, val) {
        commit('setShowLoader',val);
    },

    clearUserState({commit}) {
        commit('clearUserData');
    },
    setTokensDialogState({commit}, val) {
        commit('updateTokensDialog', val);
    },
    setSuspendDialogState({commit}, val) {
        commit('updateSuspendDialog', val);
    },
    setUserCurrentBalance({commit}, data) {
        commit('updateBalance', data);
    },
    setUserCurrentStatus({commit}, val) {
        commit('updateStatus', val);
    },
    updateUserData({commit}, data) {
        commit('setUserData', data);
    },
    getUserData(context, id) {
        return UserMainService.getUserData(id)
        .then((data) => {
                if (data) {
                    context.commit('setUserInfo', data);
                    return data;
                }else{
                    context.commit('clearUserData');
                }
            });
    },
    removeTutor(context, id) {
        return UserMainService.removeTutor(id)
        .then(() => {
                    context.commit('updateTutorSate', null);
                
            });
    },

    deleteQuestionItem({commit}, index) {
        commit('removeQuestion', index);
    },
    deleteAnswerItem({commit}, index) {
        commit('removeAnswer', index);
    },
    deleteDocumentItem({commit}, index) {
        commit('removeDocument', index);
    },

    getUserQuestions(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getUserQuestions(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserQuestions', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }

            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserAnswers(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getUserAnswers(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserAnswers', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }


            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserPurchasedDocuments(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getPurchasedDocs(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserPurchasedDocs', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }
            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserDocuments(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getUserDocuments(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserDocuments', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }
                context.commit('setUserDocuments', data);

            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserConversations(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getUserConversations(idPageObj.id).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserConversations', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }
                context.commit('setUserConversations', data);
            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserSessions(context, idPageObj) {
        context.commit("setShowLoader", true);
        return UserMainService.getUserSessions(idPageObj.id).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserSessions', data);
                }
                if (data.length < quantityPerPage) {
                    return true;
                }
                context.commit('setUserSessions', data);
            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    verifyUserPhone(context, verifyObj) {
        return UserMainService.verifyPhone(verifyObj).then((resp) => {
            context.commit('setPhoneConfirmStatus');
            return resp;
        });
    }
};
export default {
    state, mutations, getters, actions
};