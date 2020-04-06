import accountService from "../services/accountService";
import { dollarCalculate } from "./constants";
import analyticsService from '../services/analytics.service';
import insightService from '../services/insightService';
import { LanguageService } from '../services/language/languageService';
import intercomeService from '../services/intercomService';
import { router } from "../main";

const state = {
    isUserLoggedIn:false,
    user: null,
    usersReferred: 0,
};
const mutations = {
    changeIsUserTutor(state, val) {
        state.user.isTutor = val;
    },
    setIsTutorState(state, val) {
        state.user.isTutorState = val;
    },
    setRefferedNumber(state, data) {
        state.usersReferred = data;
    },
    logout(state) {
        state.isUserLoggedIn = false;
        state.user = null;
    },
    updateUser(state, val) {
        state.user = val;
    },
    changeLoginStatus(state, val) {
        state.isUserLoggedIn = val;
    },
    setAccountPicture(state, imageUrl) {
        state.user = { ...state.user, image: imageUrl };
    },
    setUserPendingPayment(state, payments) {
        state.user.pendingSessionsPayments = payments
    }
};

const getters = {
    getIsTutorState: state => {
        if (state.user && state.user.isTutorState) {
            return state.user.isTutorState;
        } else {
            return false;
        }
    },
    //TODO need to change this to accountretive
    getUserLoggedInStatus: state => state.isUserLoggedIn || global.isAuth,
    getIsTeacher: (state, getters) => {
        return getters.getUserLoggedInStatus && state.user?.userType === 'Teacher'
    },
    getIsStudent: (state, getters) => {
        return getters.getUserLoggedInStatus && ['UniversityStudent', 'HighSchoolStudent'].indexOf(state.user.userType) !== -1
    },
    usersReffered: state => state.usersReferred,
    accountUser: (state) => {
        return state.user;
    },
    getPendingPayment: state => state.user?.pendingSessionsPayments,
    getUserBalance: state =>  state.user?.balance.toFixed(0) || 0,
    getIsSold: state => state.user?.isSold
};

const actions = {
    
    uploadAccountImage(context, obj) {
        return accountService.uploadImage(obj).then((resp) => {
            let imageUrl = resp.data;
            context.commit('setAccountPicture', imageUrl);
            context.commit('setProfilePicture', imageUrl)
            return true;
        });
    },
    uploadCoverImage(context, obj) {
        return accountService.uploadCover(obj).then((resp) => {
            let imageUrl = resp.data;
           // context.commit('setAccountPicture', imageUrl);
            context.commit('setCoverPicture', imageUrl)
        });
    },
    getRefferedUsersNum(context, id) {
        accountService.getNumberReffered(id)
            .then(({ data }) => {
                let refNumber = data.referrals ? data.referrals : 0;
                context.commit('setRefferedNumber', refNumber);
            },
                (error) => {
                    console.error(error);
                }
            );
    },
    logout({ commit }) {
        intercomeService.restrartService();
        commit("logout");
        global.location.replace("/logout");
    },
    getUserAccountForRegister({dispatch}) {
        return accountService.getAccount().then((userAccount) => {
            dispatch('updateAccountUser',userAccount)
            return userAccount;
        })
    },
    updateAccountUser({commit,dispatch},userAccount){
        intercomeService.startService(userAccount);
        commit("updateUser", userAccount);
        // dispatch("syncUniData", userAccount); // uni
        dispatch("getAllConversations");
        analyticsService.sb_setUserId(userAccount.id);
        insightService.authenticate.set(userAccount.id);
        dispatch('updateLoginStatus',true);
    },
    userStatus({state,dispatch,getters}) {
        if(state.user !== null && state.user.hasOwnProperty('id')){
            return Promise.resolve()
        }
        if (getters.getUserLoggedInStatus) {
           return accountService.getAccount().then((userAccount) => {
                dispatch('updateAccountUser',userAccount);
                return Promise.resolve(userAccount)
            }, () => {
                //TODO what is that....
                intercomeService.restrartService();
                dispatch('updateLoginStatus',false)
            });
        }
        else {
            intercomeService.startService();
            return Promise.resolve()
        }
    },
    signalR_SetBalance({ commit, state, dispatch, getters }, newBalance) {
        if(router.currentRoute.query?.dialog){
            router.push({query:{...router.currentRoute.query,dialog:undefined}})
        }
        if (getters.getIsBuyPoints || state.user.balance > newBalance) {
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("buyTokens_success_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
        }
        commit('updateUser', { ...state.user, balance: newBalance, dollar: dollarCalculate(newBalance) });
    },
    updateAccountUserToTutor(context, val) {
        context.commit('changeIsUserTutor', val);
        context.commit('setIsTutorState', 'pending');
    },
    updateUserStats(context, lastDays) {
        return accountService.getAccountStats(lastDays)
    },
    updateLoginStatus({commit},val){
        commit("changeLoginStatus", val);
    }
};


export default {
    state,
    getters,
    actions,
    mutations
};