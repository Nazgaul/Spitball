import axios from 'axios'

import analyticsService from '../services/analytics.service';
import intercomeService from '../services/intercomService';
import insightService from '../services/insightService';
import { dollarCalculate } from "./constants";
import { i18n } from '../plugins/t-i18n'
import { router } from "../main";

const accountInstance = axios.create({
    baseURL: '/api/account'
})

const state = {
    isUserLoggedIn:false,
    user: null,
    usersReferred: 0,
};

const getters = {
    //TODO need to change this to accountretive
    getUserLoggedInStatus: state => state.isUserLoggedIn || global.isAuth,
    getIsTeacher: (state, _getters) => _getters.getUserLoggedInStatus && state.user?.isTutor,
    usersReffered: state => state.usersReferred,
    accountUser: (state) => state.user,
    getPendingPayment: state => state.user?.pendingSessionsPayments,
    getUserBalance: state =>  state.user?.balance.toFixed(0) || 0,
    getIsSold: state => state.user?.isSold,
    getIsTutorSubscription: state => state.user?.subscription,
    getIsMyProfile: (state, _getters) => _getters.getUserLoggedInStatus && (state.user?.id === _getters.getProfile?.user.id),
    getAccountId: state => state.user?.id,
    getAccountName: state => state.user?.name,
    getAccountImage: state => state.user?.image,
};

const mutations = {
    setRefferedNumber(state, data) {
        state.usersReferred = data;
    },
    logout(state) {
        state.isUserLoggedIn = false;
        state.user = null;
        global.location.replace("/logout");
        //intercomeService.restrartService();
    },
    changeLoginStatus(state, val) {
        state.isUserLoggedIn = val;
    },
    setAccountPicture(state, imageUrl) {
        state.user = { ...state.user, image: imageUrl };
    },
    setUserPendingPayment(state, payments) {
        state.user.pendingSessionsPayments = payments
    },
    setAccount(state, data) {
        const user = new Account(data)

        function Account(objInit) {
            this.id = objInit.id
            this.email = objInit.email
            this.lastName = objInit.lastName
            this.firstName = objInit.firstName
            this.name = `${objInit.firstName} ${objInit.lastName}`
            this.image = objInit.image || ''
            this.balance = objInit.balance
            this.currencySymbol = objInit.currencySymbol
            this.subscription = objInit.tutorSubscription
            this.needPayment = objInit.needPayment
            this.isTutor = objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok'
            this.isSold = objInit.isSold
            this.pendingSessionsPayments = objInit.pendingSessionsPayments
            this.chatUnread = objInit.chatUnread
        }
        
        state.user = user
    }
};

const actions = {
    userStatus({state, commit, dispatch, getters}) {
        if(state.user !== null && state.user.hasOwnProperty('id')){
            return Promise.resolve()
        }
        if (getters.getUserLoggedInStatus) {
           return accountInstance.get().then(({data}) => {
                // setAccount must be first to initialize userAccount variable
                commit('setAccount', data);
                const userAccount = state.user
    
                analyticsService.sb_setUserId(userAccount.id);
                intercomeService.startService(userAccount);
                insightService.authenticate.set(userAccount.id);
                commit('updateTotalUnread',userAccount.chatUnread || 0)
                commit("changeLoginStatus", true);
    
                return Promise.resolve(userAccount)
            }).catch(ex => {
                intercomeService.restrartService();
                console.error(ex);
                commit("changeLoginStatus", false);
            })
        }
        else {
            intercomeService.startService();
            return Promise.resolve()
        }
    },
    uploadAccountImage({ commit }, obj) {
        return accountInstance.post('/image', obj).then(({data}) => {
            let imageUrl = data;
            commit('setAccountPicture', imageUrl);
            commit('setProfilePicture', imageUrl)
            return true;
        })
    },
    uploadCoverImage({ commit }, obj) {
        return accountInstance.post('/cover', obj).then(({data}) => {
            let imageUrl = data;
            commit('setCoverPicture', imageUrl)
        });
    },
    getRefferedUsersNum({ commit }) {
        accountInstance.get('/referrals').then(({ data }) => {
            let refNumber = data.referrals ? data.referrals : 0;
            commit('setRefferedNumber', refNumber);
        }).catch(ex => {
            console.error(ex);
        })
    },
    saveUserInfo(params) {
        return accountInstance.post('/settings', params)
    },
    updateUserStats(context, days) {
        return accountInstance.get('/stats', { params: { days } }).then(({data}) => {
            function Stats(objInit) {
                this.revenue = objInit.revenue
                this.sales = objInit.sales
                this.views = objInit.views
                this.followers = objInit.followers
            }
            return data.map(stats => new Stats(stats))
        })
    },
    signalR_SetBalance({ commit, state, dispatch, getters }, newBalance) {
        if(router.currentRoute.query?.dialog){
            router.push({query:{...router.currentRoute.query, dialog: undefined}})
        }
        if (getters.getIsBuyPoints || state.user.balance > newBalance) {
            dispatch('updateToasterParams', {
                toasterText: i18n.t("buyTokens_success_transaction"),
                showToaster: true,
                toasterTimeout: 5000
            });
        }
        commit('updateUser', { ...state.user, balance: newBalance, dollar: dollarCalculate(newBalance) });
    },
    changeLanguage(context, locale) {
        axios.post("/Account/language", { culture: locale }).then(resp => {
            console.log("language response success", resp);
            global.location.reload(true);
        }).catch(ex => {
            console.log("language error error", ex);
        })
    }
};


export default {
    state,
    getters,
    actions,
    mutations
};