import { connectivityModule } from "./connectivity.module"

function Conversation(objInit){
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.unread = objInit.unread || 0;
    this.online = objInit.online || true;
    this.conversationId = objInit.conversationId;
    this.dateTime = objInit.dateTime || new Date().toISOString();
    this.image = objInit.image;
    this.studyRoomId = objInit.studyRoomId;
    this.lastMessage = objInit.lastMessage
}

function createConversation(objInit){
    return new Conversation(objInit)
}

function TextMessage(objInit, id){
    this.userId= objInit.userId;
    this.text = objInit.text;
    this.conversationId = id;
    this.type = objInit.type;
    this.dateTime = objInit.dateTime || new Date().toISOString();
    this.name = objInit.name;
    this.image = objInit.image;
}
function FileMessage(objInit, id){
    this.userId= objInit.userId;
    this.conversationId = id;
    this.src = objInit.src;
    this.href = objInit.href;
    this.type = objInit.type;
    this.dateTime = objInit.dateTime || new Date().toISOString();
    this.name = objInit.name;
    this.image = objInit.image;
}

function activeConversationObj(objInit){
        this.userId = objInit.userId || null;
        this.conversationId = objInit.conversationId || null;
        this.name = objInit.name || null;
        this.image = objInit.image || null;
}

function createActiveConversationObj(objInit){
    return new activeConversationObj(objInit);
}


function createMessage(objInit, id){
    if(objInit.type === 'text'){
        return new TextMessage(objInit, id)
    }else{
        return new FileMessage(objInit, id)
    }
}

function serverMessageObj(objInit){
    this.message = objInit.message;
    this.otherUser = objInit.otherUser;
}

function createServerMessageObj(objInit){
    return new serverMessageObj(objInit)
}

const getAllConversations = () => {
    return connectivityModule.http.get(`Chat`);
}  

const getChatById = (id) => {
    return connectivityModule.http.get(`Chat/conversation/${id}`);
}

const getMessageById = (id) => {
    return connectivityModule.http.get(`Chat/${id}`);
}

const sendChatMessage = (messageObj) => {
    return connectivityModule.http.post(`Chat`, messageObj);
}

const clearUnread = (otherUserId) => {
    let serverObj = {
        otherUserId:otherUserId
    }
    return connectivityModule.http.post(`Chat/read`, serverObj);
}

export default {
    getAllConversations,
    getChatById,
    createConversation,
    getMessageById,
    createMessage,
    sendChatMessage,
    createServerMessageObj,
    clearUnread,
    createActiveConversationObj
}