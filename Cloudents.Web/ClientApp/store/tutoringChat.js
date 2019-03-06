import chatService from '../components/tutor/chat/chatService';

const state = {
    messages: [],
    identity: '',
    isRoom: false,
    roomId: ''
};
const getters = {
    getChatMessages: state => state.messages,
    userIdentity: state => state.identity,
    isRoomCreated : state => state.isRoom,
    roomLinkID: state => state.roomId,
};

const mutations = {
    setRoomId(state, val){
        state.roomId = val
    },
    setRoomStatus(state, val){
        state.isRoom = val;
    },
    setUserIdentity(state, val) {
        state.identity = val;
    },
    updateMessages(state, sendObj) {
        state.messages.push(sendObj)
    }
};

const actions = {
    updateRoomID({commit, state}, val){
        commit('setRoomId', val)
    },
    updateRoomStatus({commit, state}, val) {
        commit('setRoomStatus', val)
    },
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