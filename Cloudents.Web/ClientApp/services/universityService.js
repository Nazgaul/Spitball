// import axios from 'axios'
// import { School } from './Dto/school.js';

// const universityInstance = axios.create({
//     baseURL: '/api/university'
// })

// export default {
//     getUni({term,page}){
//         let params = {term,page}
//         return universityInstance.get('',{params}).then(({ data }) => {
//             let result = [];
//             if (!!data.universities && data.universities.length > 0) {
//                 data.universities.forEach((uni) => {
//                     result.push(new School.University(uni));
//                 });
//             }
//             return result;
//         }, (err) => {
//             return Promise.reject(err);
//         });
//     },
//     async assaignUniversity(id){
//         return await universityInstance.post('set',{id})
//     },
//     async createUni(name){
//         await universityInstance.post('create', { name })
//         return name
//     },
// };