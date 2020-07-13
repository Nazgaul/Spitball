import axios from 'axios'
import analyticsService from '../services/analytics.service';
import intercomeService from '../services/intercomService';
import * as componentConsts from '../components/pages/global/toasterInjection/componentConsts.js'
import { dollarCalculate } from "./constants";

const accountInstance = axios.create({
    baseURL: '/api/account'
})

const state = {
    isUserLoggedIn:false,
    user: null,
    usersReferred: 0,
};

const getters = {
    getUserLoggedInStatus: state => state.isUserLoggedIn || global.isAuth,
    getIsTeacher: (state, _getters) => _getters.getUserLoggedInStatus && state.user?.isTutor,
    usersReffered: state => state.usersReferred,
    accountUser: (state) => state.user,
    getPendingPayment: state => state.user?.pendingSessionsPayments,
    getUserBalance: state =>  state.user?.balance.toFixed(0) || 0,
    getIsSold: state => state.user?.isSold,
    getIsTutorSubscription: state => state.user?.subscription,
    getAccountId: state => state.user?.id,
    getAccountFirstName: state => state.user?.firstName,
    getAccountLastName: state => state.user?.lastName,
    getAccountName: state => state.user?.name,
    getAccountEmail: state => state.user?.email,
    getAccountImage: state => state.user?.image,
    getIsAccountChat: state => state.user?.chatUnread !== null && state.user?.chatUnread !== undefined,
    
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
    updateUser(state, data) {
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
            this.isTutor = typeof objInit.isTutor == 'boolean'? objInit.isTutor : objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok';
            this.isSold = objInit.isSold
            this.pendingSessionsPayments = objInit.pendingSessionsPayments
            this.chatUnread = objInit.chatUnread;
        }
        
        state.user = user
    },
    setAccountStudentInfo(state, { firstName, lastName }) {
        state.user.name = `${firstName} ${lastName}`
        state.user.firstName = firstName
        state.user.lastName = lastName
    },
    setAccountTutorInfo(state, { firstName, lastName }) {
        state.user.name = `${firstName} ${lastName}`
        state.user.firstName = firstName
        state.user.lastName = lastName
    }
};

const actions = {
    userStatus({state, commit,  getters}) {
        if(state.user !== null && state.user.hasOwnProperty('id')){
            return Promise.resolve()
        }
        if (getters.getUserLoggedInStatus) {
           return accountInstance.get().then(({data}) => {
                // setAccount must be first to initialize userAccount variable
                commit('updateUser', data);
                const userAccount = state.user
    
                analyticsService.sb_setUserId(userAccount.id);
                intercomeService.startService(userAccount);
                global.appInsights.setAuthenticatedUserContext(`${userAccount.id}`);
                //dispatch("getAllConversations");
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
    saveUserInfo({getters, commit}, params) {
       let passData =  {
           firstName: params.firstName,
           lastName: params.lastName,
           title: params.title,
           shortParagraph: params.shortParagraph,
           paragraph: params.bio
       }
        return accountInstance.post('/settings', passData).then(() => {
            if(getters.getIsTeacher) {
                commit('setProfileTutorInfo', params)
                commit('setAccountTutorInfo', params)
                return
            }
            commit('setAccountStudentInfo', params)
            return
        })
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
    signalR_SetBalance({ commit, state, getters }, newBalance) {
        if(getters.getIsComponentActiveByName(componentConsts.PAYMENT_DIALOG)){
            commit('removeComponent',componentConsts.PAYMENT_DIALOG)
        }
        if (getters.getIsBuyPoints || state.user.balance > newBalance) {
            commit('setComponent', 'buyPointsTransaction')
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