import axios from 'axios';

let subjectInstance = axios.create({
    baseURL: '/api/AdminSubject'
})

function getSubjects(){
    return subjectInstance.get()
 }
export  {
    getSubjects
}