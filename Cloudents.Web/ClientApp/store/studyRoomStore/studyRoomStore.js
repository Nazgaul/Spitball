import studyRoomService from '../../services/studyRoomService.js';
import {studyRoom_SETTERS} from '../constants/studyRoomConstants.js';
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
   activeNavEditor: 'white-board',
   roomOnlineDocument: null,
   roomIsTutor: false,
   roomIsActive: false,
   roomIsNeedPayment: false,
   roomTutor: {},
   // TODO: change it to roomId after u clean all
   studyRoomId: null,

   dialogRoomSettings: false,
   dialogEndSession: false,
   dialogUserConsent: false,
   dialogSnapshot: false,

   roomProps: null,
}

const mutations = {
   [studyRoom_SETTERS.ACTIVE_NAV_EDITOR]: (state, navEditor) => state.activeNavEditor = navEditor,
   [studyRoom_SETTERS.ROOM_PROPS](state, props) {
      state.roomOnlineDocument = props.onlineDocument;
      state.roomIsTutor = this.getters.accountUser.id == props.tutorId;
      state.roomTutor = {
         tutorId: props.tutorId,
         tutorName: props.tutorName,
         tutorImage: props.tutorImage,
         tutorPrice: props.tutorPrice,
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
      state.activeNavEditor = 'white-board';
      state.roomOnlineDocument = null;
      state.roomIsTutor = false;
      state.roomIsActive = false;
      state.roomIsNeedPayment = false;
      state.roomTutor = {};
      state.studyRoomId = null;
      state.dialogRoomSettings = false;
      state.dialogEndSession = false;
      state.roomProps = null;
   },
   [studyRoom_SETTERS.DIALOG_USER_CONSENT]: (state, val) => state.dialogUserConsent = val,
   [studyRoom_SETTERS.DIALOG_SNAPSHOT]: (state, val) => state.dialogSnapshot = val,
}
const getters = {
   getActiveNavEditor: state => state.activeNavEditor,
   getRoomOnlineDocument: state => state.roomOnlineDocument,
   getRoomIsTutor: state => state.roomIsTutor,
   getRoomIsActive: state => state.roomIsActive,
   getRoomTutor: state => state.roomTutor,
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
   getDialogUserConsent: state => state.dialogUserConsent,
   getDialogSnapshot: state => state.dialogSnapshot,
}
const actions = {
   updateFullScreen(context,elId){
      let className = 'fullscreenMode';
      if(elId){
         let interval = setInterval(() => {
            let vidEl = document.querySelector(`#${elId} video`);
            if(vidEl){
               vidEl.classList.add(className);
               clearInterval(interval)
            }
         }, 50);
      }else{
         document.querySelector(`.${className}`).classList.remove(className);
      }
   },
   updateDialogSnapshot({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_SNAPSHOT, val);
   },
   updateDialogUserConsent({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_USER_CONSENT, val);
   },
   updateEndDialog({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_END_SESSION, val);
   },
   updateActiveNavEditor({ commit }, val) {
      commit(studyRoom_SETTERS.ACTIVE_NAV_EDITOR, val)
   },
   updateDialogRoomSettings({ commit }, val) {
      commit(studyRoom_SETTERS.DIALOG_ROOM_SETTINGS, val)
   },
   updateEnterRoom({ dispatch }, roomId) { // when tutor press start session
      studyRoomService.enterRoom(roomId).then((jwtToken) => {
         dispatch('updateJwtToken',jwtToken);
      })
   },
   updateRoomIsNeedPayment({ commit,getters ,dispatch}, isNeedPayment) {
      commit(studyRoom_SETTERS.ROOM_PAYMENT, isNeedPayment)
      if(getters.getJwtToken){ 
         dispatch('updateJwtToken',getters.getJwtToken);
      }
   },
   updateStudyRoomInformation({ getters, dispatch, commit }, roomId) {
      if (getters.getRoomIdSession) {
         return dispatch('studyRoomMiddleWare')
      } else {
         return studyRoomService.getRoomInformation(roomId).then((roomProps) => {
            commit(studyRoom_SETTERS.ROOM_PROPS, roomProps)
            if (roomProps.jwt){
               dispatch('updateJwtToken',roomProps.jwt);
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
   updateEndTutorSession({ commit, state ,getters}) {
      commit(studyRoom_SETTERS.ROOM_ACTIVE, false);
      if(getters.getIsRecording){
         studyRoomRecordingService.stopRecord();
      }
      studyRoomService.endTutoringSession(state.studyRoomId).then(() => {
         commit(studyRoom_SETTERS.DIALOG_END_SESSION, false)
      })
   },
   updateResetRoom({ commit }) {
      commit(studyRoom_SETTERS.ROOM_ACTIVE, false);
      commit(studyRoom_SETTERS.ROOM_RESET)
   },
   updateCreateStudyRoom({getters,commit},params){
      return studyRoomService.createRoom(params).then(({data})=>{
         let newStudyRoomParams = {
            date: params.date || new Date().toISOString(),
            id: data.studyRoomId,
            name: params.name,
            conversationId: data.identifier,
         }
         let myStudyRooms = getters.getStudyRoomItems;
         myStudyRooms.unshift(newStudyRoomParams);
         commit('setStudyRoomItems',myStudyRooms)
         return
      })
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}