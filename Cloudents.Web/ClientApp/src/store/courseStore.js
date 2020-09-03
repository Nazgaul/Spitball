import { ENROLLED_ERROR,ENROLLED_ERROR_2 } from '../components/pages/global/toasterInjection/componentConsts.js';
import Vue from 'vue';

const COURSE_API = 'course';

function _createCourseEditedSections(objInit, refObj) {
  let heroSection = new HeroSection(objInit, refObj);
  clean(heroSection);
  let liveClassSection = objInit.studyRooms?.length ? objInit.studyRooms.map(c => new LiveClassSection(c)) : undefined;
  let teacherBio = new TeacherBio(objInit, refObj);
  clean(teacherBio);
  let classContent = new ClassContent(objInit, refObj);
  clean(classContent);

  let editedObject = {
    heroSection: Object.values(heroSection).some(p => (p)) ? heroSection : undefined,
    liveClassSection,
    classContent: Object.values(classContent).some(p => (p)) ? classContent : undefined,
    teacherBio: Object.values(teacherBio).some(p => (p)) ? teacherBio : undefined,
  }
  clean(editedObject)
  return editedObject
  function HeroSection(objInit, objRef) {
    this.name = objInit.name || objRef?.name;
    this.description = objInit.description || objRef?.description;
    this.image = objInit.image;
    this.button = objInit.heroButton || objRef?.heroButton;
  }
  function LiveClassSection(objInit) {
    this.id = objInit.id;
    this.name = objInit.name;
  }
  function ClassContent(objInit, objRef) {
    this.title = objInit.contentTitle || objRef.contentTitle;
    this.text = objInit.contentText || objRef.contentText;
  }
  function TeacherBio(objInit, objRef) {
    this.name = objInit.tutorName || objRef.tutorName;
    this.title = objInit.teacherTitle || objRef.teacherTitle;
    this.text = objInit.tutorBio || objRef.tutorBio;
  }
  function clean(obj) {
    for (var propName in obj) {
      if (obj[propName] === null || obj[propName] === undefined) {
        delete obj[propName];
      }
    }
  }
}
const state = {
  courseDetails: null,
  courseEditedDetails: {},
  loadingEditCourseBtn: false,
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
      this.studyRooms = objInit.studyRooms.map(session => {
        return {
          id: session.id,
          name: session.name,
          date: session.dateTime,
          onGoing: session.onGoing,
          past: new Date() > new Date(session.dateTime) && !session.onGoing,
        }
      });

      this.documents = objInit.documents;
      this.tutorImage = objInit.tutorImage;
      this.tutorId = objInit.tutorId;
      this.tutorCountry = objInit.tutorCountry;
      this.startTime = objInit.broadcastTime;



      this.heroButton = objInit.details?.heroButton;
      this.teacherTitle = objInit.details?.teacherBioTitle;
      this.tutorName = objInit.details?.teacherBioName || objInit.tutorName;
      this.tutorBio = objInit.details?.teacherBioText || objInit.tutorBio;

      this.contentText = objInit.details?.contentText;
      this.contentTitle = objInit.details?.contentTitle;
    }
  },



  setEditedDetailsByType(state, { type, val }) {
    Vue.set(state.courseEditedDetails, type, val);
  },
  initEditedDetails(state){
    state.courseEditedDetails = {};
  },
  setSessionOnGoing(state,sessionId){
    let courseSessions = [...state.courseDetails?.studyRooms];
    courseSessions.forEach(session => {
      if(session.id == sessionId){
        session.onGoing = true;
      }
    })
    Vue.set(state.courseDetails, 'studyRooms', courseSessions);
  }
}
const getters = {
  getCourseDetails: state => state.courseDetails,

  getCourseNamePreview: state => state.courseEditedDetails?.name || state.courseDetails?.name,
  getCourseDescriptionPreview: state => state.courseEditedDetails?.description || state.courseDetails?.description,
  getCourseImagePreview: state => state.courseEditedDetails?.image ? state.courseEditedDetails.previewImage : state.courseDetails?.image,
  getCourseSessionsPreview: state => {
    if (state.courseDetails?.studyRooms) {
      if (state.courseEditedDetails.studyRooms) {
        return state.courseDetails.studyRooms.map(courseSession => {
          let refSession = state.courseEditedDetails.studyRooms.find(s => s.id == courseSession.id);
          let currentSession = courseSession;
          currentSession.name = refSession?.name || courseSession.name;
          return currentSession;
        })
      } else {
        return state.courseDetails.studyRooms;
      }
    } else {
      return [];
    }
  },
  getCourseTeacherNamePreview: state => state.courseEditedDetails?.tutorName || state.courseDetails?.tutorName,
  getCourseTeacherBioPreview: state => state.courseEditedDetails?.tutorBio || state.courseDetails?.tutorBio,
  getCourseIsFull: state => state.courseDetails?.full,
  getCourseTeacherTitlePreview: state => state.courseEditedDetails?.teacherTitle || state.courseDetails?.teacherTitle,

  getCourseButtonPreview: state => state.courseEditedDetails?.heroButton || state.courseDetails?.heroButton,
  getCourseItemsContentTextPreview: state => state.courseEditedDetails?.contentText || state.courseDetails?.contentText,
  getCourseItemsContentTitlePreview: state => state.courseEditedDetails?.contentTitle || state.courseDetails?.contentTitle,

  getCourseLoadingButton: state => state.loadingEditCourseBtn,


  getNextCourseSession: (state, getters) => getters.getCourseSessionsPreview.find(s=> s.onGoing || !s.past),
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
  async updateEnrollCourse({ commit, getters, dispatch }, courseId) {
    if (getters.getCoursePrice?.amount) {
      let session = {
        studyRoomId: courseId
      };
      if (getters.getCourseDetails?.tutorCountry !== 'IL') {
        let x = await dispatch('updateStudyroomLiveSessionsWithPrice', session);
        dispatch('goStripe', x)
        return;
      } else {
        let x = await dispatch('updateStudyroomLiveSessionsWithPricePayMe', session)
          .catch(err=>{
            commit('setComponent', ENROLLED_ERROR_2);
            commit('trackException', err);
            return;
          });
        if(x){
          location.href = x;
        }
        return;
      }
    }
    return this.$axios.post(`${COURSE_API}/${courseId}/enroll`)
      .then(() => {
        commit('setCourseEnrolled', true);
      }).catch(ex => {
        commit('setComponent', ENROLLED_ERROR);
        commit('trackException', ex);
      })
  },
  updateCourseEditedInfo({ state }, courseId) {
    state.loadingEditCourseBtn = true;
    let params = _createCourseEditedSections(state.courseEditedDetails, state.courseDetails);
    return this.$axios.put(`${COURSE_API}/${courseId}/landing`, params).finally(() => {
      state.loadingEditCourseBtn = false;
    })
  }
}
export default {
  state,
  mutations,
  getters,
  actions
}