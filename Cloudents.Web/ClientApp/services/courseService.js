import axios from 'axios'
import { School } from './Dto/school.js';

const courseInstance = axios.create({
   baseURL: '/api/course'
})

export default {
   async getSubject(params) {
      if (!params) return
      let { data } = await courseInstance.get('subject', { params })
      return data
   },
   async assaignCourse(courseName) {
      return await courseInstance.post('set', courseName)
   },
   async createCourse(course) {
      let { data } = await courseInstance.post('create', course)
      let createdCourse = {
         name: data.name,
         isFollowing: true,
         isTeaching: false,
         isPending: true,
      };
      return new School.Course(createdCourse);
   },
   async deleteCourse(courseName) {
      return await courseInstance.delete('',{params: {name: encodeURIComponent(courseName)}})
   },
   async teachCourse(name) {
      return await courseInstance.post('teach',{name})
   },
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
   }
}