import { connectivityModule } from '../../services/connectivity.module'

function UserData(objInit) {
    this.userInfo = createUserInfoItem(objInit.user);
    this.userAnswers =  createAnswertItem(objInit.answers);
    this.userQuestions = createQuestionItem(objInit.questions);
    this.userDocuments = createDocumentItem(objInit.documents);
}

function UserInfo(objInit) {
    this.id=  {value: objInit.id || 0, label: 'User ID' };
    this.name = {value: objInit.name || '', label: 'User Name' };
    this.email =  {value: objInit.email || '', label: 'User Email' };
    this.emailConfirmed =  {value: objInit.emailConfirmed ? 'Yes' : 'No', label: 'Email Confirmed' };
    this.phoneNumber = {value: objInit.phoneNumber ||  '--', label: 'Phone Number', showButton: !objInit.phoneNumberConfirmed, buttonText:"verify Phone" };
    this.phoneNumberConfirmed = {value: objInit.phoneNumberConfirmed ? 'Yes' : 'No', label:"Phone Confirmed"};
    this.university = {value: objInit.university ||  '', label: 'University' };
    this.country =  {value: objInit.country ||  '', label: 'Country' };
    this.score =  {value: objInit.score ||  0, label: 'Score' };
    this.fraudScore = {value: objInit.fraudScore ||  0, label: 'Fraud Score' };
    this.referredCount = {value: objInit.referredCount ||  0, label: 'People Referred' };
    this.balance = {value: objInit.balance ||  0, label: 'Balance' };
    this.status = {value: objInit.isActive ? false : true , label: 'Suspended' };
    this.wasSuspended = {value: objInit.wasSuspended ? true : false, label : 'Was Suspended' };
    this.joined = {value: new Date(objInit.joined).toLocaleString(), label:"Join Date"}
}

function createUserInfoItem(data) {
    return new UserInfo(data);
}

function QuestionItem(objInit) {
    this.id = objInit.id || 0;
    this.create = objInit.created;
    this.text = objInit.text;
    this.state =  objInit.state ? objInit.state.toLowerCase() : 'ok';
}

function DocumentItem(objInit) {
    this.name = objInit.name;
    this.id = objInit.id;
    this.create = objInit.created;
    this.university = objInit.university;
    this.course = objInit.course;
    this.price = objInit.price;
    this.state =  objInit.state ? objInit.state.toLowerCase() : 'ok';
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

function createUserItem(objInit) {
    return new UserData(objInit);
}

function createQuestionItem(data) {
    return data.map((item) => {
        return new QuestionItem(item)
    });
}
function createDocumentItem(data) {
    return data.map((item) => {
        return new DocumentItem(item)
    });
}
function createAnswertItem(data) {
    return data.map((item) => {
        return new AnswerItem(item)
    });
}


export default {
    getUserData: (id, page) => {
        let path = `AdminUser/info?userIdentifier=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createUserInfoItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
    getUserDocuments: (id, page) => {
        let path = `AdminUser/documents?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createDocumentItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
    getUserQuestions: (id, page) => {
        let path = `AdminUser/questions?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createQuestionItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
    getUserAnswers: (id, page) => {
        let path = `AdminUser/answers?id=${id}&page=${page}`;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createAnswertItem(resp);

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
    verifyPhone: (data) => {
        let path = `AdminUser/verify`;
        return connectivityModule.http.post(path, data)
            .then((resp) => {

            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    },
}