import { connectivityModule } from '../../services/connectivity.module'

function UserData(objInit) {
    this.userInfo = objInit.user || {};
    this.userAnswers =  createAnswertItem(objInit.answers);
    this.userQuestions = createQuestionItem(objInit.questions);
    this.userDocuments = createDocumentItem(objInit.documents);
    // this.answers = new AnswerItem(objInit.answers);
    // this.fraudScore = objInit.fraudScore;
    // this.userQueryRatio = objInit.userQueryRatio;
    // this.isSuspect = objInit.isSuspect;
    // this.isIsrael = objInit.isIsrael;
}

function QuestionItem(objInit) {
    this.id = objInit.id || 0;
    this.created = objInit.created;
    this.text = objInit.text;
    this.state = objInit.state;

}

function DocumentItem(objInit) {
    this.name = objInit.name;
    this.id = objInit.id;
    this.created = objInit.created;
    this.university = objInit.university;
    this.course = objInit.course;
    this.price = objInit.price;
    this.state = objInit.state;
}

function AnswerItem(objInit) {
    this.id = objInit.id;
    this.questionId = objInit.questionId;
    this.created = objInit.created;
    this.text = objInit.text;
    this.questionText = objInit.questionText;
    this.state = objInit.state;
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
};


export default {
    getUserData: (id) => {
        let path = "AdminUser/info?userIdentifier=" + id;
        return connectivityModule.http.get(path)
            .then((resp) => {
                console.log(resp, 'success get 20 docs');
                return createUserItem(resp);


            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    }


}