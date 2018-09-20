import Talk from "talkjs";
import accountService from "../services/accountService"
import {debug} from "util";
import {dollarCalculate} from "./constants";
import analyticsService from '../services/analytics.service'


function setIntercomSettings(data){
    let app_id = "njmpgayv";
    let hide_default_launcher = intercomSettings.hide_default_launcher;
    let user_id = null;
    let user_name = null;
    let user_email = null;
    if(!!data){
        user_id = "Sb_" + data.id;
        user_name = data.name;
        user_email = data.email;
    }
    global.intercomSettings = {
        app_id,
        hide_default_launcher,
        user_id,
        name: user_name,
        email: user_email
    }

    global.Intercom('boot', {intercomSettings});
}

function removeIntercomeData(){
    global.Intercom('shutdown');
}

function setIntercomeData(data){
    //do not set intercome setting for mobile because no intercome should be on mobile case 10850
    if(global.innerWidth > 600){
        setIntercomSettings(data)
    }
}


const state = {
    login: false,
    user: null,
    talkSession: null,
    talkMe: null,
    unreadMessages: 0,
    fromPath: null,
    lastActiveRoute: null
}
const mutations = {
    changeLoginStatus(state, val) {
        state.login = val;
    },
    updateUser(state, val) {
        state.user = val;
    },
    updateTalkSession(state, val) {
        state.talkSession = val;
    },
    updateChatUser(state, val) {
        state.talkMe = val;
    },
    updateMessageCount(state, val) {
        state.unreadMessages = val;
    },
    logout(state) {
        state.fromPath = null;
        state.login = false;
        state.user = null;
    },
    updateFromPath(state, val) {
        state.fromPath = val;
    },
    setLastActiveRoute(state, val){
        state.lastActiveRoute = val;
    }
};

const getters = {
    fromPath: state => state.fromPath,
    unreadMessages: state => state.unreadMessages,
    loginStatus: state => state.login,
    isUser: state => state.user !== null,
    talkSession: state => state.talkSession,
    chatAccount: state => state.talkMe,
    accountUser: state => state.user,
    lastActiveRoute: state => state.lastActiveRoute
};




const actions = {
    logout({state, commit}) {
        removeIntercomeData();
        setIntercomeData();
        global.location.replace("/logout");

    },
    changeLastActiveRoute({commit}, route){
        commit("setLastActiveRoute", route)
    },
    userStatus({dispatch, commit, getters}, {isRequire, to}) {
        const $this = this;
        // if (getters.isUser) {
        //     return Promise.resolve();
        // }
        if (getters.isUser) {
            return Promise.resolve();
        }
        if (global.isAuth) {
            return accountService.getAccount().then(({data}) => {
                setIntercomeData(data)
                commit("changeLoginStatus", true);
                commit("updateUser", data);
                dispatch("connectToChat");
                analyticsService.sb_setUserId(data.id);
            }).catch(_ => {
                setIntercomeData()
                isRequire ? commit("updateFromPath", to) : '';
                commit("changeLoginStatus", false);
                
            });
        }else{
            removeIntercomeData();
            setIntercomeData();
        }
    },
    saveCurrentPathOnPageChange({commit}, {currentRoute}){
        commit("updateFromPath", currentRoute);
    },
    connectToChat({state, commit}) {
        if (!state.user) {
            return;
        }
        try {


            Talk.ready.then(() => {
                // 
                // const me = new Talk.User(state.user.id);
                //{id, name, email, phone, photoUrl, welcomeMessage, configuration, custom, availabilityText, locale}
                const me = new Talk.User({
                    id: state.user.id,
                    name: state.user.name,
                    photoUrl: state.user.image,
                    configuration: "buyer"
                });

                commit("updateChatUser", me);
                const talkSession = new Talk.Session({
                    appId: global.talkJsId,
                    me: me,
                    signature: state.user.token
                });
                //talkSession.syncThemeForLocalDev("/content/talkjs-theme.css")
                talkSession.unreads.on("change", m => {
                    commit("updateMessageCount", m.length);
                });
                commit("updateTalkSession", talkSession);
            });
        } catch (error) {
            console.error(error);
        }
    },
    updateUserBalance({commit, state}, payload) {
        let newBalance = state.user.balance + payload;
        // debugger
        commit('updateUser', {...state.user, balance: newBalance, dollar: dollarCalculate(newBalance)})
    }
};


export default {
    state,
    getters,
    actions,
    mutations
}