import axios from 'axios'

const courseInstance = axios.create({
   baseURL:'/api/course'
})

const state = {
   courseDetails:null,
}
const mutations = {
   setCourseEnrolled(state,val){
      state.courseDetails.enrolled = val;
   },
   setCourseDetails(state,courseDetails){
      if(courseDetails?.id){
         state.courseDetails = new CourseDetails(courseDetails)
      }else{
         state.courseDetails = null
      }
      function CourseDetails(objInit){
         this.id = objInit.id; 
         this.name = objInit.name;
         this.image = objInit.image;
         this.enrolled = objInit.enrolled;
         this.full = objInit.full;
         this.description = objInit.description; 
         this.price = {
            amount: objInit.price?.amount,
            currency: objInit.price?.currency
         }
         
         this.sessionStarted = objInit.sessionStarted || null;
         this.studyRooms = objInit.studyRooms;
         this.documents = objInit.documents

         this.tutorName = objInit.tutorName; 
         this.tutorImage = objInit.tutorImage;
         this.tutorId = objInit.tutorId; 
         this.tutorCountry = objInit.tutorCountry;
         this.tutorBio = objInit.tutorBio;
         
         this.startTime = objInit.startTime || '2020-07-30T09:15:00Z';
         this.nextEvents = [] // need to delete after
      }
   }
}
const getters = {
   getCourseDetails: state => state.courseDetails,
   getCourseSessions: state => state.courseDetails?.studyRooms || [],
   getIsCourseTutor: (state,getters) => state.courseDetails?.tutorId == getters.getAccountId,
   getCoursePrice: state => state.courseDetails?.price || null,
}
const actions = {
   updateCourseDetails({commit},courseId){
      if(courseId){
         courseInstance.get(`${courseId}`).then(({data})=>{
            commit('setCourseDetails',data)
         })
      }else{
         commit('setCourseDetails',null)
      }
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}