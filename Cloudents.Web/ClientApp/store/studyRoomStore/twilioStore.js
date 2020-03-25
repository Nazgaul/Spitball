import {twilio_SETTERS} from '../constants/twilioConstants.js';

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
   [twilio_SETTERS.JWT_TOKEN]: (state,token) => state.jwtToken = token,
   [twilio_SETTERS.DATA_TRACK]: () => {},
   [twilio_SETTERS.CHANGE_VIDEO_DEVICE]: () => {},
   [twilio_SETTERS.VIDEO_AVAILABLE]: (state,val) => state.isVideoAvailable = val,
   [twilio_SETTERS.VIDEO_TOGGLE]: (state,val) => state.isVideoActive = val,
   [twilio_SETTERS.CHANGE_AUDIO_DEVICE]: () => {},
   [twilio_SETTERS.AUDIO_AVAILABLE]: (state,val) => state.isAudioAvailable = val,
   [twilio_SETTERS.AUDIO_TOGGLE]: (state,val) => state.isAudioActive = val,
   [twilio_SETTERS.FULL_SCREEN_AVAILABLE]: (state,val) => state.isFullScreenAvailable = val,
   [twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE]: (state,val) => state.isShareScreen = val,
}

const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
   getIsFullScreenAvailable: (state) => state.isFullScreenAvailable,
   getIsShareScreen: (state) => state.isShareScreen,
}
const actions = {
   updateJwtToken({commit,getters},token){
      if(!getters.getRoomIsTutor){
         commit('setToaster', 'simpleToaster_sessionStarted');
      }
      commit(twilio_SETTERS.JWT_TOKEN,token)
   },
   sendDataTrack({commit},data){
      commit(twilio_SETTERS.DATA_TRACK,data)
   },
   updateVideoTrack({commit},trackId){
      global.localStorage.setItem('sb-videoTrackId',trackId);
      commit(twilio_SETTERS.CHANGE_VIDEO_DEVICE,trackId);
   },
   updateAudioTrack({commit},trackId){
      global.localStorage.setItem('sb-audioTrackId',trackId);
      commit(twilio_SETTERS.CHANGE_AUDIO_DEVICE,trackId);
   },
   updateShareScreen({commit},val){
      commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,val)
   },
   updateVideoToggle({commit,state}){
      if(state.isVideoAvailable){
         commit(twilio_SETTERS.VIDEO_TOGGLE,!state.isVideoActive);
      }
   },
   updateAudioToggle({commit,state}){
      if(state.isAudioAvailable){
         commit(twilio_SETTERS.AUDIO_TOGGLE,!state.isAudioActive);
      }
   },
}
export default {
   state,
   mutations,
   getters,
   actions
}