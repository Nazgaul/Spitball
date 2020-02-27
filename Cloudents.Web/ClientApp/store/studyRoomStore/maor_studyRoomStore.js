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
   maor_studyRoomMiddleWare({getters,dispatch}){
      let isStudentNeedPayment = (!getters.getStudyRoomData.isTutor && getters.getStudyRoomData.needPayment);
      if(isStudentNeedPayment){
         console.log(router)
         debugger
         // change it : go to root route
         router.push({name: 'feed', query:{dialog:'payment'}})
         // router.push({query:{dialog:'payment'}})
         // https://localhost:53217/studyroom/2d052fbf-e2ad-4e61-ac0f-ab6c00d58919

         // return dispatch('requestPaymentURL'); 
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