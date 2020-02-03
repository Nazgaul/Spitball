import accountService from "../services/accountService";
import { dollarCalculate } from "./constants";
import analyticsService from '../services/analytics.service';
import initSignalRService from '../services/signalR/signalrEventService';
import insightService from '../services/insightService';
import { LanguageService } from '../services/language/languageService';
import intercomeService from '../services/intercomService';

const state = {
    login: false,
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
        state.login = false;
        state.user = null;
    },
    updateUser(state, val) {
        state.user = val;
    },
    changeLoginStatus(state, val) {
        state.login = val;
    },
    setAccountPicture(state, imageUrl) {
        state.user = { ...state.user, image: imageUrl };
    },

    setUniExists(state, val) {
        state.user.universityExists = val;
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
    loginStatus: state => state.login,
    isUser: state => state.user !== null,
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
    userStatus({ dispatch, commit, getters }) {
        commit("changeLoginStatus", global.isAuth);
        // TODO check
        if (getters.isUser) {
            return;
        }
        if (global.isAuth) {
            accountService.getAccount().then((userAccount) => {
                intercomeService.startService(userAccount);
                commit("changeLoginStatus", true);
                commit("updateUser", userAccount);
                dispatch("syncUniData", userAccount);
                dispatch("getAllConversations");
                analyticsService.sb_setUserId(userAccount.id);
                insightService.authenticate.set(userAccount.id);
                initSignalRService();
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

    // TODO 
    updateUniExists(context, val) {
        context.commit("setUniExists", val);
    },
};


export default {
    state,
    getters,
    actions,
    mutations
};