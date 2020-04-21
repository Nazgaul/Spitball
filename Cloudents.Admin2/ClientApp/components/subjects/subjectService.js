import axios from 'axios';

let subjectInstance = axios.create({
    baseURL: '/api/AdminSubject',
})

function Subject(objInit) {
    this.id = objInit.id;
    this.name = objInit.name;
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

export default {
    addSubject: params => subjectInstance.post('', params),
    getSubjects: () => subjectInstance.get().then(createSubjects),
    // getSubject: () => subjectInstance.get().then(({data}) => createSubject(data)),
    editSubject: params => subjectInstance.put('', params),
    deleteSubject: id => subjectInstance.delete(`${id}`),
}