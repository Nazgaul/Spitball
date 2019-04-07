import { connectivityModule } from "./connectivity.module"

function Conversation(objInit){
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.unread = objInit.unread;
    this.online = objInit.online;
    this.conversationId = objInit.conversationId;
    this.dateTime = objInit.dateTime || new Date().toISOString();
}

function createConversation(objInit){
    return new Conversation(objInit)
}

function TextMessage(objInit, id){
    this.userId= objInit.userId;
    this.text = objInit.text;
    this.conversationId = id;
    this.type = objInit.type;
}
function FileMessage(objInit, id){
    this.userId= objInit.userId;
    this.conversationId = id;
    this.src = objInit.src;
    this.href = objInit.href;
    this.type = objInit.type;
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

const getMessageById = (id) => {
    return connectivityModule.http.get(`Chat/${id}`);
}

const sendChatMessage = (messageObj) => {
    return connectivityModule.http.post(`Chat`, messageObj);
}

const clearUnread = (otherUserId) => {
    return connectivityModule.http.post(`Chat/read?otherUser=${otherUserId}`);
}

export default {
    getAllConversations,
    createConversation,
    getMessageById,
    createMessage,
    sendChatMessage,
    createServerMessageObj,
    clearUnread
}