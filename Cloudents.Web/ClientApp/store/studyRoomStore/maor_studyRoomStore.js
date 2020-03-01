import maor_studyRoomService from '../../services/maor_studyRoomService.js';
import {router} from '../../main.js';

function _checkPayment(context){
   let isStudentNeedPayment = (!context.getters.getStudyRoomData.isTutor && context.getters.getStudyRoomData.needPayment);
   if(isStudentNeedPayment){
      let roomId = router.currentRoute.params.id || context.getters.getStudyRoomData.roomId;
      return Promise.reject({name:'tutoring',params:{id:roomId},query:{dialog:'payment'}})
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