import maor_studyRoomService from '../../services/maor_studyRoomService.js';
import {router} from '../../main.js'
const state = {
   
}
const mutations = {
   
}
const getters = {
   
}
const actions = {
   maor_updateStudyRoomInformation(context,roomId){
      return maor_studyRoomService.getRoomInformation(roomId);
   },
   maor_goStudyRoom({getters}){
      let roomId = router.currentRoute.params.id || getters.getStudyRoomData.roomId;
      router.push({name:'tutoring',params:{id:roomId}})
   },
   maor_studyRoomMiddleWare({getters,dispatch},{to,from,next}){
      let isStudentNeedPayment = (!getters.getStudyRoomData.isTutor && getters.getStudyRoomData.needPayment);
      if(isStudentNeedPayment){
         let params = {title: 'payme_title', name: getters.getStudyRoomData.tutorName}
         dispatch('requestPaymentURL', params); 
      }else{
         next()
      }
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}