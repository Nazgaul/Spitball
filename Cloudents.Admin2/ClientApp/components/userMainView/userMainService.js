import {connectivityModule} from '../../services/connectivity.module'

function UserData(objInit){
    this.userInfo = objInit.user || {};
    this.userQuestions = new QuestionItem(objInit);
    this.documents = new DocumentItem(objInit) || {};
    this.answers = new AnswerItem(objInit);
    this.fraudScore = objInit.fraudScore;
    this.userQueryRatio = objInit.userQueryRatio;
    this.isSuspect = objInit.isSuspect;
    this.isIsrael = objInit.isIsrael;
}

function QuestionItem(objInit){
    this.id = objInit.id;
    this.created= objInit.created;
    this.text = objInit.text;

}

function DocumentItem(objInit){
    this.name= objInit.name
    this.id = objInit.id;
    this.created= objInit.created;
    this.university= objInit.university;
    this.course = objInit.course;
    this.price = objInit.price;

}
function AnswerItem(objInit){
    this.id = objInit.id;
    this.questionId = objInit.questionId;
    this.created= objInit.created;
    this.text = objInit.text;

}




function createUserItem(objInit){
    return new UserData(objInit);
}
export default {
   getUserData :  (id)=> {
        let path = "AdminUser/info" + id;
        return connectivityModule.http.get(path)
            .then((resp) => {
                console.log(resp, 'success get 20 docs');
                resp.forEach((item) => {
                    return createUserItem(item)
                });
                return Promise.resolve(resp)
            }, (error) => {
                console.log(error, 'error get 20 docs');
                return Promise.reject(error)
            })
    }


}