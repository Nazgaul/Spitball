import {connectivityModule} from '../../../../services/connectivity.module'

function Answer(objInit){
    this.id = objInit.id;
    this.text = objInit.text;
    this.imagesCount = objInit.imagesCount;
}

function createAnswers(arrobjInit){
    let answers = [];
    if (!!arrobjInit && arrobjInit.length > 0) {
        arrobjInit.forEach((answer) => {
            answers.push(new Answer(answer));
        });
    } else {
        console.error("Question without answers Detected - Notify Ram");
    }
    return answers;
}

function QuestionItem(objInit){
    this.id = objInit.id;
    this.create= objInit.create;
    this.answerId = objInit.answerId;
    this.text = objInit.text;
    this.answers = createAnswers(objInit.answers);
    this.url = objInit.url;
    this.isFictive = objInit.isFictive;
    this.imagesCount = objInit.imagesCount;
}

QuestionItem.prototype.toServer = function(answerId){
    let answerToAccept = {
        questionId: this.id,
        answerId: answerId
    };
    return answerToAccept;
};

function createQuestionItem(objInit){
    return new QuestionItem(objInit);
}

const getAllQuestions = function(page){
    let path = `AdminMarkQuestion?page=${page}`;
    return connectivityModule.http.get(path).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createQuestionItem(question));
            });
        }
        return Promise.resolve(arrQuestions);
    }, ()=>{
        return Promise.reject(null);
    });
};

const acceptAnswer = function(question){
    let path = 'AdminMarkQuestion';
    return connectivityModule.http.post(path, question).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createQuestionItem(question));
            });
        }
        return Promise.resolve(arrQuestions);
    }, ()=>{
        return Promise.reject(null);
    });
};


export {
    getAllQuestions,
    acceptAnswer
}