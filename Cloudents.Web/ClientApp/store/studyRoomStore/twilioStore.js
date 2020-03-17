import {SETTERS} from '../constants/twilioConstants.js';

const state = {
   jwtToken: null,
   isVideoActive: true,
   isVideoAvailable: false,
   isAudioAvailable: false,
   isAudioActive: true,
}
const mutations = {
   [SETTERS.JWT_TOKEN]: (state,token) => state.jwtToken = token,
   [SETTERS.DATA_TRACK]: () => {},
   [SETTERS.VIDEO_AVAILABLE]: (state,val) => state.isVideoAvailable = val,
   [SETTERS.VIDEO_TOGGLE]: (state,val) => state.isVideoActive = val,
   [SETTERS.AUDIO_AVAILABLE]: (state,val) => state.isAudioAvailable = val,
   [SETTERS.AUDIO_TOGGLE]: (state,val) => state.isAudioActive = val,
   [SETTERS.SCREEN_SHARE]: () => {},
}
const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
}
const actions = {
   updateJwtToken({commit},token){
      commit(SETTERS.JWT_TOKEN,token)
   },
   sendDataTrack({commit},data){
      commit(SETTERS.DATA_TRACK,data)
   },
   updateVideoToggle({commit,state}){
      if(state.isVideoAvailable){
         commit(SETTERS.VIDEO_TOGGLE,!state.isVideoActive);
      }
   },
   updateAudioToggle({commit,state}){
      if(state.isAudioAvailable){
         commit(SETTERS.AUDIO_TOGGLE,!state.isAudioActive);
      }
   },
   updateShareScreen({commit}){
      commit(SETTERS.SCREEN_SHARE)
   }
}
export default {
   state,
   mutations,
   getters,
   actions
}