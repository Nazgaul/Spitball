import {connectivityModule} from '../../../../services/connectivity.module'


function PendingQuestionItem(objInit){
    this.id = objInit.id;
    this.text = objInit.text;
    this.imagesCount = objInit.imagesCount;
    this.user = {
        id: objInit.userId,
        email: objInit.email
    };
}


function createPendingQuestionItem(objInit){
    return new PendingQuestionItem(objInit);
}

const getAllQuesitons = function(){
    let path = 'AdminQuestion/Pending';
    return connectivityModule.http.get(path).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createPendingQuestionItem(question));
            });
        }
        return Promise.resolve(arrQuestions);
    }, (err)=>{
        return Promise.reject(err);
    });
};

const aproveQuestion = function(id){
    let path = 'AdminQuestion/approve';
    let idObj = {
        id: id
    };
    return connectivityModule.http.post(path, idObj).then(()=>{
        return Promise.resolve(true);
    }, (err)=>{
        return Promise.reject(err);
    });
};


export {
    getAllQuesitons,
    aproveQuestion,
}