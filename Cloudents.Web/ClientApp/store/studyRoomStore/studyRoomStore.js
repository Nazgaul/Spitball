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
}

const mutations = {
   [SETTERS.ACTIVE_NAV_TAB_INDICATOR]: (state,{activeNav}) => state.activeNavIndicator = activeNav,
}
const getters = {
   getActiveNavIndicator: state => state.activeNavIndicator,
}
const actions = {
   updateActiveNavTab({commit},val){
      commit(SETTERS.ACTIVE_NAV_TAB,val)
   },













   updateStudyRoomInformation({getters,dispatch},roomId){
      if(getters.getStudyRoomData){
         return dispatch('studyRoomMiddleWare')
      }else{
         return studyRoomService.getRoomInformation(roomId).then((roomProps)=>{
            dispatch('updateStudyRoomProps',roomProps);
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