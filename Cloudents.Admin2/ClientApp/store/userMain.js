import userMainService from '../components/userMainView/userMainService';

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
    loader : false,
    MAX_ITEMS: 25,
    requestLock: false,
    currentIdRequest: '',
    userSoldItems: [],
    userNotes: [],
   // userTypes: []
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
        state.requestLock = false;
        state.currentIdRequest = '';
        state.userNotes = [];
       // state.userTypes = [];
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
    setUserNotes(state, data) {
        if(state.userNotes.length) {
            state.userNotes.unshift(data);
        }else {
            state.userNotes = data;
        }
    },
    updateTutorSate(state, val){
        //state.userInfo.isTutor = val;
        state.userInfo.isTutor.value = val;
    },
    updateCalenderSate()
    {
        state.userInfo.calendarExists.value = !state.userInfo.calendarExists.value;
    },
    setUserInfo(state, data) {
        state.userInfo = data;
    },
    // setUserTypes(state, data) {
    //     state.userTypes = data;
    // },
    // setUserType(state, data){
    //     state.userInfo.userType.value = data;
    // },
    setUserQuestions(state, data) {
        state.userQuestions = data;
    },
    setUserAnswers(state, data) {
        state.userAnswers = data;
    },
    setUserPurchasedDocs(state, data) {
        state.userPurchasedDocs = data;
    },
    setUserSoldItems(state,data){
        state.userSoldItems = data;
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
    },
    setLockRequestCall(state, val) {
        state.requestLock = val;
    },
    setCurrentIdRequest(state, id) {
        state.currentIdRequest = id;
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
    userSessions: (state) => state.userSessions,
    getRequestLock: (state) => state.requestLock,
    userSoldDocItems: (state) => state.userSoldItems,
    userNotes:(state) => state.userNotes
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
    addUserNote({commit}, data) {
        return userMainService.addUserNote(data).then((res) => {
            commit('setUserNotes', res);
        }) 
    },
    updateUserData({commit}, data) {
        commit('setUserData', data);
    },
    getUserData(context, id) {
        context.commit('clearUserData');
        return userMainService.getUserData(id)
        .then((data) => {
                if (data) {
                    context.commit('setUserInfo', data);
                    return data;
                }
            });
    },
    // getUserTypes(context, id){
    //     return userMainService.getUserTypes(id)
    //     .then((data) => {
    //                 context.commit('setUserTypes', data);
    //                 return data;
    //         });
    // },
    // updateUserType(context, {userId,userType}){
    //     return userMainService.updateUserType({userId,userType}).then(() => {
    //         context.commit('setUserType', userType);

    //     })
    // },
    removeTutor(context, id) {
        return userMainService.removeTutor(id)
        .then(() => {
            context.commit('updateTutorSate', 'Not a tutor');
        });
    },
    updateSuspendTutor({commit, state}, id) {      
        let suspendObj = {tutorId: id};
        return userMainService.suspendTutor(suspendObj).then(() => {
            state.userInfo.isTutor.value = 'flagged';
        }, (ex) => {
            console.log(ex);
        });
    },
    updateUnSuspendTutor({commit, state}, id) {
        let suspendObj = {tutorId: id};
        return userMainService.unSuspendTutor(suspendObj).then(() => {
            state.userInfo.isTutor.value = 'ok';
        }, (ex) => {
            console.log(ex);
        });
    },
    removeCalender(context, id) {
        return userMainService.removeCalender(id)
        .then(() => {
            context.commit('updateCalenderSate');
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
        return userMainService.getUserQuestions(idPageObj.id, idPageObj.page).then((data) => {
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
        return userMainService.getUserAnswers(idPageObj.id, idPageObj.page).then((data) => {
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
        return userMainService.getPurchasedDocs(idPageObj.id, idPageObj.page).then((data) => {
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
    getUserSoldItems(context,idPageObj){
        context.commit("setShowLoader", true);

        return userMainService.getSoldItems(idPageObj.id, idPageObj.page).then((data) => {
            if (data && data.length !== 0) {
                context.commit('setUserSoldItems', data);
            }
        },
        (error) => {
            console.log(error, 'error');
        }
    ).finally(() => context.commit("setShowLoader", false));
    },
    getUserDocuments(context, idPageObj) {
        let currentDocs;
        context.commit("setShowLoader", true);
        return userMainService.getUserDocuments(idPageObj.id, idPageObj.page).then((data) => {
                if(data.length < context.state.MAX_ITEMS) {
                    context.commit('setLockRequestCall', true);
                } else {
                    context.commit('setLockRequestCall', false);
                }

                if(idPageObj.id !== context.state.currentIdRequest) {
                    context.commit('setCurrentIdRequest', idPageObj.id);
                    currentDocs = data;
                } else {
                    currentDocs = [...context.state.userDocuments, ...data];
                }

                if (data && data.length !== 0) {
                    context.commit('setUserDocuments', currentDocs);
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
        return userMainService.getUserConversations(idPageObj.id).then((data) => {
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
        return userMainService.getUserSessions(idPageObj.id).then((data) => {

                context.commit('setUserSessions', data);
           
                if (data.length < quantityPerPage) {
                    return true;
                }
               
            },
            (error) => {
                console.log(error, 'error');
            }
        ).finally(() => context.commit("setShowLoader", false));
    },
    getUserNotes(context, idPageObj){
        if(!state.userNotes.length)
        {
        context.commit("setShowLoader", true);
        return userMainService.getUserNotes(idPageObj.id).then((data) => {
            context.commit('setUserNotes', data);
        
        },
        (error) => {
            console.log(error, 'error');
        }).finally(() => context.commit("setShowLoader", false));
        }
    },
    verifyUserPhone(context, verifyObj) {
        return userMainService.verifyPhone(verifyObj).then((resp) => {
            context.commit('setPhoneConfirmStatus');
            return resp;
        });
    },
    updateUserName({ commit, dispatch }, payload) {
        return userMainService.updateUserName(payload).then(() => {
            dispatch('getUserData', payload.userId);
        },
        () => {
            return false;
        });
    },
    updateUserPhone({dispatch }, payload) {
        return userMainService.updateUserPhone(payload).then(() => {
            return dispatch('getUserData', payload.userId);
        });
    },
    updateTutorPrice({commit}, priceObj) {
        return userMainService.updateTutorPrice(priceObj).then(() => {
             state.userInfo.tutorPrice.value = priceObj.price;
        }).catch(ex => {
            return ex;
        });
    },
    deletePayment({commit, dispatch, state}, id) {
        return userMainService.deletePayment(id).then(() => {
            state.userInfo.payment.value = false;
        }).catch(ex => {
            return ex;
        });
    }
};
export default {
    state, mutations, getters, actions
};