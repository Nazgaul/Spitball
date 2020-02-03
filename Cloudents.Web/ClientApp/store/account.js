import accountService from "../services/accountService";
import { dollarCalculate } from "./constants";
import analyticsService from '../services/analytics.service';
import reputationService from '../services/reputationService';
import initSignalRService from '../services/signalR/signalrEventService';
import insightService from '../services/insightService';
import { LanguageService } from '../services/language/languageService';
import intercomeService from '../services/intercomService';
import profileService from "../services/profileService";

const state = {
    profileReviews:null,
    login: false,
    user: null,
    lastActiveRoute: null,
    profile: null,
    usersReferred: 0,
    showEditDataDialog: false,
    profileImageLoading: false,
};
const mutations = {
    setProfileFollower(state, val){
        state.profile.user.isFollowing = val;
        if(val){
            state.profile.user.followers += 1;
        }else{
            state.profile.user.followers -= 1;
        }
    },
    setProfileImageLoading(state, val){
        state.profileImageLoading = val;
    },
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
    setProfileReviews(state,val){
        state.profileReviews = val;
    },
    setPorfileDocuments(state, val) {
        state.profile.documents = val;
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
        state.login = false;
        state.user = null;
    },
    setLastActiveRoute(state, val) {
        state.lastActiveRoute = val;
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
            state.profile.documents.result.forEach(question => {
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
    deleteItemFromProfile(state, {id}) {
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
            state.profile.documents.result.forEach((item, index) => {
                if(item.id === id) {
                    state.profile.documents.result.splice(index, 1);
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
            state.profile.user.tutorData.bio = newData.bio;
            state.profile.user.firstName = newData.firstName;
            state.profile.user.lastName = newData.lastName;
            state.profile.user.description = newData.description;
            state.profile.user.tutorData.price = newData.price;
        } else {
            state.profile.user.name = newData.name;
            state.profile.user.firstName = newData.firstName;
            state.profile.user.lastName = newData.lastName;
            state.profile.user.description = newData.description;
        }
    },
    setEditDialog(state, val){
        state.showEditDataDialog = val;
    }
};

const getters = {
    getProfileImageLoading: state =>{
       return state.profileImageLoading;
    },
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
    getProfileReviews: state => state.profileReviews,
    loginStatus: state => state.login,
    isUser: state => state.user !== null,
    usersReffered: state => state.usersReferred,
    accountUser: (state) => {
        return state.user;
    },
    lastActiveRoute: state => state.lastActiveRoute,
    getUniversity: state => {
        if(!!state.user && !!state.user.universityExists) {
            return true;
        } else {
            return false;
        }
    },
    getShowEditDataDialog: state => state.showEditDataDialog,
};

const actions = {
    toggleProfileFollower({state,commit},val){
        if(val){
            return profileService.followProfile(state.profile.user.id).then(()=>{
                commit('setProfileFollower',true)

                return Promise.resolve()
            })
        }else{
            return profileService.unfollowProfile(state.profile.user.id).then(()=>{
                commit('setProfileFollower',false)
                return Promise.resolve()
            })
        }
    },
    updateProfileImageLoader(context, val){
        context.commit('setProfileImageLoading', val);
    },
    updateEditDialog(context, val){
        context.commit('setEditDialog', val);
    },
    // TODO 
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
                                                       context.commit('setProfileImageLoading', false);
                                                 return true;
                                             },
                                             (error) => {
                                                 context.commit('setProfileImageLoading', false);
                                                 console.log(error, 'error upload account image');
                                             });
    },
    updateAccountUserToTutor(context, val) {
        context.commit('changeIsUserTutor', val);
        context.commit('setIsTutorState', 'pending');
    },
    syncProfile(context, {id,type,params,/*activeTab*/}) {
        
        // //fetch all the data before returning the value to the component
        profileService.getProfile(id).then(profileUserData=>{
            context.commit('setProfile', profileUserData);
            context.dispatch('updateProfileItemsByType', {id,type,params});
            context.dispatch('setUserStatus', profileUserData.user);
            if(profileUserData.user.isTutor){
                profileService.getProfileReviews(id).then(val=>{
                    context.commit('setProfileReviews', val);
                })
            }
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
    updateProfileItemsByType({state,commit},{id,type,params}){
        if(!!state.profile && !!state.profile.user) {
            if(type == "documents"){
                return profileService.getProfileDocuments(id,params).then(documents => {
                    commit('setPorfileDocuments', documents);
                });
            }
        }
    },
    removeItemFromProfile({commit}, data) {
        commit('deleteItemFromProfile', data);
    },
    resetProfileData(context) {
        context.commit('resetProfile');
    },
    logout({commit}) {
        intercomeService.restrartService();
        commit("logout");
        global.location.replace("/logout");

    },
    changeLastActiveRoute({commit}, route) {
        commit("setLastActiveRoute", route);
    },
    userStatus({dispatch, commit, getters}) {
        commit("changeLoginStatus", global.isAuth);
        // TODO check
        if(getters.isUser) {
            return;
        }
        if(global.isAuth) {
            accountService.getAccount().then((userAccount) => {
                intercomeService.startService(userAccount);
                commit("changeLoginStatus", true);
                commit("updateUser", userAccount);
                dispatch("syncUniData",userAccount);
                dispatch("getAllConversations");
                analyticsService.sb_setUserId(userAccount.id);
                insightService.authenticate.set(userAccount.id);
                initSignalRService();
            }, ()=>{
                //TODO what is that....
                intercomeService.restrartService();
                commit("changeLoginStatus", false);
            });
        } 
        else {
            intercomeService.startService();
        }
    },
    updateUserBalance() {
        return;
    },

    signalR_SetBalance({commit, state, dispatch,getters}, newBalance) {
        dispatch('updatePaymentDialogState',false);
        if(getters.getShowBuyDialog || state.user.balance > newBalance){
            dispatch('updateShowBuyDialog', false);
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("buyTokens_success_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
        }
        commit('updateUser', {...state.user, balance: newBalance, dollar: dollarCalculate(newBalance)});
    },
    profileVote({commit}, data) {
        commit('updateProfileVote', data);
    }
};


export default {
    state,
    getters,
    actions,
    mutations
};