import {twilio_SETTERS} from '../constants/twilioConstants.js';

const state = {
   jwtToken: null,
   isVideoActive: true,
   isAudioActive: true,
   isVideoAvailable: false,
   isAudioAvailable: false,
   isShareScreen: false,
   isFullScreen:false,
   isAudioParticipants:true,
   settings_isVideo:false,
   videoDeviceId: global.localStorage.getItem('sb-videoTrackId'),
   audioDeviceId: global.localStorage.getItem('sb-audioTrackId'),
}
const mutations = {
   settings_setIsVideo(state,val){
      state.settings_isVideo = val
   },
   [twilio_SETTERS.JWT_TOKEN]: (state,token) => state.jwtToken = token,
   [twilio_SETTERS.DATA_TRACK]: () => {},
   [twilio_SETTERS.CHANGE_VIDEO_DEVICE]: () => {},
   [twilio_SETTERS.VIDEO_AVAILABLE]: (state,val) => state.isVideoAvailable = val,
   [twilio_SETTERS.VIDEO_TOGGLE]: (state,val) => state.isVideoActive = val,
   [twilio_SETTERS.CHANGE_AUDIO_DEVICE]: () => {},
   [twilio_SETTERS.AUDIO_AVAILABLE]: (state,val) => state.isAudioAvailable = val,
   [twilio_SETTERS.AUDIO_TOGGLE]: (state,val) => state.isAudioActive = val,
   [twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE]: (state,val) => state.isShareScreen = val,
   [twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN]:(state,val)=> state.isFullScreen = val,
   [twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS]:(state,val)=> state.isAudioParticipants = val,
   [twilio_SETTERS.VIDEO_DEVICE_ID]:(state,id)=> state.videoDeviceId = id,
   [twilio_SETTERS.AUDIO_DEVICE_ID]:(state,id)=> state.audioDeviceId = id,
}

const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
   getIsShareScreen: (state) => state.isShareScreen,
   getIsAudioParticipants: (state) => state.isAudioParticipants,
   getIsFullScreen: (state) => state.isFullScreen,
   settings_getIsVideo: state => state.settings_isVideo,
   getVideoDeviceId: state => state.videoDeviceId,
   getAudioDeviceId: state => state.audioDeviceId,
}
const actions = {
   updateToggleAudioParticipants({commit,state}){
      commit(twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS,!state.isAudioParticipants)
   },
   updateJwtToken({commit},token){
      commit(twilio_SETTERS.JWT_TOKEN,token)
   },
   sendDataTrack({commit},data){
      commit(twilio_SETTERS.DATA_TRACK,data)
   },
   updateVideoDeviceId({commit},id){
      global.localStorage.setItem('sb-videoTrackId',id);
      commit(twilio_SETTERS.VIDEO_DEVICE_ID,id)
   },
   updateAudioDeviceId({commit},id){
      global.localStorage.setItem('sb-audioTrackId',id);
      commit(twilio_SETTERS.AUDIO_DEVICE_ID,id)
   },
   updateVideoTrack({commit},trackId){
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
   updateAudioToggleByRemote({commit,state},val){
      if(state.isAudioAvailable){
         commit(twilio_SETTERS.AUDIO_TOGGLE,val);
      }
   },
}
export default {
   state,
   mutations,
   getters,
   actions
}