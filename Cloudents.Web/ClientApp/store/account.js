import axios from "axios";
import Talk from "talkjs";
import accountService from "../services/accountService"
import { debug } from "util";
//import { stat } from "fs";
let userLogin = false;

const state = {
    login: false,
    user: null,
    talkSession: null,
    talkMe: null,
    unreadMessages: 0,
    fromPath: null
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
    }
};

const getters = {
    fromPath: state => state.fromPath,
    unreadMessages: state => state.unreadMessages,
    loginStatus: state => state.login,
    isUser: state => state.user !== null,
    talkSession: state => state.talkSession,
    chatAccount: state => state.talkMe,
    accountUser: state => state.user
};
const actions = {
    logout({ state, commit }) {
        accountService.logout();
        commit("logout");
    },
    userStatus({ dispatch, commit, getters }, { isRequire, to }) {
        const $this = this;
        // if (getters.isUser) {
        //     return Promise.resolve();
        // }
        if (getters.isUser) {
            return Promise.resolve();
        }
        if (window.isAuth) {
            return axios.get("account").then(({ data }) => {
                commit("changeLoginStatus", true);
                commit("updateUser", data);
                dispatch("connectToChat");
            }).catch(_ => {
                isRequire ? commit("updateFromPath", to) : '';
                commit("changeLoginStatus", false);
            });
        }
    },
    connectToChat({ state, commit }) {
        if (!state.user) {
            return;
        }
        try {


           
            Talk.ready.then(() => {
                // 
                const me = new Talk.User(state.user.id);
                // const me = new Talk.User({
                //     id : state.user.id,
                //     configuration : "buyer"
                // });

                commit("updateChatUser", me);
                const talkSession = new Talk.Session({
                    appId: window.talkJsId,
                    me: me,
                    signature: state.user.token
                });
                //talkSession.syncThemeForLocalDev("/content/talkjs-theme.css")
                talkSession.unreads.on("change", m => {
                    commit("updateMessageCount", conversationIds.length);
                });
                commit("updateTalkSession", talkSession);
            });
        } catch (error) {
            console.error(error);
        }
    },
    updateUserBalance({commit,state},payload){
        debugger;
        commit('updateUser',{...state.user,balance:payload, dollar:(payload/40)})
    }
};


export default {
    state,
    getters,
    actions,
    mutations
}