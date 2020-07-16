import axios from 'axios'
import { School } from './Dto/school.js';

const courseInstance = axios.create({
   baseURL: '/api/course'
})

export default {
   getCourse(val){
      let path = val ? `search?term=${val.term}&page=${val.page}` : `search`;
      return courseInstance.get(`${path}`).then(({data}) => {
          let result = [];
          if(!!data.courses && data.courses.length > 0) {
              data.courses.forEach((course) => {
                  result.push(new School.Course(course));
              });
          }
          return result;
      }, (err) => {
          return Promise.reject(err);
      });
   },
}