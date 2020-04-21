import {twilio_SETTERS} from '../constants/twilioConstants.js';

const state = {
   jwtToken: null,
   isVideoActive: true,
   isAudioActive: true,
   isVideoAvailable: false,
   isAudioAvailable: false,
   isShareScreen: false,
   videoTracks: [],
   isFullScreen:false,
   isAudioParticipants:true,
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
   [twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE]: (state,val) => state.isShareScreen = val,
   [twilio_SETTERS.ADD_REMOTE_VIDEO_TRACK]: (state,videoTrack) => {
      let remoteTrackId = `${videoTrack.sid || videoTrack.trackSid}`
      videoTrack.sb_video_id = remoteTrackId;
      let idx;
      let isTrackInList = state.videoTracks.some((t,i)=>{idx = i;return t.sb_video_id == remoteTrackId})
      if(isTrackInList){
         state.videoTracks.splice(idx,1);
      }else{
         if(videoTrack.attach){
            state.videoTracks.push(videoTrack)
         }
      }
   },
   [twilio_SETTERS.DELETE_REMOTE_VIDEO_TRACK]: (state,track) => {
      if(!track.sb_video_id) return;
      let idx;
      let isInList = state.videoTracks.some((t,i)=>{idx = i;return t.sb_video_id == track.sb_video_id})
      if(isInList){
         state.videoTracks.splice(idx,1)
      }
   },
   [twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN]:(state,val)=> state.isFullScreen = val,
   [twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS]:(state,val)=> state.isAudioParticipants = val,
}

const getters = {
   getJwtToken: (state) => state.jwtToken,
   getIsVideoActive: (state) => state.isVideoAvailable && state.isVideoActive,
   getIsAudioActive: (state) => state.isAudioAvailable && state.isAudioActive,
   getIsShareScreen: (state) => state.isShareScreen,
   getVideoTrackList: (state) => state.videoTracks,
   getIsAudioParticipants: (state) => state.isAudioParticipants,
   getIsFullScreen: (state) => state.isFullScreen
}
const actions = {
   updateToggleAudioParticipants({commit,state}){
      commit(twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS,!state.isAudioParticipants)
   },
   updateToggleTutorFullScreen({commit},val){
      commit(twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN,val)
   },
   updateJwtToken({commit,getters},token){
      if(!getters.getRoomIsTutor){
         commit('setComponent', 'simpleToaster_sessionStarted');
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