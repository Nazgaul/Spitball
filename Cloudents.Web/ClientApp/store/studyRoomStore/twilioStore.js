const state = {
   jwtToken: null,
}
const mutations = {
   setJwtToken(state,token){
      state.jwtToken = token;
   },
   setDataTrack(state,data){
   },
   setVideoToggle(state,val){
   },
   setAudioToggle(state,val){
   },
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
   updateVideoToggle({commit},val){
      commit('setVideoToggle',val)
   },
   updateAudioToggle({commit},val){
      commit('setAudioToggle',val)
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}