import { connectivityModule } from "./connectivity.module"

function Conversation(objInit){
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.unread = objInit.unread;
    this.online = objInit.online;
    this.conversationId = objInit.userId + (Math.random()*9999 | 0);
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

const getAllConversations = () => {
    return connectivityModule.http.get(`Chat`);
}   

const getMessageById = (id) => {
    return connectivityModule.http.get(`Chat/${id}`);
}

export default {
    getAllConversations,
    createConversation,
    getMessageById,
    createMessage
}