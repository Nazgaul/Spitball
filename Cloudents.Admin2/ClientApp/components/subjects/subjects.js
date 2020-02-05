import axios from 'axios';

let subjectInstance = axios.create({
    baseURL: '/api/AdminSubject'
})

function Subject(objInit) {
    this.id = objInit.id;
    this.heName = objInit.heName;
    this.enName = objInit.enName;
}

function createSubject(subject) {
    return new Subject(subject) 
}

function createSubjects({data}) {
    let subjectsObj = [], i;

    for (i = 0; i < data.length; i++) {
        subjectsObj.push(createSubject(data[i]));
    }

    return subjectsObj;
}

function getSubjects(){
    return subjectInstance.get().then(createSubjects)
}

export default {
    getSubjects: () => subjectInstance.get().then(createSubjects)
}