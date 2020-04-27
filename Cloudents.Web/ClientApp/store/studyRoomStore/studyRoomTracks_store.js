// import studyRoomRecordingService from '../../components/studyroom/studyRoomRecordingService';

// const _getLocalTrack = (getters,type) =>{
//     if(getters['activeRoom'] && getters['activeRoom'].localParticipant) {
//         let localTracks = getters['activeRoom'].localParticipant[`${type}Tracks`].entries();
//         let currentTrack;
//             for(let trackObj of localTracks){
//                 if(trackObj){
//                     currentTrack = trackObj;
//                 }
//             }
//         return !!currentTrack ? currentTrack[1] : currentTrack;
//     }
// };

const state = {
//     currentAudioTrack: null,
//     videoDevice: null,
//     audioDevice: null,
};

const getters = {
//     getCurrentAudioTrack:state => state.currentAudioTrack,
};

const mutations = {
//     setCurrentAudioTrack(state, track){
//         state.currentAudioTrack = track;
//     },
};

const actions = {
//     destroyLocalVideoTrack({getters,dispatch},track){
//         if(track.isEnabled){
//             if (track.detach) {
//                 track.detach().forEach((detachedElement) => {
//                     detachedElement.remove();
//                 });
//                 getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
//             }
//         } 
//     },
//     destroyLocalAudioTrack({getters,commit},track){
//         if(track.isEnabled){
//             getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
//         } 
//     },
//     setCurrentAudioTrack({commit}, track){
//         commit('setCurrentAudioTrack', track);
//     },
//     updateRemoteTrack({state, dispatch, getters}, updateObj){
//         let type = updateObj.type;
//         let track = updateObj.track;
//         let container = updateObj.container;
//         if(type === 'video'){
//             let videoTrack = ss;
//             if (!!videoTrack && videoTrack.detach) {
//                 videoTrack.detach().forEach((detachedElement) => {
//                     detachedElement.remove();
//                 });
//                 getters['activeRoom'].localParticipant.unpublishTrack(videoTrack.mediaStreamTrack);
//             }
//             container.appendChild(track.attach());
//         }
//         if(type === 'audio'){
            
//             let AudioTrack = state.currentAudioTrack;
//             if (!!AudioTrack && AudioTrack.detach) {
                
//                 AudioTrack.detach().forEach((detachedElement) => {
//                     detachedElement.remove();
//                 });
//                 getters['activeRoom'].localParticipant.unpublishTrack(AudioTrack.mediaStreamTrack);
//             }
//             dispatch('setCurrentAudioTrack', track);
//             let recorderStream = getters.getRecorder;
//             if(recorderStream){
//                 // recorderStream.requestData(); //init the dataAvailable Event;
//                 let recorderTrackToAdd = studyRoomRecordingService.createRemoteAudioStream();
//                 studyRoomRecordingService.createCombinedMediaStreams(recorderTrackToAdd);
//             }
//             container.appendChild(track.attach());
//         }
//     },
//     stopTracks({state, dispatch}){
//         let currentVideoTrack = _getLocalTrack(getters,'video');
//         if(currentVideoTrack){
//             dispatch('destroyLocalVideoTrack',currentVideoTrack.track);
//         }
//         let currentAudioTrack = _getLocalTrack(getters,'audio');
//         if(currentAudioTrack){
//             dispatch('destroyLocalAudioTrack',currentAudioTrack.track);
//         }      
//     }
};

export default {
    state,
    mutations,
    getters,
    actions
}