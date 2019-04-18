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
    activeConversationObj: chatService.createActiveConversationObj({}), //points to conversation Obj
    isVisible: true,
    isMinimized: true,
    totalUnread: 0,
    chatLocked: false
};
const getters = {
    getIsChatVisible:state=> state.isVisible,
    getIsChatMinimized:state=> state.isMinimized,
    getChatState:state=>state.chatState,
    getEnumChatState:state=>state.enumChatState,
    getConversations: state=>state.conversations,
    getMessages: (state, {getConversationIdCurrentUserId})=>{
        //can get only messages of the current conversation room
        if(!!state.activeConversationObj.conversationId){
            return state.messages[state.activeConversationObj.conversationId];
        }else if(!!state.activeConversationObj.userId){
            //get conversation id From User Id
            let conversationId  = getConversationIdCurrentUserId;
            if(!!conversationId){
                return state.messages[conversationId];
            }else{
                return [];
            }
        }
    },
    getConversationIdCurrentUserId:(state)=>{
        let userId = state.activeConversationObj.userId;
        return Object.keys(state.conversations).filter((conversationId)=>{
            return state.conversations[conversationId].userId === userId;
        })[0];
    },
    getActiveConversationObj:state=>state.activeConversationObj,
    getTotalUnread: state=>state.totalUnread,
    getIsChatLocked: state=>state.chatLocked,
};

const mutations = {
    addConversationUnread:(state, message)=>{
        state.conversations[message.conversationId].unread++
    },
    addMessage:(state, message)=>{
        let id = message.conversationId || state.activeConversationObj.conversationId;
        if(!state.messages[id]){
            // add a properly this way allow the computed to be fired!
            state.messages = { ...state.messages, [id]:[] };
        }
        state.messages[id].push(message);
    },
    setActiveConversationObj(state, obj){
        if(!!state.conversations[obj.conversationId]){
            state.activeConversationObj = chatService.createConversation(state.conversations[obj.conversationId]); 
        }else{
            state.activeConversationObj =  chatService.createActiveConversationObj(obj);
        }
    },
    setActiveConversationId(state, id){
        state.activeConversationObj.conversationId = id;
    },
    setActiveConversationStudyRoom(state, id){
        state.activeConversationObj.studyRoomId = id;
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
    },
    toggleChatMinimize:(state)=>{
        state.isMinimized = !state.isMinimized;
    },
    expandChat:()=>{
        state.isMinimized = false;
    },
    closeChat:(state)=>{
        state.isVisible = false
    },
    openChat:(state)=>{
        state.isVisible = true
    },
    clearUnreadFromConversation:(state, conversationId)=>{
        state.conversations[conversationId].unread = 0;
    },
    updateTotalUnread:(state, val)=>{
        //val could be negative value
        state.totalUnread = state.totalUnread + val;
    },
    lockChat:(state)=>{
        state.chatLocked = true;
    }
};

const actions = {
    addMessage:({commit, state, dispatch}, message)=>{
        //check if inside conversation
        let isInConversation = state.chatState == state.enumChatState.messages;
        if(isInConversation){
            //check if message sent is part of the current conversation
            if(state.activeConversationObj.conversationId === message.conversationId){
                commit('addMessage', message)
            }else{
                // check if conversation with this user is exists
                if(!!state.conversations[message.conversationId]){
                    //update unread conversations
                    commit('addConversationUnread', message)
                    commit('updateTotalUnread', 1);
                }else{
                    //no conversation should be added
                    // TODO get conversation by id
                    dispatch('getChatById', message.conversationId).then(({data})=>{
                        let ConversationObj = chatService.createConversation(data);
                        commit('addConversation', ConversationObj)
                        commit('setActiveConversationId', ConversationObj.conversationId)
                        commit('addMessage', message)
                    })
                }
            }
        }else{
            // check if conversation with this user is exists
            if(!!state.conversations[message.conversationId]){
                //update unread conversations
                commit('addConversationUnread', message)
                commit('updateTotalUnread', 1);
            }else{
                //no conversation should be added
                let ConversationObj = chatService.createConversation(message);
                commit('addConversation', ConversationObj)
            }
        }      
        
    },
    getChatById:(context, conversationId)=>{
        return chatService.getChatById(conversationId);
    },
    setTotalUnread:({commit}, totalUnread)=>{
        commit('updateTotalUnread', totalUnread)
    },
    clearUnread:({commit, state}, conversationId)=>{
        if(state.conversations[conversationId]){
            let otherUserId = state.conversations[conversationId].userId;
            chatService.clearUnread(otherUserId);
            let unreadNumber = state.conversations[conversationId].unread * -1;
            commit('updateTotalUnread', unreadNumber);
            commit('clearUnreadFromConversation', conversationId);
        }
    },
    signalRAddMessage({dispatch}, messageObj){
        let MessageObj = chatService.createMessage(messageObj.message, messageObj.conversationId);
        dispatch('addMessage', MessageObj);
    },
    signalRAddRoomInformationMessage({commit, dispatch, state}, roomInfo){
        let messageObj ={
            message: {
                userId: roomInfo.userId,
                text: `Room created ${global.location.origin}/studyroom/${roomInfo.id}`,
                type: 'text'
            },
            //TODO signalR should return Conversation ID
            conversationId: state.activeConversationObj.conversationId,
        }
        let MessageObj = chatService.createMessage(messageObj.message, messageObj.conversationId);
        dispatch('addMessage', MessageObj);
        commit('setActiveConversationStudyRoom', roomInfo.id);
    },
    setActiveConversationObj:({commit, dispatch, state}, Obj)=>{
        commit('setActiveConversationObj', Obj);
        dispatch('clearUnread', Obj.conversationId);
        dispatch('syncMessagesByConversationId');
        dispatch('updateChatState', state.enumChatState.messages);
    },
    getAllConversations:({commit, getters, dispatch})=>{
        if(!getters.accountUser) {
            commit('closeChat')
            return;
        };
        chatService.getAllConversations().then(({data})=>{
            if(data.length > 0){
                data.forEach(conversation => {
                    let ConversationObj = chatService.createConversation(conversation);
                        commit('addConversation', ConversationObj);
                        commit('updateTotalUnread', ConversationObj.unread);
                        let userStatus = {
                            id: ConversationObj.userId,
                            online: ConversationObj.online
                        }
                        dispatch('setUserStatus', userStatus);
                })
            }
        });
    },
    syncMessagesByConversationId:({dispatch, state, getters, commit})=>{
        //get from server the messages by id
        let id = null;
        if(!state.activeConversationObj.conversationId) {
            //try to get conversation ID
            let conversationId = getters.getConversationIdCurrentUserId
            if(!!conversationId){
                id = conversationId;
                commit('setActiveConversationId', id)
            }
        }else{
            id = state.activeConversationObj.conversationId;
        }
        if(!!id && !state.messages[id]){
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
    },
    openChatInterface:({commit, dispatch})=>{
        commit('expandChat');
        dispatch('openChat');
    },
    lockChat:({commit})=>{
        commit('lockChat');
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}