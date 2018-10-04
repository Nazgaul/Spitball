import {connectivityModule} from '../../../../services/connectivity.module'

function QuestionItem(objInit){
    this.questionId = objInit.questionId;
    this.answerId = objInit.answerId;
    this.questionText = objInit.questionText;
    this.answerText = objInit.answerText;
    this.url = objInit.url;
    this.isFictive = objInit.isFictive;
}

QuestionItem.prototype.toServer = function(){
    let answerToAccept = {
        questionId: this.questionId,
        answerId: this.answerId
    }
    return answerToAccept;
}

function createQuestionItem(objInit){
    return new QuestionItem(objInit);
}

const getAllQuesitons = function(){
    let path = 'AdminMarkQuestion'
    return connectivityModule.http.get(path).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createQuestionItem(question));
            })
        }
        return Promise.resolve(arrQuestions)
    }, (err)=>{
        return Promise.reject(null)
    })
}

const acceptAnswer = function(question){
    let path = 'AdminMarkQuestion';
    return connectivityModule.http.post(path, question).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createQuestionItem(question));
            })
        }
        return Promise.resolve(arrQuestions)
    }, (err)=>{
        return Promise.reject(null)
    })
}


export {
    getAllQuesitons,
    acceptAnswer
}