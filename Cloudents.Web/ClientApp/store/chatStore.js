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
    activeConversationObj: {
        userId: null,
        conversationId: null
    },
    isVisible: true,
    isMinimized: false,
};
const getters = {
    getIsChatVisible:state=> state.isVisible,
    getIsChatMinimized:state=> state.isMinimized,
    getChatState:state=>state.chatState,
    getEnumChatState:state=>state.enumChatState,
    getConversations: state=>state.conversations,
    getMessages: (state)=>{
        //can get only messages of the current conversation room
        if(!!state.activeConversationObj.conversationId){
            return state.messages[state.activeConversationObj.conversationId];
        }else if(!!state.activeConversationObj.userId){
            return [];
        }
    },
    getCurrentConversationObj:(state)=>{
        if(!!state.activeConversationObj.userId){
            return state.conversations[state.activeConversationObj.userId];
        }else{
            return null;
        }
    },
    getActiveConversationObj:state=>state.activeConversationObj,
};

const mutations = {
    addConversationUnread:(state, message)=>{
        state.conversations[message.userId].unread++
    },
    addMessage:(state, message)=>{
        let id = message.conversationId;
        if(!state.messages[id]){
            // add a properly this way allow the computed to be fired!
            state.messages = { ...state.messages, [id]:[] };
        }
        state.messages[id].push(message);
    },
    setActiveConversationObj(state, obj){
        state.activeConversationObj.userId = obj.userId || null;
        state.activeConversationObj.conversationId = obj.conversationId || null;
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
    },
    toggleChatMinimize:(state)=>{
        state.isMinimized = !state.isMinimized;
    },
    closeChat:(state)=>{
        state.isVisible = false
    },
    openChat:(state)=>{
        state.isVisible = true
    },
};

const actions = {
    addMessage:({commit, state}, message)=>{
        //check if inside conversation
        let isInConversation = state.chatState == state.enumChatState.messages;
        if(isInConversation){
            //check if message sent is part of the current conversation
            if(state.activeConversationObj.conversationId === message.conversationId){
                commit('addMessage', message)
            }else{
                // check if conversation with this user is exists
                if(!!state.conversations[message.userId]){
                    //update unread conversations
                    commit('addConversationUnread', message)
                }else{
                    //no conversation should be added
                    let ConversationObj = chatService.createConversation(message);
                    commit('addConversation', ConversationObj)
                }
            }
        }else{
            // check if conversation with this user is exists
            if(!!state.conversations[message.userId]){
                //update unread conversations
                commit('addConversationUnread', message)
            }else{
                //no conversation should be added
                let ConversationObj = chatService.createConversation(message);
                commit('addConversation', ConversationObj)
            }
        }      
        
    },
    signalRAddMessage({dispatch}, messageObj){
        let MessageObj = chatService.createMessage(messageObj.message, messageObj.conversationId);
        dispatch('addMessage', MessageObj);
    },
    setActiveConversationObj:({commit, dispatch, state}, Obj)=>{
        commit('setActiveConversationObj', Obj);
        dispatch('syncMessagesByConversationId');
        dispatch('updateChatState', state.enumChatState.messages);
    },
    getAllConversations:({commit, getters})=>{
        if(!getters.accountUser) {
            commit('closeChat')
            return;
        };
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
        if(!state.activeConversationObj.conversationId) return;
        let id = state.activeConversationObj.conversationId;
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
    },
    sendChatMessage:({state}, message)=>{
        let messageObj = chatService.createServerMessageObj({
            message: message,
            otherUser: state.activeConversationObj.userId
        })
        chatService.sendChatMessage(messageObj);
    },
    toggleChatMinimize:({commit})=>{
        commit('toggleChatMinimize')
    },
    closeChat:({commit})=>{
        commit('closeChat')
    },
    openChat:({commit})=>{
        commit('openChat')
    }
    
};
export default {
    state,
    mutations,
    getters,
    actions
}