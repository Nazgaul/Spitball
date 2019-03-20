import chatService from '../services/chatService';

const state = {
    conversations: {},
    messages: {},
    unreadMessages:{},
    enumChatState:{
        conversation:"conversation",
        messages:"messages"
    },
    chatState:"messages",
    activeConversationId: null,
    isVisible: true,
    isMinimized: false,
};
const getters = {
    getIsVisible:state=> state.isVisible,
    getChatState:state=>state.chatState,
    getConversations: state=>state.conversations,
    getMessages: (state)=>{
        //can get only messages of the current conversation room
        if(!!activeConversationId){
            return state.messages[activeConversationId];
        }
    }
};

const mutations = {
    addMessage:(state, message)=>{
        if(!state.messages[message.id]){
            state.messages[message.id] = [];
        }
        state.messages[message.id].push(message);
    },
    setActiveConversationId(state, id){
        state.activeConversationId = id;
    }
};

const actions = {
    addMessage:({commit}, messageObj)=>{
        //verify messageObj is actually in the right format
        commit('addMessage', messageObj)
    },
    setActiveConversationId:({commit, state}, id)=>{
        if(!!state.conversations[id])
        commit('setActiveConversationId', id)
    },
    syncMessagesByConversationId:({commit}, id)=>{
        //get from server the messages by  id
        
        //set messages to the messages obj
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}