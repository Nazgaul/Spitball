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
   maor_goStudyRoom(){
      let roomId = router.currentRoute.params.id;
      router.push({name:'tutoring',params:{id:roomId}})
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}