
import { connectivityModule } from '../../../../services/connectivity.module';

function DetailsItem(objInit) {
    this.userName = objInit.userName;
    this.email = objInit.email;
    this.phoneNumber = objInit.phoneNumber;
    if (objInit.image) {
        objInit.image += "?width=50&height=50";
    }
    this.image = objInit.image;
}

function createDetailsItem(objInit) {
    return new DetailsItem(objInit);
}

function createMessageItem(objInit) {
    return new CreateMessageItem(objInit);
}

function CreateMessageItem(objInit) {
    this.userId = objInit.userId;
    this.dateTime = new Date( objInit.dateTime);
    this.type = objInit.type;
    this.name = objInit.name;
    this.text = objInit.text;
}

const path = 'AdminConversation/';

const getDetails = function (id) {
    return connectivityModule.http.get(`${path}${id}/details`).then((newConversationDetails) => {
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
                arrConversationMessages.unshift(createMessageItem(conversationMessages));
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