import axios from 'axios'
import {ENROLLED_ERROR} from '../components/pages/global/toasterInjection/componentConsts.js';

const courseInstance = axios.create({
  baseURL: '/api/course'
})

const state = {
  courseDetails: null,
}
const mutations = {
  setCourseEnrolled(state, val) {
    state.courseDetails.enrolled = val;
  },
  setCourseDetails(state, courseDetails) {
    if (courseDetails?.id) {
      state.courseDetails = new CourseDetails(courseDetails)
    } else {
      state.courseDetails = null
    }
    function CourseDetails(objInit) {
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
      this.items = objInit.documents.map(item => {
        return {
          id: item.id,
          title: item.title,
          preview: item.preview,
          documentType: item.documentType,
        }
      })
      this.sessionStarted = objInit.sessionStarted || null;
      this.studyRooms = objInit.studyRooms;
      this.documents = objInit.documents
      this.tutorName = objInit.tutorName;
      this.tutorImage = objInit.tutorImage;
      this.tutorId = objInit.tutorId;
      this.tutorCountry = objInit.tutorCountry;
      this.tutorBio = objInit.tutorBio;
      this.startTime = objInit.broadcastTime;
    }
  }
}
const getters = {
  getCourseDetails: state => state.courseDetails,
  getCourseSessions: state => state.courseDetails?.studyRooms || [],
  getIsCourseTutor: (state, getters) => state.courseDetails?.tutorId == getters.getAccountId,
  getCoursePrice: state => state.courseDetails?.price || null,
  getCourseItems: state => state.courseDetails?.items || [],
}
const actions = {
  updateCourseDetails({ commit }, courseId) {
    if (courseId) {
      courseInstance.get(`${courseId}`).then(({ data }) => {
        commit('setCourseDetails', data)
      })
    } else {
      commit('setCourseDetails', null)
    }
  },
  async updateEnrollCourse({commit,getters,dispatch}, courseId) {
    if(getters.getCoursePrice?.amount){
      let session = {
        studyRoomId: courseId
      };
      if(getters.getCourseDetails?.tutorCountry !== 'IL' ){
        let x = await dispatch('updateStudyroomLiveSessionsWithPrice', session);
        dispatch('goStripe',x)
        return;
      }else{
        let x = await dispatch('updateStudyroomLiveSessionsWithPricePayMe',session);
        location.href = x;
        return;
      }
    }
    return courseInstance.post(`${courseId}/enroll`)
      .then(() => {
        commit('setCourseEnrolled',true);
      }).catch(ex => {
        commit('setComponent',ENROLLED_ERROR);
        commit('trackException',ex);
      })
  }
}
export default {
  state,
  mutations,
  getters,
  actions
}