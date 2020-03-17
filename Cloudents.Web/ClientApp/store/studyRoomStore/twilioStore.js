const state = {
   jwtToken: null,
   isVideoActive: true,
   isVideoAvailable: false,
   isAudioAvailable: false,
   isAudioActive: true,
}
const mutations = {
   setJwtToken(state,token){
      state.jwtToken = token;
   },
   setDataTrack(){},
   setIsVideoAvailable(state,val){
      state.isVideoAvailable = val;
   },
   setVideoToggle(state,val){
      state.isVideoActive = val;
   },
   setIsAudioAvailable(state,val){
      state.isAudioAvailable = val;
   },
   setAudioToggle(state,val){
      state.isAudioActive = val;
   },
}
const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
}
const actions = {
   updateJwtToken({commit},token){
      commit('setJwtToken',token)
   },
   sendDataTrack({commit},data){
      commit('setDataTrack',data)
   },
   updateVideoToggle({commit,state}){
      if(state.isVideoAvailable){
         commit('setVideoToggle',!state.isVideoActive);
      }
   },
   updateAudioToggle({commit,state}){
      if(state.isAudioAvailable){
         commit('setAudioToggle',!state.isAudioActive);
      }
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}