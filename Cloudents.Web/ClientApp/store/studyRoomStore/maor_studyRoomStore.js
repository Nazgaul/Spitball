import maor_studyRoomService from '../../services/maor_studyRoomService.js';

function _checkPayment(context){
   let data = context.getters.getStudyRoomData;
   let isStudentNeedPayment = (!data.isTutor && data.needPayment);
   if(isStudentNeedPayment){
      return Promise.reject()
   }
   return Promise.resolve();
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
      let arr = [_checkPayment];
      arr.forEach(async (d) => {
         await d(context).catch(() => {
            return Promise.reject();
         });
      })
      return Promise.resolve();
   },
   maor_endTutoringSession(context,roomId){
      return maor_studyRoomService.endTutoringSession(roomId);
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}