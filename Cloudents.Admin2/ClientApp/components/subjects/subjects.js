import axios from 'axios';

let subjectInstance = axios.create({
    baseURL: '/api/AdminSubject',
    requestInterceptor: (config, { store }) => {
        let qs = require('qs')
        config.paramsSerializer = (params) => {
          return qs.stringify(params, { arrayFormat: 'brackets' })
        }
        return config
    }
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

export default {
    getSubjects: () => subjectInstance.get().then(createSubjects),
    getSubject: () => subjectInstance.get().then(({data}) => createSubject(data)),
    addSubject: params => subjectInstance.post('', params),
    editSubject: params => subjectInstance.put('', params),
    deleteSubject: () => subjectInstance.delete(),
}