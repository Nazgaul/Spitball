import { connectivityModule } from '../../../../services/connectivity.module';

function ConversationItem(objInit) {
    this.id = objInit.id;
    this.userName = objInit.userName;
    this.tutorName = objInit.tutorName;
    this.lastMessage = new Date(objInit.lastMessage);
    this.expanded = false;
}
function createConversationItem(objInit) {
    return new ConversationItem(objInit);
}

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

const getConversationsList = function () {
    return connectivityModule.http.get(`${path}`).then((newConversationList) => {
        let arrConversationList = [];
        if (newConversationList.length > 0) {
            newConversationList.forEach((conversation) => {
                arrConversationList.push(createConversationItem(conversation));
            });
        }
        return Promise.resolve(arrConversationList);
    }, (err) => {
        return Promise.reject(err);
    });
};

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
    getConversationsList,
    getDetails,
    getMessages
};