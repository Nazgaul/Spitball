import studyRoomService from '../../services/studyRoomService.js';
import {studyRoom_SETTERS} from '../constants/studyRoomConstants.js';
import {twilio_SETTERS} from '../constants/twilioConstants.js';
import Vue from 'vue';

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
   roomTopologyType: 'PeerToPeer',
   // TODO: change it to roomId after u clean all
   studyRoomId: null,
   roomParticipantCount: 0,

   dialogEndSession: false,
   dialogUserConsent: false,
   dialogSnapshot: false,
   roomProps: null,
   roomParticipants:{},
   audioVideoDialog:false,
   studyRoomDrawerState:true,
   studyRoomFooterState:true,
   isBrowserNotSupport:false,
   roomNetworkQuality: null,
   roomEnrolled:null,
}

const mutations = {
   [studyRoom_SETTERS.ACTIVE_NAV_EDITOR]: (state, navEditor) => state.activeNavEditor = navEditor,
   [studyRoom_SETTERS.ROOM_PROPS](state, props) {
      state.roomOnlineDocument = props.onlineDocument;
      state.roomIsTutor = this.getters.accountUser?.id == props.tutorId;
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
      state.roomTopologyType = props.topologyType;
      state.roomEnrolled = props.enrolled;
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
      state.roomConversationId = null;
      state.roomTutor = {};
      state.studyRoomId = null;
      state.dialogEndSession = false;
      state.roomProps = null;
      state.roomType = null;
      state.roomName = null;
      state.roomDate = null;
      state.isBrowserNotSupport = false;
      state.roomParticipants = {};
      state.roomTopologyType = 'PeerToPeer';
      state.roomNetworkQuality = null;
      state.roomEnrolled = null;
   },
   [studyRoom_SETTERS.DIALOG_USER_CONSENT]: (state, val) => state.dialogUserConsent = val,
   [studyRoom_SETTERS.DIALOG_SNAPSHOT]: (state, val) => state.dialogSnapshot = val,
   [studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT]: (state, val) => state.roomParticipantCount = val,
   [studyRoom_SETTERS.ROOM_JOINED]: (state, val) => state.roomIsJoined = val,
   [studyRoom_SETTERS.RESET_ROOM_PARTICIPANTS]: (state) => state.roomParticipants = {},
   [studyRoom_SETTERS.ADD_ROOM_PARTICIPANT]: (state, participant) => {
      let participantId = _getIdFromIdentity(participant.identity)
      let participantObj = {
         name: _getNameFromIdentity(participant.identity),
         id: participantId
      }
      Vue.set(state.roomParticipants, participantId, participantObj);
   },
   [studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT]: (state, participant) => {
      let participantId = _getIdFromIdentity(participant.identity)
      Vue.delete(state.roomParticipants, participantId)
   },
   [studyRoom_SETTERS.ADD_ROOM_PARTICIPANT_TRACK]: (state, track) => {
      if(track.attach){
         let participantId = _getIdFromIdentity(track.identity);
         let isParticipantTutor = (participantId == state.roomTutor.tutorId)
         if(track.name == 'screenTrack' && isParticipantTutor){
            Vue.set(state.roomParticipants[participantId], 'screen', track);
         }else{
            Vue.set(state.roomParticipants[participantId], track.kind, track);
         }
      }
   },
   [studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT_TRACK]: (state, track) => {
      let participantId = _getIdFromIdentity(track.identity);
      let isParticipantTutor = (participantId == state.roomTutor.tutorId);
      if(track.name == 'screenTrack' && isParticipantTutor){
         Vue.delete(state.roomParticipants[participantId], 'screen')
      }else{
         Vue.delete(state.roomParticipants[participantId], track.kind)
      }
   },
   toggleAudioVideoDialog(state,val){
      state.audioVideoDialog = val;
   },
   setStudyRoomDrawer(state,val){
      state.studyRoomDrawerState = val;
   },
   setStudyRoomFooterState(state,val){
      state.studyRoomFooterState = val;
   },
   [studyRoom_SETTERS.BROWSER_NOT_SUPPORT]: (state, val) => state.isBrowserNotSupport = val,
   [studyRoom_SETTERS.ROOM_NETWORK_QUALITY]: (state, val) => state.roomNetworkQuality = val,
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
   getAudioVideoDialog:state => state.audioVideoDialog, 
   getStudyRoomDrawerState:state => state.studyRoomDrawerState, 
   getStudyRoomFooterState:state => state.studyRoomFooterState, 
   getRoomAudioTracks(state){
      return Object.values(state.roomParticipants).map(p=>p.audio)
   },
   getIsBrowserNotSupport:state => state.isBrowserNotSupport, 
   getRoomTopologyType:state => state.roomTopologyType, 
   getRoomNetworkQuality:state => state.roomNetworkQuality,
   getRoomParticipantsAudio:state =>{
      return Object.entries(state.roomParticipants).map(e=>({
            id: e[1].id,
            name: e[1].name,
            audio: e[1].audio
         })
      ).filter(e=>e.audio)
   },
   getStudyroomEnrolled:state => state.roomEnrolled,
   // getSessionRecurring: () => (nextEvents) => {
   //    if(!nextEvents) return null;
   //    let times = nextEvents.length;

   //    let daysObj = nextEvents.map(day=>{
   //       return {
   //          text: Moment(day).format('ddd'),
   //          digit:Moment(day).format('d')
   //       }
   //    }).sort((a,b)=>a.digit - b.digit)

   //    let days = Array.from(new Set(daysObj.map(d=>d.text))).join(', ');
   //    let start = nextEvents[0];
   //    let startNext = nextEvents.filter(dateEvent=> Moment(dateEvent).isAfter())[0];
   //    return {
   //       times,
   //       days,
   //       start,
   //       startNext
   //    }
   // },
}
const actions = {
   updateToggleTutorFullScreen({dispatch,commit},val){
      commit(twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN,val)
      if(val){
         dispatch('updateActiveNavEditor',ROOM_MODE.CLASS_SCREEN)
      }else{
         dispatch('updateActiveNavEditor',ROOM_MODE.CLASS_MODE)
      }
   },
   updateFullScreen(context,{participantId,trackType}){
      if(participantId){
         if(trackType == 'videoTrack'){
            context.dispatch('updateActiveNavEditor',ROOM_MODE.CLASS_SCREEN)
         }else{
            context.dispatch('updateActiveNavEditor',ROOM_MODE.SCREEN_MODE)
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
   updateActiveNavEditor({ commit,getters,dispatch }, val) {
      commit(studyRoom_SETTERS.ACTIVE_NAV_EDITOR, val)
      if(getters.getRoomIsTutor){
         let transferDataObj = {
             type: "updateActiveNav",
             data: val
         };
         let normalizedData = JSON.stringify(transferDataObj);
         dispatch('sendDataTrack',normalizedData)
      }
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
      if (getters.getRoomIdSession == roomId) {
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
   updateEndSession({ commit, state ,getters,dispatch}){
      if(getters.getIsShareScreen){
         dispatch('updateShareScreen',false)
      }
      commit(studyRoom_SETTERS.ROOM_ACTIVE, false);
      if(getters.getIsRecording){
         studyRoomRecordingService.stopRecord();
      }
      if(getters.getRoomIsTutor){
         studyRoomService.endTutoringSession(state.studyRoomId).then(() => {
            commit(studyRoom_SETTERS.DIALOG_END_SESSION, false)
            commit('logoutTailorEd');
         })
      }
   },
   updateResetRoom({dispatch, commit }) {
      commit(studyRoom_SETTERS.ROOM_ACTIVE, false);
      commit(studyRoom_SETTERS.ROOM_RESET)
      dispatch('updateReviewDialog',false)
   },
   updateLiveImage(context, formData) {
      return studyRoomService.updateImage(formData)
   },
   updateCreateStudyRoomPrivate({dispatch}, params) {
      return studyRoomService.createPrivateRoom(params).then(({data}) => {
         dispatch('updateCreateStudyRoom', {data, params})
      })
   },
   updateCreateStudyRoom({commit, getters}, {data, params}) {
      let newStudyRoomParams = {
         date: params.date || new Date().toISOString(),
         id: data.studyRoomId,
         currency: params.currency || '',
         name: params.name,
         price: params.price,
         conversationId: data.identifier,
            lastSession: params.date || new Date().toISOString(),
            tutorId: getters.getAccountId,
            tutorName: getters.getAccountName
      }
      let myStudyRooms = getters.getStudyRoomItems;
      myStudyRooms.unshift(newStudyRoomParams);
      commit('setStudyRoomItems', myStudyRooms);
      let chatParams = {
         conversationId:data.identifier,
         studyRoomId:data.studyRoomId
      }
      commit('ADD_CONVERSATION_STUDYROOM',chatParams)

   },
   updateRoomDisconnected({commit,getters}){
      commit(twilio_SETTERS.VIDEO_AVAILABLE,false);
      commit(twilio_SETTERS.AUDIO_AVAILABLE,false);
      commit(studyRoom_SETTERS.RESET_ROOM_PARTICIPANTS);
      if(!getters.getRoomIsTutor){
         commit('logoutTailorEd');
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
   updateTailorEd(context,{roomId,code}){
      return studyRoomService.tailorEd({roomId,code});
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}