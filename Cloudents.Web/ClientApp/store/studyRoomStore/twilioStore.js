import {SETTERS} from '../constants/twilioConstants.js';

const state = {
   jwtToken: null,
   isVideoActive: true,
   isAudioActive: true,
   isVideoAvailable: false,
   isAudioAvailable: false,
   isFullScreenAvailable:false,
   isShareScreen:false,
}
const mutations = {
   [SETTERS.JWT_TOKEN]: (state,token) => state.jwtToken = token,
   [SETTERS.DATA_TRACK]: () => {},
   [SETTERS.CHANGE_VIDEO_DEVICE]: () => {},
   [SETTERS.VIDEO_AVAILABLE]: (state,val) => state.isVideoAvailable = val,
   [SETTERS.VIDEO_TOGGLE]: (state,val) => state.isVideoActive = val,
   [SETTERS.CHANGE_AUDIO_DEVICE]: () => {},
   [SETTERS.AUDIO_AVAILABLE]: (state,val) => state.isAudioAvailable = val,
   [SETTERS.AUDIO_TOGGLE]: (state,val) => state.isAudioActive = val,
   [SETTERS.FULL_SCREEN_AVAILABLE]: (state,val) => state.isFullScreenAvailable = val,
   [SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE]: (state,val) => state.isShareScreen = val,
}

const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
   getIsFullScreenAvailable: (state) => state.isFullScreenAvailable,
   getIsShareScreen: (state) => state.isShareScreen,
}
const actions = {
   updateJwtToken({commit},token){
      commit(SETTERS.JWT_TOKEN,token)
   },
   sendDataTrack({commit},data){
      commit(SETTERS.DATA_TRACK,data)
   },
   updateVideoTrack({commit},trackId){
      global.localStorage.setItem('sb-videoTrackId',trackId);
      commit(SETTERS.CHANGE_VIDEO_DEVICE,trackId);
   },
   updateAudioTrack({commit},trackId){
      global.localStorage.setItem('sb-audioTrackId',trackId);
      commit(SETTERS.CHANGE_AUDIO_DEVICE,trackId);
   },
   updateShareScreen({commit},val){
      commit(SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,val)
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
}
export default {
   state,
   mutations,
   getters,
   actions
}