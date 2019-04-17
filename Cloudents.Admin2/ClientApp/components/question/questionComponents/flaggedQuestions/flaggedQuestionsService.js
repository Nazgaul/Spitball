import {connectivityModule} from '../../../../services/connectivity.module'


function FlaggedQuestionItem(objInit){
    this.id = objInit.id;
    this.reason = objInit.reason;
    this.flaggedUserEmail = objInit.flaggedUserEmail || "none";
    this.text = objInit.text || '';

}


function createFlaggedQuestionItem(objInit){
    return new FlaggedQuestionItem(objInit);
}

const getAllQuesitons = function(){
    let path = 'AdminQuestion/flagged';
    return connectivityModule.http.get(path).then((questions)=>{
        let arrQuestions = [];
        if(questions.length > 0){
            questions.forEach(function(question){
                arrQuestions.push(createFlaggedQuestionItem(question));
            });
        }
        return Promise.resolve(arrQuestions);
    }, (err)=>{
        return Promise.reject(err);
    });
};

const unflagQuestion = function(id){
    let path = 'AdminQuestion/unflag';
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
    unflagQuestion,
}