import { connectivityModule } from '../../../../services/connectivity.module'


function SubjectItem(objInit){
    this.id = objInit.id;
    this.subject = objInit.subject;
}

function createSubjectItem(objInit){
    return new SubjectItem(objInit);
}

const getSubjectList = function(){
    let path = "AdminQuestion/subject";
    let subjectsToReturn = [];
    return connectivityModule.http.get(path).then((subjects)=>{
        if (subjects.length > 0){
            subjects.forEach((subject)=>{
                subjectsToReturn.push(createSubjectItem(subject)); 
            });
        }
        return Promise.resolve(subjectsToReturn);
    },(err)=>{
        return Promise.reject(err);
    });
};

const addQuestion = function(course, text, country, uni){
    let path = "AdminQuestion";
    let questionData = {
        course,
        text,
        country,
    };
    return connectivityModule.http.post(path, questionData).then(()=>{
        return Promise.resolve();
    },(err)=>{
        return Promise.reject(err);
    });
};

export{
    getSubjectList,
    addQuestion
}