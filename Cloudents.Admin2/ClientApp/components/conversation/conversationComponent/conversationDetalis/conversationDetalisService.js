import { connectivityModule } from '../../../../services/connectivity.module';

function ConversationFilters(objInit){
    this.assignTo = objInit.assignTo;
    this.status = objInit.status;
    this.waitingFor = objInit.waitingFor;
}

function createConversationFilters(objInit){
    return new ConversationFilters(objInit);
}

function GroupStatus(objInit){
    this.group = objInit.group || 'new';
    this.id = objInit.id  || null;
    this.name = objInit.name || null;
}

function createGroupStatus(objInit){
    return new GroupStatus(objInit);
}

function ConversationItem(objInit) {
    this.id = objInit.id;
    this.userName = objInit.userName;
    this.userId = objInit.userId;
    this.userPhoneNumber = objInit.userPhoneNumber;
    this.userEmail = objInit.userEmail;
    this.tutorName = objInit.tutorName;
    this.tutorId = objInit.tutorId;
    this.tutorPhoneNumber = objInit.tutorPhoneNumber;
    this.tutorEmail = objInit.tutorEmail;
    this.autoStatus = objInit.autoStatus;
    this.status = createGroupStatus(objInit.status ? objInit.status : {});
    this.lastMessage = new Date(objInit.lastMessage);
    this.studyRoomExists = objInit.studyRoomExists;
    this.expanded = false;
    this.hoursFromLastMessage = objInit.hoursFromLastMessage;
    this.assignTo = objInit.assignTo;
    this.requestFor = objInit.requestFor;
}
// function createConversationItem(objInit) {
//     return new ConversationItem(objInit);
// }

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

const getFilters = (id, filters) => {    
    let userId = id ? `id=${id}&`: '';
    return connectivityModule.http.get(`${path}?${userId}${filters}`).then((filtersConversations)=>{
        let filtersArray = [];   
        if (filtersConversations.length > 0) {
            filtersConversations.forEach((conversation) => {
                filtersArray.push(new ConversationItem(conversation));
            });
        }
        return filtersArray;
    });
};

const setConversationsStatus = (id,status) => {
    return connectivityModule.http.post(`${path}${id}/status`,status).then((res)=>{
        return res;
    });
};

const getConversationsListPage = function (id, page, filter) {
    let userId = id ? `id=${id}&`: '';
    return connectivityModule.http.get(`${path}?${userId}page=${page}&${filter}`).then((newConversationList) => {
        let arrConversationList = [];
        if (newConversationList.length > 0) {
            newConversationList.forEach((conversation) => {
                arrConversationList.push(new ConversationItem(conversation));
            });
        }
        return arrConversationList;
    });
    
};

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
                arrConversationMessages.push(createMessageItem(conversationMessages));
            });
        }
        return arrConversationMessages;
    });
};

const getFiltersParams = () => {
    return connectivityModule.http.get(`${path}params`).then((res)=>{
        return createConversationFilters(res);
    });
};

const setAssignTo = (id, assignTo) => {    
    return connectivityModule.http.post(`${path}${id}/assignTo`, {assignTo});
};

export {
    getDetails,
    getMessages,
    setConversationsStatus,
    getConversationsListPage,
    getFiltersParams,
    setAssignTo,
    getFilters,
    createGroupStatus
};