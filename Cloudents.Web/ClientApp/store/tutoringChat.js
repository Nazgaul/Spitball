import chatService from '../components/tutor/chat/chatService';

const state = {
    messages: [],

};
const getters = {
    getChatMessages: state => state.messages,
};

const mutations = {
    updateMessages(state, sendObj) {
        state.messages.push(sendObj)
    }
};

const actions = {
    addMessage({commit, state}, message) {
        commit('updateMessages', message)
    },
    uploadChatFile({commit, state }, objData){
        let link;
        let formData = objData.formData;
        let isImage = objData.isImageType;
        chatService.uploadChatFile(formData)
            .then((resp)=>{
                console.log('chat file store',resp);
                link = resp.data && resp.data.link ? resp.data.link : '';
                let userIdentity = state.identity;
                let messageObj = {
                    //if image type add prefix to differ
                    "text": isImage ? `sb-preview_${link}` : `${link}`,
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