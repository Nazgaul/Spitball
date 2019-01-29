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
    this.phoneNumber = {value: objInit.phoneNumber ||  'Not Added', label: 'Phone Number' };
    this.university = {value: objInit.university ||  '', label: 'University' };
    this.country =  {value: objInit.country ||  '', label: 'Country' };
    this.score =  {value: objInit.score ||  0, label: 'Score' };
    this.fraudScore = {value: objInit.fraudScore ||  0, label: 'Fraud Score' };
    this.referredCount = {value: objInit.referredCount ||  0, label: 'People Referred' };
    this.balance = {value: objInit.balance ||  0, label: 'Balance' };
    this.status = {value: objInit.isActive ? false : true , label: 'Suspended' };
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
    getUserData: (id) => {
        let path = "AdminUser/info?userIdentifier=" + id;
        return connectivityModule.http.get(path)
            .then((resp) => {
                return createUserItem(resp);


            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    }


}