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
        if(!!activeConversationId){
            return state.messages[activeConversationId];
        }
    }
};

const mutations = {
    addMessage:(state, message)=>{
        if(!state.messages[message.id]){
            // add a properly this way allow the computed to be fired!
            state.messages = { ...state.messages, [message.id]:[] };
        }
        state.messages[message.id].push(message);
    },
    setActiveConversationId(state, id){
        state.activeConversationId = id;
    },
    addConversation: (state, conversationObj)=>{
        let id = conversationObj.conversationId;
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
                    // for(let i = 0 ; i < 50; i++){
                    let ConversationObj = chatService.createConversation(conversation);
                        commit('addConversation', ConversationObj);
                    // }
                })
            }
        });
    },
    syncMessagesByConversationId:({commit, state})=>{
        //get from server the messages by id
        let id = state.activeConversationId;
        chatService.getMessageById(id).then(({data})=>{
            if(!data) return;
            data.forEach(message => {
                let MessageObj = chatService.createMessage(message, id);
                commit('addMessage', MessageObj);
            })
        })
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