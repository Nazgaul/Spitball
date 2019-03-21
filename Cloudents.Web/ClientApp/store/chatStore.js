import chatService from '../services/chatService';

const state = {
    conversations: {},
    messages: {},
    unreadMessages:{},
    enumChatState:{
        conversation:"conversation",
        messages:"messages"
    },
    chatState:"conversation",
    activeConversationId: null,
    isVisible: true,
    isMinimized: false,
};
const getters = {
    getIsVisible:state=> state.isVisible,
    getChatState:state=>state.chatState,
    getEnumChatState:state=>state.enumChatState,
    getConversations: state=>state.conversations,
    getMessages: (state)=>{
        //can get only messages of the current conversation room
        if(!!state.activeConversationId){
            return state.messages[state.activeConversationId];
        }
    }
};

const mutations = {
    addMessage:(state, message)=>{
        let id = message.conversationId;
        if(!state.messages[id]){
            // add a properly this way allow the computed to be fired!
            state.messages = { ...state.messages, [id]:[] };
        }
        state.messages[id].push(message);
    },
    setActiveConversationId(state, id){
        state.activeConversationId = id;
    },
    addConversation: (state, conversationObj)=>{
        let id = conversationObj.userId;
        // add a properly this way allow the computed to be fired!
        state.conversations = { ...state.conversations, [id]:conversationObj };
    },
    changeChatState: (state, newChatState)=>{
        if(!!state.enumChatState[newChatState]){
            state.chatState = newChatState;
        }
    }
};

const actions = {
    addMessage:({commit}, messageObj)=>{
        //verify messageObj is actually in the right format
        commit('addMessage', messageObj)
    },
    setActiveConversationId:({commit, dispatch, state}, id)=>{
        commit('setActiveConversationId', id);
        dispatch('syncMessagesByConversationId');
        commit('changeChatState', state.enumChatState.messages);
    },
    getAllConversations:({commit})=>{
        chatService.getAllConversations().then(({data})=>{
            if(data.length > 0){
                data.forEach(conversation => {
                    let ConversationObj = chatService.createConversation(conversation);
                        commit('addConversation', ConversationObj);
                })
            }
        });
    },
    syncMessagesByConversationId:({dispatch, state})=>{
        //get from server the messages by id
        let id = state.activeConversationId;
        if(!state.messages[id]){
            chatService.getMessageById(id).then(({data})=>{
                if(!data) return;
                data.forEach(message => {
                    let MessageObj = chatService.createMessage(message, id);
                    dispatch('addMessage', MessageObj);
                })
            })
        }
    },
    updateChatState:({commit}, val)=>{
        commit('changeChatState', val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}