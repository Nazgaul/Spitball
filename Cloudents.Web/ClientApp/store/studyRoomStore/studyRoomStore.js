import studyRoomService from '../../services/studyRoomService.js';
import {studyRoom_SETTERS} from '../constants/studyRoomConstants.js';
import {twilio_SETTERS} from '../constants/twilioConstants.js';
import studyRoomRecordingService from '../../components/studyroom/studyRoomRecordingService.js'

function _checkPayment(context) {
   let isTutor = context.getters.getRoomIsTutor;
   let isNeedPayment = context.getters.getRoomIsNeedPayment;
   let isStudentNeedPayment = (!isTutor && isNeedPayment);
   if (isStudentNeedPayment) {
      return Promise.reject()
   }
   return Promise.resolve();
}


const state = {
   activeNavIndicator: 'white-board',
   roomOnlineDocument: null,
   roomIsTutor: false,
   roomIsActive: false,
   roomIsNeedPayment: false,
   roomTutor: {},
   roomStudent: {},
   // TODO: change it to roomId after u clean all
   studyRoomId: null,

   dialogRoomSettings: false,
   dialogEndSession: false,

   roomProps: null,
}

const mutations = {
   [studyRoom_SETTERS.ACTIVE_NAV_TAB_INDICATOR]: (state, { activeNav }) => state.activeNavIndicator = activeNav,
   [studyRoom_SETTERS.ROOM_PROPS](state, props) {
      state.roomOnlineDocument = props.onlineDocument;
      state.roomIsTutor = this.getters.accountUser.id == props.tutorId;
      state.roomTutor = {
         tutorId: props.tutorId,
         tutorName: props.tutorName,
         tutorImage: props.tutorImage,
         tutorPrice: props.tutorPrice,
      }
      state.roomStudent = {
         studentId: props.studentId,
         studentImage: props.studentImage,
         studentName: props.studentName,
      }
      state.roomIsNeedPayment = props.needPayment;
      state.roomConversationId = props.conversationId;
      state.studyRoomId = props.roomId;
   },
   [studyRoom_SETTERS.DIALOG_ROOM_SETTINGS]: (state, val) => state.dialogRoomSettings = val,
   [studyRoom_SETTERS.DIALOG_END_SESSION]: (state, val) => state.dialogEndSession = val,
   [studyRoom_SETTERS.ROOM_ACTIVE]: (state, val) => state.roomIsActive = val,
   [studyRoom_SETTERS.ROOM_PAYMENT]: (state, val) => state.roomIsNeedPayment = val,
   [studyRoom_SETTERS.ROOM_RESET]: (state) => {
      state.activeNavIndicator = 'white-board';
      state.roomOnlineDocument = null;
      state.roomIsTutor = false;
      state.roomIsActive = false;
      state.roomIsNeedPayment = false;
      state.roomTutor = {};
      state.roomStudent = {};
      state.studyRoomId = null;
      state.dialogRoomSettings = false;
      state.dialogEndSession = false;
      state.roomProps = null;
   }
}
const getters = {
   getActiveNavIndicator: state => state.activeNavIndicator,
   getRoomOnlineDocument: state => state.roomOnlineDocument,
   getRoomIsTutor: state => state.roomIsTutor,
   getRoomIsActive: state => state.roomIsActive,
   getRoomTutor: state => state.roomTutor,
   getRoomStudent: state => state.roomStudent,
   getRoomIdSession: state => state.studyRoomId,
   getRoomConversationId: state => state.roomConversationId,
   getRoomIsNeedPayment: state => {
      if (!state.studyRoomId) {
         return null;
      }
      if (state.roomIsTutor) {
         return false;
      }
      return state.roomIsNeedPayment;
   },
   getDialogRoomSettings: state => state.dialogRoomSettings,
   getDialogRoomEnd: state => state.roomIsActive && state.roomIsTutor && state.dialogEndSession,
   getDialogTutorStart: state => !state.roomIsActive && state.roomIsTutor,
}
const actions = {
   updateEndDialog({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_END_SESSION, val);
   },
   updateActiveNavTab({ commit }, val) {
      commit(studyRoom_SETTERS.ACTIVE_NAV_TAB, val)
   },
   updateDialogRoomSettings({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_ROOM_SETTINGS, val)
   },
   updateEnterRoom({ commit }, roomId) { // when tutor press start session
      studyRoomService.enterRoom(roomId).then((jwtToken) => {
         commit(twilio_SETTERS.JWT_TOKEN, jwtToken)
      })
   },
   updateRoomIsNeedPayment({ commit,getters }, isNeedPayment) {
      commit(studyRoom_SETTERS.ROOM_PAYMENT, isNeedPayment)
      if(getters.getJwtToken){ 
         commit(twilio_SETTERS.JWT_TOKEN, getters.getJwtToken)
      }
   },








   updateStudyRoomInformation({ getters, dispatch, commit }, roomId) {
      if (getters.getRoomIdSession) {
         return dispatch('studyRoomMiddleWare')
      } else {
         return studyRoomService.getRoomInformation(roomId).then((roomProps) => {
            commit(studyRoom_SETTERS.ROOM_PROPS, roomProps)
            if (roomProps.jwt){
               commit(twilio_SETTERS.JWT_TOKEN, roomProps.jwt)
            }
            return dispatch('studyRoomMiddleWare')
         })
      }
   },
   studyRoomMiddleWare(context) {
      let arr = [_checkPayment];
      arr.forEach(async (d) => {
         await d(context).catch(() => {
            return Promise.reject();
         });
      })
      return Promise.resolve();
   },
   updateEndTutorSession({ commit, state }) {
      commit(studyRoom_SETTERS.ROOM_ACTIVE, false);
      studyRoomRecordingService.stopRecord();
      studyRoomService.endTutoringSession(state.studyRoomId).then(() => {
         commit(studyRoom_SETTERS.DIALOG_END_SESSION, false)
      })
   },
   updateResetRoom({ commit }) {
      commit(studyRoom_SETTERS.ROOM_RESET)
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}