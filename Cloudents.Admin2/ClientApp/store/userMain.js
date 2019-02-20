import UserMainService from '../components/userMainView/userMainService';

const quantatyPerPage = 200;
const state = {
    tokensDilaogState: false,
    suspendDialog: false,
    userInfo: {},
    userQuestions: [],
    userAnswers: [],
    userDocuments: [],
    userPurchasedDocs: [],
    userUpVotes: [],
    userDownVotes: [],
    userFlaggedItems: [],
    filterVal: 'ok'

};
const mutations = {

    clearUserData(state) {
        state.userInfo = {};
        state.userQuestions = [];
        state.userAnswers = [];
        state.userDocuments = [];
        state.userPurchasedDocs= [];
        state.userUpVotes= [];
        state.userDownVotes= [];
        state.userFlaggedItems= [];
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
    setUserUpVotes(state, data){
      state.userUpVotes = data;
    },
    setUserFlaggedItems(state, data){
        state.userFlaggedItems = data;
    },
    setUserDownVotes(state, data){
        state.userDownVotes = data;
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
    setFilterStr(state, strVal){
        state.filterVal = strVal
    }
};
const getters = {
    filterValue: (state) => state.filterVal,
    getTokensDialogState: (state) => state.tokensDilaogState,
    suspendDialogState: (state) => state.suspendDialog,
    getUserBalance: (state) => state.userBalance,
    UserInfo: (state) => state.userInfo,
    UserQuestions: (state) => state.userQuestions,
    UserAnswers: (state) => state.userAnswers,
    UserDocuments: (state) => state.userDocuments,
    UserPurchasedDocuments: (state)=> state.userPurchasedDocs,
    UserUpVotes: (state)=> state.userUpVotes,
    UserDownVotes: (state)=> state.userDownVotes,
    UserFlaggedItems: (state) => state.userFlaggedItems

};
const actions = {
    updateFilterValue({commit}, val) {
        commit('setFilterStr', val);
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
                return data
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },

    deleteQuestionItem({commit}, index) {
        commit('removeQuestion', index)
    },
    deleteAnswerItem({commit}, index) {
        commit('removeAnswer', index)
    },
    deleteDocumentItem({commit}, index) {
        commit('removeDocument', index)
    },

    getUserQuestions(context, idPageObj) {
        return UserMainService.getUserQuestions(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserQuestions', data);
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }

            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserAnswers(context, idPageObj) {
        return UserMainService.getUserAnswers(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserAnswers', data);
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }


            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserPurchasedDocuments(context, idPageObj) {
        return UserMainService.getPurchasedDocs(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserPurchasedDocs', data);
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }
            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserDocuments(context, idPageObj) {
        return UserMainService.getUserDocuments(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserDocuments', data)
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }
                context.commit('setUserDocuments', data)

            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },

    getUserFlaggedItems(context, idPageObj) {
    return UserMainService.getFlaggedItems(idPageObj.id, idPageObj.page).then((data) => {
            if (data && data.length !== 0) {
                context.commit('setUserFlaggedItems', data)
            }
            if (data.length < quantatyPerPage) {
                return true;
            }
            context.commit('setUserFlaggedItems', data)

        },
        (error) => {
            console.log(error, 'error')
        }
    )
},
    getUserUpVotes(context, idPageObj) {
        return UserMainService.getUpvotes(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserUpVotes', data)
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }
                context.commit('setUserUpVotes', data)

            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },
    getUserDownVotes(context, idPageObj) {
        return UserMainService.getDownVotes(idPageObj.id, idPageObj.page).then((data) => {
                if (data && data.length !== 0) {
                    context.commit('setUserDownVotes', data)
                }
                if (data.length < quantatyPerPage) {
                    return true;
                }
                context.commit('setUserDownVotes', data)

            },
            (error) => {
                console.log(error, 'error')
            }
        )
    },

    verifyUserPhone(context, verifyObj) {
        return UserMainService.verifyPhone(verifyObj).then((resp) => {
            context.commit('setPhoneConfirmStatus');
            return resp
        })
    },
};
export default {
    state, mutations, getters, actions
};