import axios from "axios";
import Talk from "talkjs"
//import { stat } from "fs";
let userLogin = false;

const state = {
    login: false,
    user: null,
    talkSession: null,
    talkMe: null,
    unreadMessages: 0
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
    }
};

const getters = {
    unreadMessages:state=>state.unreadMessages,
    loginStatus:state=>state.login,
    isUser: state => state.user !== null,
    talkSession: state => state.talkSession,
    chatAccount: state => state.talkMe,
    accountUser: state => state.user
};
const actions = {
    userStatus({ dispatch, commit,getters }) {
        const $this = this;
        // if (getters.isUser) {
        //     return Promise.resolve();
        // }
        if(getters.isUser){
            return Promise.resolve();
        }
        return axios.get("account").then(({ data }) => {
            commit("changeLoginStatus", true);
            commit("updateUser", data);
            dispatch("connectToChat");
        }).catch(_ => {
            commit("changeLoginStatus", false);
        });
    },
    connectToChat({ state, commit }) {
        if (!state.user) {
            return;
        }

        const me = new Talk.User(state.user.id);
        // const me = new Talk.User({
        //     id : state.user.id,
        //     configuration : "buyer"
        // });

        commit("updateChatUser", me);
        const talkSession = new Talk.Session({
            appId: "tXsrQpOx",
            me: me,
            signature: state.user.token
        });
        talkSession.unreads.on("change", m => {
            commit("updateMessageCount", conversationIds.length);
        });
        commit("updateTalkSession", talkSession);
    }
};


export default {
    state,
    getters,
    actions,
    mutations
}