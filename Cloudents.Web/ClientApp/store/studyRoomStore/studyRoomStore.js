import studyRoomService from '../../services/studyRoomService.js';
import {SETTERS} from '../constants/studyRoomConstants.js';

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
   dialogRoomSettings: false,
   
   roomProps: null,
}

const mutations = {
   [SETTERS.ACTIVE_NAV_TAB_INDICATOR]: (state,{activeNav}) => state.activeNavIndicator = activeNav,
   [SETTERS.ROOM_PROPS](state,props){
      state.roomOnlineDocument = props.onlineDocument;

      // props.isTutor = this.getters.accountUser.id == props.tutorId;
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
   [SETTERS.DIALOG_ROOM_SETTINGS]: (state,val) => state.dialogRoomSettings = val,
}
const getters = {
   getActiveNavIndicator: state => state.activeNavIndicator,
   getRoomOnlineDocument: state => state.roomOnlineDocument,
   getDialogRoomSettings: state => state.dialogRoomSettings,
}
const actions = {
   updateActiveNavTab({commit},val){
      commit(SETTERS.ACTIVE_NAV_TAB,val)
   },
   updateDialogRoomSettings({commit},val){
      commit(SETTERS.DIALOG_ROOM_SETTINGS,val)
   },












   updateStudyRoomInformation({getters,dispatch,commit},roomId){
      if(getters.getStudyRoomData){
         return dispatch('studyRoomMiddleWare')
      }else{
         return studyRoomService.getRoomInformation(roomId).then((roomProps)=>{
            commit(SETTERS.ROOM_PROPS,roomProps)
            // dispatch('updateStudyRoomProps',roomProps);
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
   endTutoringSession(context,roomId){
      return studyRoomService.endTutoringSession(roomId);
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}