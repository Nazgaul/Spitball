import studyRoomService from '../../services/studyRoomService.js';

function _checkPayment(context){
   let data = context.getters.getStudyRoomData;
   let isStudentNeedPayment = (!data.isTutor && data.needPayment);
   if(isStudentNeedPayment){
      return Promise.reject()
   }
   return Promise.resolve();
}

const state = {
   jwtToken: null,
   
}
const mutations = {
   setJwtToken(state,token){
      state.jwtToken = token;
   },
   setDataTrack(state,data){

   }
}
const getters = {
   getJwtToken: (state) => state.jwtToken,
   
}
const actions = {
   updateJwtToken({commit},token){
      commit('setJwtToken',token)
   },
   sendDataTrack({commit},data){
      commit('setDataTrack',data)
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