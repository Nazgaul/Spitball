const state = {
   jwtToken: null,
}
const mutations = {
   setJwtToken(state,token){
      state.jwtToken = token;
   },
   setDataTrack(state,data){
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
   
}
export default {
   state,
   mutations,
   getters,
   actions
}