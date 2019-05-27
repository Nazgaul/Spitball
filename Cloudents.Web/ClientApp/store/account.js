import accountService from "../services/accountService";
import { debug } from "util";
import { dollarCalculate } from "./constants";
import analyticsService from '../services/analytics.service';
import profileService from "../services/profile/profileService";
import reputationService from '../services/reputationService';
import initSignalRService from '../services/signalR/signalrEventService';


function setIntercomSettings(data) {
    let app_id = "njmpgayv";
    let hide_default_launcher = global.innerWidth < 600 ? true : intercomSettings.hide_default_launcher;
    let user_id = null;
    let user_name = null;
    let user_email = null;
    let user_phoneNumber = null;
    let alignment = global.isRtl ? 'left' : 'right';

    if(!!data) {
        user_id = "Sb_" + data.id;
        user_name = data.name;
        user_email = data.email;
        user_phoneNumber = data.phoneNumber;
    }
    global.intercomSettings = {
        app_id,
        hide_default_launcher,
        user_id,
        name: user_name,
        email: user_email,
        phoneNumber: user_phoneNumber,
        alignment: alignment
    };

    global.Intercom('boot', {intercomSettings});
}

function removeIntercomeData() {
    global.Intercom('shutdown');
}

function setIntercomeData(data) {
    //do not set intercome setting for mobile because no intercome should be on mobile case 10850
    // if(global.innerWidth > 600){
    //     setIntercomSettings(data)
    // }
    setIntercomSettings(data);
}


const state = {
    login: false,
    user: null,
    unreadMessages: 0,
    fromPath: null,
    lastActiveRoute: null,
    profileData: profileService.getProfileData('profileGeneral'),
    profile: null,
    usersReferred: 0,
    showEditDataDialog: false
};
const mutations = {
    setIsTutorState(state, val){
        state.user.isTutorState = val;
    },
    setUniExists(state, val){
        state.user.universityExists = val;
    },
    changeIsUserTutor(state, val) {
        state.user.isTutor = val;
    },

    setProfilePicture(state, imageUrl) {
        //check, cause if done from another place not profile(beoome tutor), will throw an error, cause
        // state.profile.user is undefined
        if(state.profile && state.profile.user) {
            state.profile.user = {...state.profile.user, image: imageUrl};
        }
        state.user = {...state.user, image: imageUrl};
    },

    setProfile(state, val) {
        state.profile = val;
    },
    setProfileQuestions(state, val) {
        state.profile.questions = val;
    },
    setProfileAbout(state, val) {
        state.profile = {...state.profile, about: val};
    },
    setProfileAnswers(state, val) {
        state.profile.answers = val;
    },
    setPorfileDocuments(state, val) {
        state.profile.documents = val;
    },
    setPorfilePurchasedDocuments(state, val) {
        state.profile.purchasedDocuments = val;
    },
    resetProfile(state) {
        state.profile = null;
    },
    changeLoginStatus(state, val) {
        state.login = val;
    },
    updateUser(state, val) {
        state.user = val;
    },
    logout(state) {
        state.fromPath = null;
        state.login = false;
        state.user = null;
    },
    updateFromPath(state, val) {
        state.fromPath = val;
    },
    setLastActiveRoute(state, val) {
        state.lastActiveRoute = val;
    },
    UPDATE_PROFILE_DATA(state, data) {
        state.profileData = data;
    },
    updateProfileVote(state, {id, type}) {
        if(!!state.profile) {
            state.profile.questions.forEach(question => {
                if(question.id === id) {
                    reputationService.updateVoteCounter(question, type);
                }
            });
            state.profile.answers.forEach(question => {
                if(question.id === id) {
                    reputationService.updateVoteCounter(question, type);
                }
            });
            state.profile.documents.forEach(question => {
                if(question.id === id) {
                    reputationService.updateVoteCounter(question, type);
                }
            });
            state.profile.purchasedDocuments.forEach(question => {
                if(question.id === id) {
                    reputationService.updateVoteCounter(question, type);
                }
            });

        }
    },
    deleteItemFromProfile(state, {id, type}) {
        if(!!state.profile) {
            state.profile.questions.forEach((item, index) => {
                if(item.id === id) {
                    state.profile.questions.splice(index, 1);
                }
            });
            state.profile.answers.forEach((item, index) => {
                if(item.id === id) {
                    state.profile.answers.splice(index, 1);
                }
            });
            state.profile.documents.forEach((item, index) => {
                if(item.id === id) {
                    state.profile.documents.splice(index, 1);
                }
            });
            state.profile.purchasedDocuments.forEach((item, index) => {
                if(item.id === id) {
                    state.profile.purchasedDocuments.splice(index, 1);
                }
            });


        }
    },
    setRefferedNumber(state, data) {
        state.usersReferred = data;
    },
    updateEditedData(state, newData) {
        if(state.profile.user.isTutor) {
            state.profile.about.bio = newData.bio;
            state.profile.user.tutorData.firstName = `${newData.name}`;
            state.profile.user.tutorData.lastName = newData.lastName;
            state.profile.user.description = newData.description;
            state.profile.user.tutorData.price = newData.price;
        } else {
            state.profile.user.name = newData.name;
            state.profile.user.description = newData.description;
        }

    },
    setEditDialog(state, val){
        state.showEditDataDialog = val;
    }
};

const getters = {
    isTutorProfile: state => {
        if(state.profile && state.profile.user && state.profile.user.isTutor) {
            return state.profile.user.isTutor;
        } else {
            return false;
        }
    },
    getIsTutorState: state =>{
        if( state.user && state.user.isTutorState) {
            return state.user.isTutorState;
        } else {
            return false;
        }
    },
    getProfile: state => state.profile,
    fromPath: state => state.fromPath,
    unreadMessages: state => state.unreadMessages,
    loginStatus: state => state.login,
    isUser: state => state.user !== null,
    usersReffered: state => state.usersReferred,
    accountUser: (state) => {
        return state.user;
    },
    lastActiveRoute: state => state.lastActiveRoute,
    getProfileData: state => state.profileData,
    getUniversity: state => {
        if(!!state.user && !!state.user.universityExists) {
            return true;
        } else {
            return false;
        }
    },
    getShowEditDataDialog: state => state.showEditDataDialog
};

const actions = {
    updateIsTutorState(context, val){
        context.commit('setIsTutorState', val)
    },
    updateEditDialog(context, val){
        context.commit('setEditDialog', val)
    },
    updateUniExists(context, val){
        context.commit("setUniExists", val);
    },
    updateEditedProfile(context, newdata) {
        context.commit("updateEditedData", newdata);
    },
    uploadAccountImage(context, obj) {
      return  accountService.uploadImage(obj).then((resp) => {
                                                 let imageUrl = resp.data;
                                                 context.commit('setProfilePicture', imageUrl);
                                                 return true
                                             },
                                             (error) => {
                                                 console.log(error, 'error upload account image');
                                             });
    },
    updateAccountUserToTutor(context, val) {
        context.commit('changeIsUserTutor', val);
        context.commit('setIsTutorState', 'pending');
    },
    syncProfile(context, {id, activeTab}) {
        //fetch all the data before returning the value to the component
        accountService.getProfile(id).then(val => {
            let profileUserData = accountService.createUserProfileData(val);
            context.commit('setProfile', profileUserData);
            // cause of multiple profile requests to server
            context.dispatch('setProfileByActiveTab', activeTab);
            context.dispatch('setUserStatus', profileUserData.user);
        });
    },
    getRefferedUsersNum(context, id) {
        accountService.getNumberReffered(id)
                      .then(({data}) => {
                                let refNumber = data.referrals ? data.referrals : 0;
                                context.commit('setRefferedNumber', refNumber);
                            },
                            (error) => {
                                console.error(error);
                            }
                      );
    },
    setProfileByActiveTab(context, activeTab) {
        if(!!context.state.profile && !!context.state.profile.user) {
            let id = context.state.profile.user.id;
            if(activeTab === 1) {
                /*
                 TODO v21 prevent duplication of gtProfile call, no need cause profile data loaded via 'syncProfile',
                  and here only the active  tab content is set
                */
                // let p1 = accountService.getProfile(id);
                let p2 = accountService.getProfileAbout(id);
                // return Promise.all([p1, p2]).then((vals) => {
                    return Promise.all([p2]).then((vals) => {
                    console.log(vals);
                    let profileData = accountService.createProfileAbout(vals);
                    context.commit('setProfileAbout', profileData);
                });

            }
            if(activeTab === 2) {
                return accountService.getProfileQuestions(id).then(vals => {
                    let profileData = accountService.createProfileQuestionData(vals);
                    context.commit('setProfileQuestions', profileData);
                });
            }
            if(activeTab === 3) {
                return accountService.getProfileAnswers(id).then(vals => {
                    let answers = accountService.createProfileAnswerData(vals);
                    context.commit('setProfileAnswers', answers);
                });
            }
            if(activeTab === 4) {
                return accountService.getProfileDocuments(id).then(vals => {
                    let documents = accountService.createProfileDocumentData(vals);
                    context.commit('setPorfileDocuments', documents);
                });
            }
            if(activeTab === 5) {
                return accountService.getProfilePurchasedDocuments(id).then(vals => {
                    let purchasedDocuments = accountService.createProfileDocumentData(vals);
                    context.commit('setPorfilePurchasedDocuments', purchasedDocuments);
                });
            }
        }
        console.log(activeTab);
    },
    removeItemFromProfile({commit}, data) {
        commit('deleteItemFromProfile', data);
    },
    resetProfileData(context) {
        context.commit('resetProfile');
    },
    getAnswers(context, answersInfo) {
        let id = answersInfo.id;
        let page = answersInfo.page;
        return accountService.getProfileAnswers(id, page).then(({data}) => {
            let maximumElementsRecivedFromServer = 50;
            if(data.length > 0) {
                data.forEach(answer => {
                    //create answer Object and push it to the state
                    let answerToPush = {
                        ...answer,
                        filesNum: answer.files,
                    };
                    context.state.profile.answers.push(answerToPush);
                });
            }
            //return true if we can call to the server
            return data.length === maximumElementsRecivedFromServer;
        }, (err) => {
            return false;
        });
    },
    getQuestions(context, questionsInfo) {
        let id = questionsInfo.id;
        let page = questionsInfo.page;
        let user = questionsInfo.user;
        return accountService.getProfileQuestions(id, page).then(({data}) => {
            let maximumElementsRecivedFromServer = 50;
            if(data.length > 0) {
                data.forEach(question => {
                    //create answer Object and push it to the state
                    let questionToPush = {
                        ...question,
                        user: user,
                        filesNum: question.files,
                    };
                    context.state.profile.questions.push(questionToPush);
                });
            }
            //return true if we can call to the server
            return data.length === maximumElementsRecivedFromServer;
        }, (err) => {
            return false;
        });
    },
    getDocuments(context, DocumentsInfo) {
        let id = DocumentsInfo.id;
        let page = DocumentsInfo.page;
        let user = DocumentsInfo.user;
        return accountService.getProfileDocuments(id, page).then(({data}) => {
            let maximumElementsRecivedFromServer = 50;
            if(data.length > 0) {
                data.forEach(document => {
                    //create answer Object and push it to the state
                    let documentToPush = {
                        ...document,
                        user: user,
                    };
                    context.state.profile.documents.push(documentToPush);
                });
            }
            //return true if we can call to the server
            return data.length === maximumElementsRecivedFromServer;
        }, (err) => {
            return false;
        });
    },
    getPurchasedDocuments(context, DocumentsInfo) {
        let id = DocumentsInfo.id;
        let page = DocumentsInfo.page;
        let user = DocumentsInfo.user;
        return accountService.getProfilePurchasedDocuments(id, page).then(({data}) => {
            let maximumElementsRecivedFromServer = 50;
            if(data.length > 0) {
                data.forEach(document => {
                    //create answer Object and push it to the state
                    let documentToPush = {
                        ...document,
                        user: user,
                    };
                    context.state.profile.purchasedDocuments.push(documentToPush);
                });
            }
            //return true if we can call to the server
            return data.length === maximumElementsRecivedFromServer;
        }, (err) => {
            return false;
        });
    },
    updateUserProfileData(context, name) {
        let currentProfile = profileService.getProfileData(name);
        context.commit("UPDATE_PROFILE_DATA", currentProfile);

    },
    logout({state, commit}) {
        removeIntercomeData();
        setIntercomeData();
        if(!state.login) return;
        commit("logout");
        global.location.replace("/logout");

    },
    changeLastActiveRoute({commit}, route) {
        commit("setLastActiveRoute", route);
    },
    userStatus({dispatch, commit, getters, rootState}, {isRequire, to}) {
        const $this = this;
        if(getters.isUser) {
            return Promise.resolve();
        }
        if(global.isAuth) {
            return accountService.getAccount().then((UserAccount) => {
                setIntercomeData(UserAccount);
                commit("changeLoginStatus", true);
                commit("updateUser", UserAccount);
                dispatch("syncUniData");
                dispatch("getAllConversations");
                analyticsService.sb_setUserId(UserAccount.id);
                initSignalRService();
            }).catch(_ => {
                setIntercomeData();
                isRequire ? commit("updateFromPath", to) : '';
                commit("changeLoginStatus", false);

            });
        } else {
            removeIntercomeData();
            setIntercomeData();
        }
    },
    saveCurrentPathOnPageChange({commit}, {currentRoute}) {
        commit("updateFromPath", currentRoute);
    },
    updateUserBalance({commit, state}, payload) {
        return;
        let newBalance = state.user.balance + payload;
        // debugger
        commit('updateUser', {...state.user, balance: newBalance, dollar: dollarCalculate(newBalance)});
    },

    signalR_SetBalance({commit, state}, newBalance) {
        commit('updateUser', {...state.user, balance: newBalance, dollar: dollarCalculate(newBalance)});
    },
    profileVote({commit}, data) {
        commit('updateProfileVote', data);
    },
    signalR_userPaymentReceived({state}){
        state.user.needPayment = false;
    },
};


export default {
    state,
    getters,
    actions,
    mutations
};