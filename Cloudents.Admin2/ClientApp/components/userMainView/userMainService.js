import { connectivityModule } from '../../services/connectivity.module'

//function userData(objInit) {
//    this.userInfo = createUserInfoItem(objInit.user);
//    this.userAnswers = createAnswertItem(objInit.answers);
//    this.userQuestions = createQuestionItem(objInit.questions);
//    this.userDocuments = createDocumentItem(objInit.documents);
//}

function UserInfo(objInit) {
    this.id = {value: objInit.id || 0, label: 'User ID'};
    this.name = {value: objInit.name || '', label: 'User Name'};
    this.email = {value: objInit.email || '', label: 'User Email'};
    this.emailConfirmed = {value: objInit.emailConfirmed ? 'Yes' : 'No', label: 'Email Confirmed'};
    this.phoneNumber = {
        value: objInit.phoneNumber || '--',
        label: 'Phone Number',
        showButton: !objInit.phoneNumberConfirmed,
        buttonText: "verify Phone"
    };
    this.phoneNumberConfirmed = {value: objInit.phoneNumberConfirmed ? 'Yes' : 'No', label: "Phone Confirmed"};
    this.university = {value: objInit.university || '', label: 'University'};
    this.country = {value: objInit.country || '', label: 'Country'};
    this.score = {value: objInit.score || 0, label: 'Score'};
    this.fraudScore = {value: objInit.fraudScore || 0, label: 'Fraud Score'};
    this.referredCount = {value: objInit.referredCount || 0, label: 'People Referred'};
    this.balance = {value: objInit.balance || 0, label: 'Balance'};
    this.status = {value: objInit.isActive ? false : true, label: 'Suspended'};
    this.wasSuspended = { value: objInit.wasSuspended ? true : false, label: 'Was Suspended' };
    if (objInit.joined !== null) {
        this.joined =
            {
                value: new Date(objInit.joined).toLocaleDateString(undefined, { day: 'numeric', month: 'numeric', year: 'numeric' }),
                label: "Join Date"
            };
    }
    else {
        this.joined =
            {
                value: "Never",
                label: "Join Date"
            };
    }
    if (objInit.lastOnline !== null) {
        this.lastOnline = {
            value: new Date(objInit.lastOnline).toLocaleDateString(undefined, { day: 'numeric', month: 'numeric', year: 'numeric' }),
            label: "Last On-line"
        };
    }
    else {
        this.lastOnline = {
            value: "Never",
            label: "Last On-line"
        };
    }
    this.isTutor = {
        value: objInit.isTutor,
        label: 'Is Tutor'
    };
    
}

function createUserInfoItem(data) {
    return new UserInfo(data);
}

function QuestionItem(objInit) {
    this.id = objInit.id || 0;
    this.create = objInit.created;
    this.text = objInit.text;
    this.state = objInit.state ? objInit.state.toLowerCase() : 'ok';
}

function DocumentItem(objInit) {
    this.name = objInit.name;
    this.id = objInit.id;
    this.create = objInit.created;
    this.university = objInit.university;
    this.course = objInit.course;
    this.price = objInit.price;
    this.state = objInit.state ? objInit.state.toLowerCase() : 'ok';
    this.preview = objInit.preview || '';
    this.siteLink = objInit.siteLink || 'Not specified';
}

function AnswerItem(objInit) {
    this.id = objInit.id;
    this.questionId = objInit.questionId;
    this.create = objInit.created;
    this.text = objInit.text;
    this.questionText = objInit.questionText;
    this.state = objInit.state ? objInit.state.toLowerCase() : 'ok';
}

function PurchasedDocItem(objInit) {
    this.id = objInit.documentId  || 'no ID';
    this.title = objInit.title  || 'not specified';
    this.university = objInit.university || 'not specified';
    this.class = objInit.class  || 'not specified';
    this.price = Math.abs(objInit.price ) || 'not specified';
}

function createQuestionItem(data) {
    return data.map((item) => {
        return new QuestionItem(item);
    });
}

function createDocumentItem(data) {
    return data.map((item) => {
        return new DocumentItem(item);
    });
}

function createAnswerItem(data) {
    return data.map((item) => {
        return new AnswerItem(item);
    });
}
function createPurchasedDocItem(data) {
    return data.map((item) => {
        return new PurchasedDocItem(item);
    });
}

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

function SessionsItem(objInit) {
    this.created = new Date(objInit.created);
    this.tutor = objInit.tutor;
    this.student = objInit.student;
    this.duration = objInit.duration;
}

function createSessionsItem(objInit){
    return new SessionsItem(objInit);
};

export default {
    getUserData: (id) => {
        let path = `AdminUser/info?userIdentifier=${encodeURIComponent(id)}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                if(resp){
                    return createUserInfoItem(resp);
                }else{
                    return false;
                }

            });
    },
    getUserDocuments: (id, page) => {
        let path = `AdminUser/documents?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createDocumentItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return error;
            });
    },
    getUserQuestions: (id, page) => {
        let path = `AdminUser/questions?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createQuestionItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return error;
            });
    },
    getUserAnswers: (id, page) => {
        let path = `AdminUser/answers?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createAnswerItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return error;
            });
    },
    getPurchasedDocs: (id, page) => {
        let path = `AdminUser/purchased?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createPurchasedDocItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return error;
            });
    },
    getUserConversations:(id) => {
        const path = `AdminUser/chat`;
        return connectivityModule.http.get(`${path}?id=${id}`).then((newConversationList) => {
            let arrConversationList = [];
            if (newConversationList.length > 0) {
                newConversationList.forEach((conversation) => {
                    arrConversationList.push(createConversationItem(conversation));
                });
            }
            return arrConversationList;
        }, (err) => {
            return err;
        });
    },
    getUserSessions:(id) => {
        let path = `AdminUser/sessions`;
        return connectivityModule.http.get(`${path}?id=${id}`).then((newSessionsList) => {
            let arrSessionsList = [];
            if (newSessionsList.length > 0) {
                newSessionsList.forEach((sessions) => {
                    arrSessionsList.push(createSessionsItem(sessions));
                });
            }
            return arrSessionsList;
        }, (err) => {
            return err;
        });

    },
    verifyPhone: (data) => {
        let path = `AdminUser/verify`;
        return connectivityModule.http.post(path, data)
            .then(() => {

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return error;
            });
    }
}