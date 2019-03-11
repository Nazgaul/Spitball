import chatService from '../components/tutor/chat/chatService';

const state = {
    messages: [],
    identity: '',
    isRoom: false,
    roomId: '',
    isRoomFull : false
};
const getters = {
    getChatMessages: state => state.messages,
    userIdentity: state => state.identity,
    isRoomCreated : state => state.isRoom,
    roomLinkID: state => state.roomId,
    isRoomFull: state => state.isRoomFull
};

const mutations = {
    setRoomIsFull(state, val){
        state.isRoomFull =val
    },
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
    updateRoomIsFull({commit, state}, val){
        commit('setRoomIsFull', val)
    },
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
    uploadChatFile({commit, state }, formData){
        let link;
        chatService.uploadChatFile(formData)
            .then((resp)=>{
                console.log('chat file store',resp);
                link = resp.data && resp.data.link ? resp.data.link : '';
                let userIdentity = state.identity;
                let messageObj = {
                    "text": link,
                    "type": 'tutoringChatMessage',
                    "identity" : userIdentity
                };
                let sendObj = chatService.createMessageItem(messageObj);
                chatService.sendChatMessage(sendObj);
                commit('updateMessages', sendObj)
            })
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