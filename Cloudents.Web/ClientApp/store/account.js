import accountService from "../services/accountService";
import { dollarCalculate } from "./constants";
import analyticsService from '../services/analytics.service';
import initSignalRService from '../services/signalR/signalrEventService';
import insightService from '../services/insightService';
import { LanguageService } from '../services/language/languageService';
import intercomeService from '../services/intercomService';

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
};

const getters = {
    getIsTutorState: state => {
        if (state.user && state.user.isTutorState) {
            return state.user.isTutorState;
        } else {
            return false;
        }
    },
    getUserLoggedInStatus: state => state.isUserLoggedIn,
    usersReffered: state => state.usersReferred,
    accountUser: (state) => {
        return state.user;
    },
};

const actions = {
    uploadAccountImage(context, obj) {
        return accountService.uploadImage(obj).then((resp) => {
            let imageUrl = resp.data;
            context.commit('setAccountPicture', imageUrl);
            context.commit('setProfilePicture', imageUrl)
            return true;
        },
            (error) => {
                console.log(error, 'error upload account image');
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
    getUserAccountForRegister({commit}) {
        return accountService.getAccount().then((userAccount) => {
            commit("updateUser", userAccount);
            analyticsService.sb_setUserId(userAccount.id);
            commit("changeLoginStatus", true);
            return userAccount;
        })
    },
    userStatus({state,dispatch, commit}) {
        if(state.user !== null && state.user.hasOwnProperty('id')){
            return
        }
        
        if (global.isAuth) {
            accountService.getAccount().then((userAccount) => {
                intercomeService.startService(userAccount);
                commit("updateUser", userAccount);
                dispatch("syncUniData", userAccount);
                dispatch("getAllConversations");
                analyticsService.sb_setUserId(userAccount.id);
                insightService.authenticate.set(userAccount.id);
                initSignalRService();
                commit("changeLoginStatus", true);
            }, () => {
                //TODO what is that....
                intercomeService.restrartService();
                commit("changeLoginStatus", false);
            });
        }
        else {
            intercomeService.startService();
        }
    },
    signalR_SetBalance({ commit, state, dispatch, getters }, newBalance) {
        dispatch('updatePaymentDialogState', false);
        if (getters.getShowBuyDialog || state.user.balance > newBalance) {
            dispatch('updateShowBuyDialog', false);
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
};


export default {
    state,
    getters,
    actions,
    mutations
};