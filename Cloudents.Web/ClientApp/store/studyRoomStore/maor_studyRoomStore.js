import maor_studyRoomService from '../../services/maor_studyRoomService.js';
import {router} from '../../main.js';

function _checkPayment(context){
   let isStudentNeedPayment = (!context.getters.getStudyRoomData.isTutor && context.getters.getStudyRoomData.needPayment);
   if(isStudentNeedPayment){
      let nextStepRoute = {query:{dialog:'payment'}}
      return Promise.reject(nextStepRoute);
   }
}

const state = {
   
}
const mutations = {
   
}
const getters = {
   
}
const actions = {
   maor_updateStudyRoomInformation({getters,dispatch},roomId){
      if(getters.getStudyRoomData){
         return dispatch('maor_studyRoomMiddleWare')
      }else{
         return maor_studyRoomService.getRoomInformation(roomId).then((roomProps)=>{
            dispatch('updateStudyRoomProps',roomProps);
            return dispatch('maor_studyRoomMiddleWare')
         })
      }
   },
   maor_goStudyRoom({getters}){
      let roomId = router.currentRoute.params.id || getters.getStudyRoomData.roomId;
      router.push({name:'tutoring',params:{id:roomId}})
   },
   maor_studyRoomMiddleWare(context){
      return _checkPayment(context) || Promise.resolve();
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}