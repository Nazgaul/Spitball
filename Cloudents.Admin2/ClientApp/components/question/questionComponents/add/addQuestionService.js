import { connectivityModule } from '../../../../services/connectivity.module'


function SubjectItem(objInit){
    this.id = objInit.id;
    this.subject = objInit.subject;
}

function createSubjectItem(objInit){
    return new SubjectItem(objInit)
}

const getSubjectList = function(){
    let path = "AdminQuestion/subject"
    let subjectsToReturn = [];
    return connectivityModule.http.get(path).then((subjects)=>{
        if (subjects.length > 0){
            subjects.forEach((subject)=>{
                subjectsToReturn.push(createSubjectItem(subject)); 
            })
        }
        return Promise.resolve(subjectsToReturn)
    },(err)=>{
        console.log(err)
    })
}

export{
    getSubjectList
}