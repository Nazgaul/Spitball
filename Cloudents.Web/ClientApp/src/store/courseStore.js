import {ENROLLED_ERROR} from '../components/pages/global/toasterInjection/componentConsts.js';
import Vue from 'vue';

const COURSE_API = 'course';

const state = {
  courseDetails: null,
  courseEditedDetails:{}
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
      this.studyRooms = objInit.studyRooms.map(session=>{
        return {
          id: session.id,
          name: session.name,
          date: session.dateTime
        }
      });

      this.documents = objInit.documents
      this.tutorName = objInit.tutorName;
      this.tutorImage = objInit.tutorImage;
      this.tutorId = objInit.tutorId;
      this.tutorCountry = objInit.tutorCountry;
      this.tutorBio = objInit.tutorBio;
      this.startTime = objInit.broadcastTime;
    }
  },



  setEditedDetailsByType(state,{type,val}){
    Vue.set(state.courseEditedDetails, type, val);
  }
}
const getters = {
  getCourseDetails: state => state.courseDetails,

  getCourseNamePreview: state => state.courseEditedDetails?.name || state.courseDetails?.name,
  getCourseDescriptionPreview: state => state.courseEditedDetails?.description || state.courseDetails?.description,
  getCourseImagePreview: state => state.courseEditedDetails?.image? state.courseEditedDetails.previewImage : state.courseDetails?.image,
  getCourseSessionsPreview: state => {
    if(state.courseDetails?.studyRooms){
      if(state.courseEditedDetails.studyRooms){
        return state.courseDetails.studyRooms.map(courseSession=>{
          let refSession = state.courseEditedDetails.studyRooms.find(s=>s.id == courseSession.id);
          let currentSession = courseSession;
          currentSession.name = refSession?.name || courseSession.name;
          return currentSession;
        })
      }else{
        return state.courseDetails.studyRooms;
      }
    }else{
      return [];
    }
  },
  getCourseTeacherNamePreview: state => state.courseEditedDetails?.tutorName || state.courseDetails?.tutorName,
  getCourseTeacherBioPreview: state => state.courseEditedDetails?.tutorBio || state.courseDetails?.tutorBio,
  getCourseIsFull: state => state.courseDetails?.full,



  getNextCourseSession: (state,getters) => {
    // TODO: get the nearest date;
    return getters.getCourseSessionsPreview[0]
  },
  getIsCourseTutor: (state, getters) => state.courseDetails?.tutorId == getters.getAccountId,
  getCoursePrice: state => state.courseDetails?.price || null,
  getCourseItems: state => state.courseDetails?.items || [],
  getIsCourseEnrolled: state => state.courseDetails?.enrolled,
}
const actions = {
  updateCourseDetails({ commit }, courseId) {
    if (courseId) {
      this.$axios.get(`${COURSE_API}/${courseId}`).then(({ data }) => {
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
    return this.$axios.post(`${COURSE_API}/${courseId}/enroll`)
      .then(() => {
        commit('setCourseEnrolled',true);
      }).catch(ex => {
        commit('setComponent',ENROLLED_ERROR);
        commit('trackException',ex);
      })
  },
}
export default {
  state,
  mutations,
  getters,
  actions
}

/*
hero:
      Vue.delete(state.roomParticipants, participantId)
      Vue.set(state.roomParticipants, participantId, participantObj);
{
  id:'123',
  image:'sdfsfsf',
  description: 'sdfs sdfsfssf sf  sdfasfs s ',
  enrollBtn: 'buy now',
}

update store object ref to real data
getters depend on the updated store object || real data
setters that set store object props && real data for preview
resx
*/ 