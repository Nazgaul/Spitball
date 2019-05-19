﻿
import { connectivityModule } from '../../../../services/connectivity.module';

function DetailsItem(objInit) {
    this.userName = objInit.userName;
    this.email = objInit.email;
    this.phoneNumber = objInit.phoneNumber;
}

function createDetailsItem(objInit) {
    return new DetailsItem(objInit);
}

function createMessageItem(objInit) {
    return new CreateMessageItem(objInit);
}

function CreateMessageItem(objInit) {
    this.userId = objInit.userId;
    this.dateTime = objInit.dateTime;
    this.type = objInit.type;
    this.name = objInit.name;
    this.text = objInit.text;
}

const path = 'AdminConversation/';

const getDetails = function (id) {
    return connectivityModule.http.get(`${path}/details?id=${id}`).then((newConversationDetails) => {
        let arrConversationDetails = [];
        if (newConversationDetails.length > 0) {
            newConversationDetails.forEach((conversationDetails) => {
                arrConversationDetails.push(createDetailsItem(conversationDetails));
            });
        }
        return Promise.resolve(arrConversationDetails);
    }, (err) => {
        return Promise.reject(err);
    });
};

const getMessages = function (id) {
    return connectivityModule.http.get(`${path}${id}`).then((newConversationMessages) => {
        let arrConversationMessages = [];
        if (newConversationMessages.length > 0) {
            newConversationMessages.forEach((conversationMessages) => {
                arrConversationMessages.push(createMessageItem(conversationMessages));
            });
        }
        return Promise.resolve(arrConversationMessages);
    }, (err) => {
        return Promise.reject(err);
    });
};



export {
    getDetails,
    getMessages
};