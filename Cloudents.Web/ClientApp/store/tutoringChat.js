import chatService from '../components/tutor/chat/chatService';

const state = {
    messages: [],
    identity: ''
};
const getters = {
    getChatMessages: state => state.messages,
    userIdentity: state => state.identity
};

const mutations = {
    setUserIdentity(state, val) {
        state.identity = val;
    },
    updateMessages(state, sendObj) {
        state.messages.push(sendObj)
    }
};

const actions = {
    updateUserIdentity({commit, state}, val) {
        commit('setUserIdentity', val)
    },
    addMessage({commit, state}, message) {
        commit('updateMessages', message)
    },
    sendMessage({commit, state}, message) {
        let sendObj = chatService.createMessageItem(message);
        chatService.sendChatMessage(sendObj);
        commit('updateMessages', sendObj)
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}