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
    updateMessageCount(state,val) {
        state.unreadMessages = val;
    }
};

const getters = {
    isLogIn: state => state.user !== null,
    talkSession: state => state.talkSession,
    chatAccount: state => state.talkMe,
    accountUser: state => state.user
};
const actions = {
    userStatus({ dispatch, commit }) {
        const $this = this;
        axios.get("account").then(({ data }) => {
            commit("changeLoginStatus", true);
            commit("updateUser", data);
            dispatch("connectToChat");
        }).catch(_ => {
        });
    },
    connectToChat({ state, commit }) {
        if (!state.user) {
            return;
        }
        const me = new Talk.User(state.user.id);
        // {
        //     id: state.user.id,
        //     configuration: "buyer"
        // });
        commit("updateChatUser", me);

        //var me = new Talk.User(state.user.id)
        const talkSession = new Talk.Session({
            appId: "tXsrQpOx",
            me: me,
            signature: state.user.token
        });
        talkSession.unreads.on("change", m => {
            commit("updateMessageCount",conversationIds.length);
        })

        commit("updateTalkSession", talkSession);
    }
};


export default {
    state,
    getters,
    actions,
    mutations
}