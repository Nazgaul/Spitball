import { connectivityModule } from "./connectivity.module"

function Conversation(objInit){
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.unread = objInit.unread;
    this.online = objInit.online;
    this.conversationId = objInit.conversationId;
}

function createConversation(objInit){
    return new Conversation(objInit)
}

function Message(objInit, id){
    this.userId= objInit.userId;
    this.text = objInit.text;
    this.conversationId = id;
}

function createMessage(objInit, id){
    return new Message(objInit, id)
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

export default {
    getAllConversations,
    createConversation,
    getMessageById,
    createMessage,
    sendChatMessage,
    createServerMessageObj
}