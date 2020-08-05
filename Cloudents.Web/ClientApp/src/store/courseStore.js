import axios from 'axios'
import {ENROLLED_ERROR} from '../components/pages/global/toasterInjection/componentConsts.js';
import Moment from 'moment';

const courseInstance = axios.create({
  baseURL: '/api/course'
})

courseInstance.interceptors.response.use(
  response => response,
  error => {
    if(error.response.status === 401){
      global.location = '/?authDialog=register';
    } else if(error.response.status === 404) {
      global.location = '/error/notfound';
    } else{
      return Promise.reject(error);
    }
 }
);

const state = {
  courseDetails: null,
  scheduledClasses: [],
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
  setScheduledClasses(state,courses){
    let colors = ['#4c59ff', '#41c4bc', '#4094ff', '#ff6f30', '#ebbc18', '#69687d', 
    '#1b2441','#5833CF', '#4DAF50', '#995BEA', '#074B8F', '#860941', '#757575', '#317CA0']
    
    courses.forEach((course,index) => {
      course.sessions.forEach(session => {
        state.scheduledClasses.push({
          courseId: course.id,
          courseName: course.name,
          studentsCount: course.studentsCount,
          id: session.id,
          name: session.name,
          date: session.dateTime,
          start: Moment(session.dateTime).format('YYYY-MM-DD HH:mm'),
          end: Moment(session.dateTime).add('1','h').format('YYYY-MM-DD HH:mm'),
          color: colors[index],
        })
      })
    })
  }
}
const getters = {
  getCourseDetails: state => state.courseDetails,
  getCourseSessions: state => state.courseDetails?.studyRooms || [],
  getNextCourseSession: (state,getters) => {
    // TODO: get the nearest date;
    return getters.getCourseSessions[0]
  },
  getIsCourseTutor: (state, getters) => state.courseDetails?.tutorId == getters.getAccountId,
  getCoursePrice: state => state.courseDetails?.price || null,
  getCourseItems: state => state.courseDetails?.items || [],
  getIsCourseEnrolled: state => state.courseDetails?.enrolled,
  getScheduledClasses: state => state.scheduledClasses,
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
  },
  updateScheduledClasses({commit}){
    commit('setScheduledClasses',fakeCourses)
  }
}
export default {
  state,
  mutations,
  getters,
  actions
}

const fakeCourses = [
  {
    id: 1,
    name: 'Why Grit, Persistence, and Hard Work Matters',
    studentsCount: 25,
    sessions: [
      {
        name: 'Learn how to effectively search for a quality online',
        dateTime: '2020-08-03T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Network on Linked In 2',
        dateTime: '2020-08-05T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Network on Linked In 2',
        dateTime: '2020-08-26T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 2,
    name: 'Network on Linked In to find work',
    studentsCount: 10,
    sessions: [
      {
        name: 'Network on Linked In 1',
        dateTime: '2020-08-25T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 3,
    name: 'Linked In to find work',
    studentsCount: 1,
    sessions: [
      {
        name: 'on Linked on Linked In 1',
        dateTime: '2020-08-29T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 2',
        dateTime: '2020-08-22T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 3',
        dateTime: '2020-08-15T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 4',
        dateTime: '2020-08-10T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 4,
    name: 'Linsdf ked In to find work',
    studentsCount: 1,
    sessions: [
      {
        name: 'on Linked on Linked In 1',
        dateTime: '2020-08-29T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 2',
        dateTime: '2020-08-22T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 3',
        dateTime: '2020-08-15T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 4',
        dateTime: '2020-08-10T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 5,
    name: 'Color palette for every course:',
    studentsCount: 1,
    sessions: [
      {
        name: 'Color palette for every course: 1',
        dateTime: '2020-08-11T12:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 2',
        dateTime: '2020-08-18T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 3',
        dateTime: '2020-08-06T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 4',
        dateTime: '2020-08-14T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 1,
    name: 'Why Grit, Persistence, and Hard Work Matters',
    studentsCount: 25,
    sessions: [
      {
        name: 'Learn how to effectively search for a quality online',
        dateTime: '2020-08-03T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Network on Linked In 2',
        dateTime: '2020-08-05T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Network on Linked In 2',
        dateTime: '2020-08-26T10:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 2,
    name: 'Network on Linked In to find work',
    studentsCount: 10,
    sessions: [
      {
        name: 'Network on Linked In 1',
        dateTime: '2020-08-25T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 3,
    name: 'Linked In to find work',
    studentsCount: 1,
    sessions: [
      {
        name: 'on Linked on Linked In 1',
        dateTime: '2020-08-29T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 2',
        dateTime: '2020-08-22T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 3',
        dateTime: '2020-08-15T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 4',
        dateTime: '2020-08-10T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 4,
    name: 'Linsdf ked In to find work',
    studentsCount: 1,
    sessions: [
      {
        name: 'on Linked on Linked In 1',
        dateTime: '2020-08-29T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 2',
        dateTime: '2020-08-22T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 3',
        dateTime: '2020-08-15T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'on Linked on Linked In 4',
        dateTime: '2020-08-10T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
  {
    id: 5,
    name: 'Color palette for every course:',
    studentsCount: 1,
    sessions: [
      {
        name: 'Color palette for every course: 1',
        dateTime: '2020-08-11T12:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 2',
        dateTime: '2020-08-18T22:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 3',
        dateTime: '2020-08-06T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      },
      {
        name: 'Color palette for every course: 4',
        dateTime: '2020-08-14T15:00:00Z',
        id: '3f74ec1d-2cb3-4b7a-8a35-abc60096a7c7',
      }
    ]
  },
]