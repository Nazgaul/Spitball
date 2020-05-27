import chatService from '../services/chatService';
import analyticsService from '../services/analytics.service';


const state = {
    fileError: false,
    conversations: {},
    messages: {},
    enumChatState: {
        conversation: "conversation",
        messages: "messages"
    },
    chatState: "conversation", //check if we need
    activeConversationObj: chatService.createActiveConversationObj({}), //points to conversation Obj
    isMinimized: true, //check if we need
    totalUnread: 0,
    chatLoader: false,
    isSyncing: true,
};
const getters = {
    getFileError: state => state.fileError,
    getConversations: (state)=>{
        let conversations = Object.keys(state.conversations).map((prop)=>{
            return state.conversations[prop];
        });
        return conversations.sort(function(a,b){
            return new Date(b.dateTime) - new Date(a.dateTime);
          });
    },
    getMessages: (state, {getConversationIdCurrentUserId})=>{
        //can get only messages of the current conversation room;
        if(!!state.activeConversationObj.conversationId){
            if(!!state.messages[state.activeConversationObj.conversationId]){
                let messages = state.messages[state.activeConversationObj.conversationId];
                return messages;
            }else{
                if(!state.isSyncing){
                    return [];
                }
            }
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
    getChatLoader: state=> state.chatLoader,
    getConversationIdCurrentUserId:(state)=>{
        let userId = state.activeConversationObj.userId;
        return Object.keys(state.conversations).filter((conversationId)=>{
            return state.conversations[conversationId].userId === userId;
        })[0];
    },
    getActiveConversationObj:state=>state.activeConversationObj,
    getTotalUnread: state=>state.totalUnread,
};

const mutations = {
    setFileError(state,val){
        state.fileError = val;
    },
    addConversationUnread:(state, message)=>{
        state.conversations[message.conversationId].unread++;
        if(message.type === 'text'){
            state.conversations[message.conversationId].lastMessage = message.text;
        }
        state.conversations[message.conversationId].dateTime = message.dateTime;
    },
    addMessage:(state, message)=>{
        let id = message.conversationId || state.activeConversationObj.conversationId;
        if(!state.messages[id]){
            // add a properly this way allow the computed to be fired!
            state.messages = { ...state.messages, [id]:[] };
        }
        state.messages[id].push(message);
        if(message.type === 'text'){
            state.conversations[id].lastMessage = message.text;
        } else if(message.type === 'file') {
            state.conversations[id].lastMessage = chatService.createLastImageMsg();
        }
        state.conversations[id].dateTime = message.dateTime;
    },
    resetMessagesById:(state, id)=>{
        state.messages[id].length = 0;
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
    clearUnreadFromConversation:(state, conversationId)=>{
        state.conversations[conversationId].unread = 0;
    },
    updateTotalUnread:(state, val)=>{
        //val could be negative value
        state.totalUnread = state.totalUnread + val;
    },
    activateLoader:(state, val)=>{
        state.chatLoader = val;
        },
    setSyncStatus:(state, val)=>{
        state.isSyncing = val;
    },
};

const actions = {
    updateFileError({commit},val){
        commit('setFileError',val);
        setTimeout(() => {
            commit('setFileError',!val);
        }, 3000);
    },
    updateChatUploadLoading({commit}, val){
        commit('activateLoader', val);
    },
    addMessage:({commit, state, getters, dispatch}, message)=>{
        //check if inside conversation
        let isInConversation = state.chatState == state.enumChatState.messages;
        let conversationExists = !!state.conversations[message.conversationId];
        if(conversationExists){
            if(isInConversation){
                //check if message sent is part of the current conversation
                if(state.activeConversationObj.conversationId === message.conversationId){
                    commit('addMessage', message);
                    if (message.fromSignalR) {
                        chatService.clearUnread(state.activeConversationObj.conversationId);
                    }
                    if(state.isMinimized && message.fromSignalR){  //check if we need
                        //in tutor room the conversation is auto loaded, so in case of refresh
                        //we dont want to update the total unread unless signalR message arrives
                        commit('addConversationUnread', message);
                        commit('updateTotalUnread', 1);
                    }
                }else{
                        //update unread conversations
                        commit('addConversationUnread', message);
                        commit('updateTotalUnread', 1);
                }
            }else{
                    //update unread conversations
                    commit('addConversationUnread', message);
                    commit('updateTotalUnread', 1);
            }
        }else{
            if(isInConversation){
                if(state.activeConversationObj.conversationId === message.conversationId){
                    // message here will be sent by local user
                    //if in conversation and is the first message then create a conversation before adding the message
                    let conversationObj = chatService.createConversation(message);
                    commit('addConversation', conversationObj);
                    commit('addMessage', message);

                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${getters.accountUser.id}`);
                }else{
                    // message here will be sent by remote user
                    dispatch('getChatById', message.conversationId).then(({data})=>{
                        let newData;
                        if(message.type === 'text') {
                            newData = {...data, lastMessage:message.text};
                        }
                        let conversationObj = chatService.createConversation(newData);
                        commit('addConversation', conversationObj);
                        // commit('addConversationUnread', message);
                        commit('updateTotalUnread', 1);
                    });
                }
            }else{
                //conversationId should be added to the current conversation
                dispatch('getChatById', message.conversationId).then(({data})=>{
                    let newData;
                    if(message.type === 'text') {
                        newData = {...data, lastMessage:message.text};
                    }
                    let ConversationObj = chatService.createConversation(newData);
                    commit('addConversation', ConversationObj);
                    commit('updateTotalUnread', 1);
                });
            }
        }
    },
    getChatById:(context, conversationId)=>{
        return chatService.getChatById(conversationId);
    },
    clearUnread:({commit, state}, conversationId)=>{
        if(!conversationId) {
            conversationId = state.activeConversationObj.conversationId;
        }
        if(state.conversations[conversationId]){
            if(state.conversations[conversationId].unread > 0) {
                chatService.clearUnread(conversationId);
            }
            let unreadNumber = state.conversations[conversationId].unread * -1;
            commit('updateTotalUnread', unreadNumber);
            commit('clearUnreadFromConversation', conversationId);
        }
    },
    signalRAddMessage({dispatch}, messageObj){
        if(messageObj.message.type === 'file') {
            messageObj.message.unread = true;
        }
        let messageObj2 = chatService.createMessage(messageObj.message, messageObj.conversationId, true);
        dispatch('addMessage', messageObj2);
        dispatch('openChatInterface');
    },
    setActiveConversationObj:({commit, dispatch, state}, obj)=>{
        commit('setSyncStatus', true);
        commit('setActiveConversationObj', obj);
        dispatch('syncMessagesByConversationId');
        dispatch('clearUnread', obj.conversationId);//maybe we dont need it here
        dispatch('updateChatState', state.enumChatState.messages);
    },
    getAllConversations:({commit, getters, dispatch})=>{
        if(!getters.accountUser) {
            return;
        }
        chatService.getAllConversations().then(({data})=>{
            if(data.length > 0){
                data.forEach(conversation => {
                    let conversationObj = chatService.createConversation(conversation);
                    commit('addConversation', conversationObj);
                    commit('updateTotalUnread', conversationObj.unread);
                    conversation.users.forEach(user=>{
                        let userStatus = {
                            id: user.userId,
                            online: user.online
                        };
                        dispatch('setUserStatus', userStatus);
                    })
                });
            }
        });
    },
    syncMessagesByConversationId:({dispatch, state, getters, commit})=>{
        //get from server the messages by id
        let id = null;
        if(!state.activeConversationObj.conversationId) {
            //try to get conversation ID
            let conversationId = getters.getConversationIdCurrentUserId;
            if(!!conversationId){
                id = conversationId;
                commit('setActiveConversationId', id);
            }
        }else{
            id = state.activeConversationObj.conversationId;
        }

        if(!!id && !state.messages[id]){
            chatService.getMessageById(id).then(({data})=>{
                if(!data) return;
                data.reverse().forEach(message => {
                    let MessageObj = chatService.createMessage(message, id);
                    dispatch('addMessage', MessageObj);
                });
                commit('setSyncStatus', false);
            });
        }else if(state.messages[id] && (!!state.conversations[id] && state.conversations[id].unread > 0)){
            // clean messages before getting all messages from server 
            chatService.getMessageById(id).then(({data})=>{
                if(!data) return;
                commit('resetMessagesById', id);
                data.reverse().forEach(message => {
                    let MessageObj = chatService.createMessage(message, id);
                    dispatch('addMessage', MessageObj);
                });
                commit('setSyncStatus', false);
            });
        }else{
            commit('setSyncStatus', false);
        }
    },
    updateChatState:({commit}, val)=>{
        commit('changeChatState', val);
    },
    sendChatMessage:({state, dispatch, getters}, message)=>{
        //send message to server.
        let messageObj = {
            message: message,
            otherUser: state.activeConversationObj.userId,
            conversationId: state.activeConversationObj.conversationId

        };
        chatService.sendChatMessage(messageObj);

        //add message locally
        let id = state.activeConversationObj.conversationId;
        let userId = getters.accountUser.id;
        let localMessageObj = {
            userId,
            text: message,
            type: 'text',
            name: state.activeConversationObj.name,
            dateTime: new Date().toISOString(),
            fromSignalR:true,
            image: state.activeConversationObj.image,
            unreadMessage: true
        };
        localMessageObj = chatService.createMessage(localMessageObj, id);
        dispatch('addMessage', localMessageObj);

    },
    openChatInterface:({dispatch, state})=>{
        if(state.chatState === state.enumChatState.messages){
            dispatch('clearUnread');
        }
    },
    checkUnreadMessageFromSignalR({state}, obj) {
        let currentConversation = state.activeConversationObj.conversationId === obj.conversationId;
        if(currentConversation) {
            let messages = state.messages[obj.conversationId];
            messages.forEach((message) => {
                message.unreadMessage = false;   
            });
        }
    },
    uploadCapturedImage(context, formData) {
        return chatService.uploadCapturedImage(formData);
    },
};

export default {
    state,
    mutations,
    getters,
    actions
}