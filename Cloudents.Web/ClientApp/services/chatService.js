import axios from 'axios'

const chatInstance = axios.create({
    baseURL:'/api/chat'
})

import { connectivityModule } from "./connectivity.module";
import { LanguageService } from './language/languageService';

function createLastImageMsg() {
    return `<img src="${require('../components/chat/images/photo-camera-small.png')}" /><span>${LanguageService.getValueByKey('chat_photo')}</span>`;
}

function createConversationId(arrIds){
    return arrIds.sort((a, b) => a - b).join('_');
}

function Conversation(objInit){
    if(objInit.users !== undefined){
        let isRoom = objInit.users.length > 1; 
        this.image = isRoom? '': objInit.users[0].image;
        this.online = false;
        this.name = objInit.users.map(u=>u.name).join(" ,");
    }else{
        this.image = objInit.image;
        this.online = objInit.online;
        this.name = objInit.name;
    }
    this.users = objInit.users || [];
    this.unread = objInit.unread;
    this.conversationId = objInit.conversationId;
    this.lastMessage = objInit.lastMessage || createLastImageMsg();
    this.dateTime = objInit.dateTime || new Date().toISOString();
}

function createConversation(objInit){
    return new Conversation(objInit);
}

function ConversationTutor(objInit){
    this.id = objInit.id;
    this.name = objInit.name;
    this.image = objInit.image;
    this.studyRoomId = objInit.studyRoomId || null;
    this.email = objInit.email;
    this.phoneNumber = objInit.phoneNumber;
    this.calendar = objInit.calendar;
}
function TextMessage(objInit, id, fromSignalR){
    this.userId= objInit.userId;
    this.text = objInit.text;
    this.conversationId = id;
    this.type = objInit.type;
    this.dateTime = objInit.dateTime || new Date().toISOString();
    this.name = objInit.name;
   // this.image = objInit.image;
    this.fromSignalR = fromSignalR || false;
    this.unreadMessage = objInit.unreadMessage || objInit.unread;
    this.isDummy = objInit.isDummy || false;
}
function FileMessage(objInit, id, fromSignalR){
    this.userId= objInit.userId;
    this.conversationId = id;
    this.src = objInit.src;
    this.href = objInit.href;
    this.type = objInit.type;
    this.dateTime = objInit.dateTime || new Date().toISOString();
    this.name = objInit.name;
    this.image = objInit.image;
    this.fromSignalR = fromSignalR || false;
    this.unreadMessage = objInit.unreadMessage || objInit.unread;
}

function ActiveConversationObj(objInit){
    this.userId = objInit.userId || null;
    this.conversationId = objInit.conversationId || null;
    this.name = objInit.name || null;
    this.image = objInit.image || null;
}

function createActiveConversationObj(objInit){
    return new ActiveConversationObj(objInit);
}


function createMessage(objInit, id, fromSignalR){
    if(objInit.type === 'text'){
        return new TextMessage(objInit, id, fromSignalR);
    }else{
        return new FileMessage(objInit, id, fromSignalR);
    }
}

const getAllConversations = () => {
    return connectivityModule.http.get(`Chat`);
};

const getChatById = (id) => {
    if(!id) return Promise.reject();
    return connectivityModule.http.get(`Chat/conversation/${id}`);
};

const getMessageById = (id) => {
    return connectivityModule.http.get(`Chat/${id}`);
};

const sendChatMessage = (messageObj) => {
    return connectivityModule.http.post(`Chat`, messageObj);
};

const clearUnread = (conversationId) => {
    let serverObj = {
        conversationId:conversationId
    };
    return connectivityModule.http.post(`Chat/read`, serverObj);
};

const uploadCapturedImage = (formData)=>{
    return connectivityModule.http.post(`Chat/uploadForm`, formData);
};

export default {
    getAllConversations,
    getChatById,
    createConversation,
    getMessageById,
    createMessage,
    sendChatMessage,
    clearUnread,
    createActiveConversationObj,
    createConversationId,
    createLastImageMsg,
    uploadCapturedImage,
    async getConversationTutor(id){ 
        let {data} = await chatInstance.get(`conversation/${id}/tutor`);
        return new ConversationTutor(data)
    }
}