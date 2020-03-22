import studyRoomService from '../../services/studyRoomService.js';
import {studyRoom_SETTERS} from '../constants/studyRoomConstants.js';
import {twilio_SETTERS} from '../constants/twilioConstants.js';
import studyRoomRecordingService from '../../components/studyroom/studyRoomRecordingService.js'

function _checkPayment(context){
   let data = context.getters.getStudyRoomData;
   let isStudentNeedPayment = (!data.isTutor && data.needPayment);
   if(isStudentNeedPayment){
      return Promise.reject()
   }
   return Promise.resolve();
}


const state = {
   activeNavIndicator: 'white-board',
   roomOnlineDocument: null,
   roomIsTutor:false,
   roomIsActive:false,
   // TODO: change it to roomId after u clean all
   studyRoomId: null,

   dialogRoomSettings: false,
   dialogENDSession:false,
   
   roomProps: null,
}

const mutations = {
   [studyRoom_SETTERS.ACTIVE_NAV_TAB_INDICATOR]: (state,{activeNav}) => state.activeNavIndicator = activeNav,
   [studyRoom_SETTERS.ROOM_PROPS](state,props){
      state.roomOnlineDocument = props.onlineDocument;
      state.roomIsTutor = this.getters.accountUser.id == props.tutorId;
      // TODO: change it to roomId after u clean all
      state.studyRoomId = props.roomId;

      // state.roomProps = props;
      /*
      conversationId: "162074_162085"
needPayment: false
studentId: 162074
studentImage: "https://spitball-dev-function.azureedge.net:443/api/image/user/162074/1575207545.png"
studentName: "maor Student IL"
tutorId: 162085
tutorImage: "https://spitball-dev-function.azureedge.net:443/api/image/user/162085/1584346344.jpg"
tutorName: "Maor Tutor IL"
tutorPrice: 50
      */ 
   },
   [studyRoom_SETTERS.DIALOG_ROOM_SETTINGS]: (state,val) => state.dialogRoomSettings = val,
   [studyRoom_SETTERS.DIALOG_END_SESSION]: (state,val) => state.dialogENDSession = val,
   [studyRoom_SETTERS.ROOM_ACTIVE]: (state,val) => state.roomIsActive = val,
}
const getters = {
   getActiveNavIndicator: state => state.activeNavIndicator,
   getRoomOnlineDocument: state => state.roomOnlineDocument,
   getRoomIsTutor: state => state.roomIsTutor,
   getRoomIsActive: state => state.roomIsActive,
   getDialogRoomSettings: state => state.dialogRoomSettings,
   getDialogRoomEnd: state => state.roomIsActive && state.roomIsTutor && state.dialogENDSession,
}
const actions = {
   updateEndDialog({commit}, val){
      commit(studyRoom_SETTERS.DIALOG_END_SESSION, val);
   },
   updateActiveNavTab({commit},val){
      commit(studyRoom_SETTERS.ACTIVE_NAV_TAB,val)
   },
   updateDialogRoomSettings({commit},val){
      commit(studyRoom_SETTERS.DIALOG_ROOM_SETTINGS,val)
   },
   updateEnterRoom({commit},roomId){
      studyRoomService.enterRoom(roomId).then((jwtToken)=>{
         commit(twilio_SETTERS.JWT_TOKEN,jwtToken)
      })
   },












   updateStudyRoomInformation({getters,dispatch,commit},roomId){
      if(getters.getStudyRoomData){
         return dispatch('studyRoomMiddleWare')
      }else{
         return studyRoomService.getRoomInformation(roomId).then((roomProps)=>{
            commit(studyRoom_SETTERS.ROOM_PROPS,roomProps)
            if(roomProps.jwt){
               commit(twilio_SETTERS.JWT_TOKEN,roomProps.jwt)
            }
            return dispatch('studyRoomMiddleWare')
         })
      }
   },
   studyRoomMiddleWare(context){
      let arr = [_checkPayment];
      arr.forEach(async (d) => {
         await d(context).catch(() => {
            return Promise.reject();
         });
      })
      return Promise.resolve();
   },
   updateEndTutorSession({commit,state}){
      commit(studyRoom_SETTERS.ROOM_ACTIVE,false);
      studyRoomRecordingService.stopRecord();
      studyRoomService.endTutoringSession(state.studyRoomId).then(()=>{
         commit(studyRoom_SETTERS.DIALOG_END_SESSION,false)
      })
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}