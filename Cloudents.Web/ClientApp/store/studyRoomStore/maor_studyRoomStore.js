import maor_studyRoomService from '../../services/maor_studyRoomService.js';
import {router} from '../../main.js'
const state = {
   
}
const mutations = {
   
}
const getters = {
   
}
const actions = {
   maor_updateStudyRoomInformation({getters,dispatch},roomId){
      if(getters.getStudyRoomData){
         dispatch('maor_studyRoomMiddleWare')
      }else{
         return maor_studyRoomService.getRoomInformation(roomId).then((roomProps)=>{
            roomProps.isTutor = getters.accountUser.id == roomProps.tutorId;
            dispatch('updateStudyRoomProps',roomProps);
            return dispatch('maor_studyRoomMiddleWare')
         })
      }
   },
   maor_goStudyRoom({getters}){
      let roomId = router.currentRoute.params.id || getters.getStudyRoomData.roomId;
      router.push({name:'tutoring',params:{id:roomId}})
   },
   maor_studyRoomMiddleWare({getters,dispatch}){
      let isStudentNeedPayment = (!getters.getStudyRoomData.isTutor && getters.getStudyRoomData.needPayment);
      if(isStudentNeedPayment){
         let params = {title: 'payme_title', name: getters.getStudyRoomData.tutorName}
         dispatch('requestPaymentURL', params); 
         return Promise.reject();
      }else{
         return Promise.resolve()
      }
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}