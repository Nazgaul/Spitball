import studyRoomService from '../../services/studyRoomService.js';
import {studyRoom_SETTERS} from '../constants/studyRoomConstants.js';
import {twilio_SETTERS} from '../constants/twilioConstants.js';

import studyRoomRecordingService from '../../components/studyroom/studyRoomRecordingService.js'
function _getRoomParticipantsWithoutTutor(roomParticipants,roomTutor){
   if(roomTutor?.tutorId){
      Object.filter = (obj, predicate) => 
      Object.keys(obj)
            .filter( key => predicate(obj[key]) )
            .reduce( (res, key) => (res[key] = obj[key], res), {} );
      return Object.filter(roomParticipants, participant => participant.id != roomTutor.tutorId); 
   }
}
function _getIdFromIdentity(identity){
   return identity.split('_')[0]
}
function _getNameFromIdentity(identity){
   return identity.split('_')[1]
}
function _newObjectPointer(obj){
   // object assign cuz we need to create new object to vuex listen to
   return Object.assign({}, obj);
}
function _checkPayment(context) {
   let isTutor = context.getters.getRoomIsTutor;
   let isNeedPayment = context.getters.getRoomIsNeedPayment;
   let isStudentNeedPayment = (!isTutor && isNeedPayment);
   if (isStudentNeedPayment) {
      return Promise.reject()
   }
   return Promise.resolve();
}

const ROOM_MODE = {
   WHITE_BOARD: 'white-board',
   TEXT_EDITOR: 'shared-document',
   CODE_EDITOR: 'code-editor',
   SCREEN_MODE: 'screen-mode',
   CLASS_MODE: 'class-mode',
   CLASS_SCREEN: 'class-screen'
}

const state = {
   activeNavEditor: ROOM_MODE.WHITE_BOARD,
   roomDate:null,
   roomType:null,
   roomName:null,
   roomOnlineDocument: null,
   roomIsTutor: false,
   roomIsActive: false,
   roomIsNeedPayment: false,
   roomTutor: {},
   roomIsJoined: false,
   // TODO: change it to roomId after u clean all
   studyRoomId: null,
   roomParticipantCount: 0,

   dialogEndSession: false,
   dialogUserConsent: false,
   dialogSnapshot: false,

   roomProps: null,
   roomParticipants:{},
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
      state.roomType = props.type;
      state.roomName = props.name;
      state.roomDate = props.broadcastTime;
   },
   [studyRoom_SETTERS.DIALOG_END_SESSION]: (state, val) => state.dialogEndSession = val,
   [studyRoom_SETTERS.ROOM_ACTIVE]: (state, isConnected) => {
      state.roomIsActive = isConnected
      if(!isConnected){
         state.roomParticipantCount = 0;
      }
   },
   [studyRoom_SETTERS.ROOM_PAYMENT]: (state, val) => state.roomIsNeedPayment = val,
   [studyRoom_SETTERS.ROOM_RESET]: (state) => {
      state.activeNavEditor = 'white-board';
      state.roomOnlineDocument = null;
      state.roomIsTutor = false;
      state.roomIsNeedPayment = false;
      state.roomTutor = {};
      state.studyRoomId = null;
      state.dialogEndSession = false;
      state.roomProps = null;
      state.roomType = null;
      state.roomName = null;
      state.roomParticipants = {}
   },
   [studyRoom_SETTERS.DIALOG_USER_CONSENT]: (state, val) => state.dialogUserConsent = val,
   [studyRoom_SETTERS.DIALOG_SNAPSHOT]: (state, val) => state.dialogSnapshot = val,
   [studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT]: (state, val) => state.roomParticipantCount = val,
   [studyRoom_SETTERS.ROOM_JOINED]: (state, val) => state.roomIsJoined = val,
   [studyRoom_SETTERS.ADD_ROOM_PARTICIPANT]: (state, participant) => {
      let participantId = _getIdFromIdentity(participant.identity)
      let participantObj = {
         name: _getNameFromIdentity(participant.identity),
         id: participantId
      }
      state.roomParticipants[participantId] = participantObj;
      state.roomParticipants = _newObjectPointer(state.roomParticipants)
   },
   [studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT]: (state, participant) => {
      let participantId = _getIdFromIdentity(participant.identity)
      delete state.roomParticipants[participantId];
      state.roomParticipants = _newObjectPointer(state.roomParticipants)
   },
   [studyRoom_SETTERS.ADD_ROOM_PARTICIPANT_TRACK]: (state, track) => {
      if(track.attach){
         let participantId = _getIdFromIdentity(track.identity);
         state.roomParticipants[participantId][track.kind] = track;
         state.roomParticipants = _newObjectPointer(state.roomParticipants)
      }
   },
   [studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT_TRACK]: (state, track) => {
      let participantId = _getIdFromIdentity(track.identity);
      state.roomParticipants[participantId][track.kind] = undefined;
      state.roomParticipants = _newObjectPointer(state.roomParticipants)
   }
}
const getters = {
   getActiveNavEditor: state => state.activeNavEditor,
   getRoomOnlineDocument: state => state.roomOnlineDocument,
   getRoomIsTutor: state => state.roomIsTutor,
   getRoomName: state => state.roomName,
   getRoomDate: state => state.roomDate,
   getRoomIsBroadcast: state => state.roomType === 'Broadcast',
   getRoomIsActive: state => state.roomIsActive,
   getRoomTutor: state => state.roomTutor,
   getRoomIdSession: state => state.studyRoomId,
   getRoomParticipantCount: state => state.roomIsActive? state.roomParticipantCount : 0,
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
   getDialogRoomEnd: state => state.roomIsActive && state.roomIsTutor && state.dialogEndSession,
   getDialogUserConsent: state => state.dialogUserConsent,
   getDialogSnapshot: state => state.dialogSnapshot,
   getRoomIsJoined:state => state.roomIsJoined,
   getRoomModeConsts: ()=> ROOM_MODE,
   getRoomParticipants:state => _getRoomParticipantsWithoutTutor(state.roomParticipants,state.roomTutor),
   getRoomTutorParticipant:state => {
      if(state.roomTutor?.tutorId){
         return state.roomParticipants[state.roomTutor.tutorId]
      }
   },
}
const actions = {
   updateFullScreen(context,elId){
      let className = 'fullscreenMode';
      if(elId){
         let interval = setInterval(() => {
            let vidEl = document.querySelector(`#${elId}`);
            if(vidEl){
               vidEl.classList.add(className);
               clearInterval(interval)
            }
         }, 50);
      }else{
         let x = document.querySelector(`.${className}`);
         if (x) {
            x.classList.remove(className);
         }
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
   updateEnterRoom({ dispatch }, roomId) { // when tutor press start session
      return studyRoomService.enterRoom(roomId).then((jwtToken) => {
         dispatch('updateJwtToken',jwtToken);
         return Promise.resolve()
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
            commit(studyRoom_SETTERS.ROOM_PROPS, roomProps);
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
   },
   updateRoomDisconnected({commit,getters,dispatch}){
      commit(twilio_SETTERS.VIDEO_AVAILABLE,false);
      commit(twilio_SETTERS.AUDIO_AVAILABLE,false)
      if(!getters.getRoomIsTutor){
         dispatch('updateReviewDialog',true)
      }
   },
   updateRoomIsJoined({ commit,getters ,dispatch}, val) {
      commit(studyRoom_SETTERS.ROOM_JOINED,val)
      if(val){
         dispatch('updateJwtToken',getters.getJwtToken);
      }else{
         dispatch('updateJwtToken',null);
      }
   },
}
export default {
   state,
   mutations,
   getters,
   actions
}