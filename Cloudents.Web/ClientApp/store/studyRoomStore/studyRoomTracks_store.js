import { createLocalVideoTrack,createLocalAudioTrack } from "twilio-video";
import studyRoomRecordingService from '../../components/studyroom/studyRoomRecordingService';

const _getLocalTrack = (getters,type) =>{
    if(getters['activeRoom'] && getters['activeRoom'].localParticipant) {
        let localTracks = getters['activeRoom'].localParticipant[`${type}Tracks`].entries();
        let currentTrack;
            for(let trackObj of localTracks){
                if(trackObj){
                    currentTrack = trackObj;
                }
            }
        return !!currentTrack ? currentTrack[1] : currentTrack;
    }
};

const state = {
    currentVideoTrack: null,
    currentAudioTrack: null,
    localVideoTrack: null,
    localAudioTrack: null,
    isRemote: false,
    lastActiveLocalVideoTrack: null,
    videoDevice: null,
    audioDevice: null,
    storageENUM:{
        video:'sb-videoTrackId',
        audio:'sb-audioTrackId'
    },
    isVideoActive: false,
    isAudioActive: false,
    localDisplayMedia: null,
    localUserMedia: null
};

const getters = {
    getCurrentVideoTrack:state => state.currentVideoTrack,
    getCurrentAudioTrack:state => state.currentAudioTrack,
    getLocalVideoTrack:state => state.localVideoTrack,
    getLocalAudioTrack:state => state.localAudioTrack,
    releaseFullVideoButton:state => state.isRemote,
    getLastActiveLocalVideoTrack: state => state.lastActiveLocalVideoTrack,
    getIsVideoActive: state => state.isVideoActive,
    getIsAudioActive: state => state.isAudioActive,
    getLocalDisplayMedia: state => state.localDisplayMedia,
    getLocalUserMedia: state => state.localUserMedia,
};

const mutations = {
    setCurrentVideoTrack(state, track){
        state.currentVideoTrack = track;
    },
    setCurrentAudioTrack(state, track){
        state.currentAudioTrack = track;
    },
    setLocalVideoTrack(state, track){
        state.localVideoTrack = track;
    },
    setLastActiveLocalVideoTrack(state, track){
        state.lastActiveLocalVideoTrack = track;
    },
    setLocalAudioTrack(state, track){
        state.localAudioTrack = track;
    },
    releaseFullVideoButton(state,val){
        state.isRemote = val;
    },
    setIsVideoActive(state, val){
        state.isVideoActive  = val;
    },
    setIsAudioActive(state, val){
        state.isAudioActive  = val;
    },
    setLocalDisplayMedia(state, val){
        state.localDisplayMedia  = val;
    },
    setLocalUserMedia(state, val){
        state.localUserMedia  = val;
    },
};

const actions = {
    initLocalMediaTracks({dispatch}){
        dispatch('setIsVideoActive', true);
        dispatch('initLocalVideoTrack');
        dispatch('setIsAudioActive', true);
        dispatch('initLocalAudioTrack');
    },
    setLocalVideoTrack({commit}, videoTrack){
        commit('setLocalVideoTrack',videoTrack);
        if(!!videoTrack){
            commit('setLastActiveLocalVideoTrack',videoTrack);
        }
    },
    initLocalVideoTrack({getters,dispatch,state}){
        let deviceId = global.localStorage.getItem(state.storageENUM.video);
        let mediaStreamTrack = state.lastActiveLocalVideoTrack;
        if(mediaStreamTrack){
            //after changing the 
            dispatch('setLocalVideoTrack',mediaStreamTrack);
            getters['activeRoom'].localParticipant.publishTrack(mediaStreamTrack);
        }else{
            dispatch('createLocalVideoTrack_Store', deviceId);
        }
    },
    createLocalVideoTrack_Store({getters, dispatch, state}, id){
        let param = id ? {deviceId: {exact: id}} : {};
        createLocalVideoTrack(param).then(videoTrack => {
            if(getters['activeRoom'] && state.isVideoActive){
                getters['activeRoom'].localParticipant.publishTrack(videoTrack.mediaStreamTrack);  
            }
                dispatch('setLocalVideoTrack',videoTrack);
        },()=>{
                createLocalVideoTrack().then(videoTrack => {
                    if(getters['activeRoom'] && state.isVideoActive){
                        getters['activeRoom'].localParticipant.publishTrack(videoTrack.mediaStreamTrack);  
                    }
                        dispatch('setLocalVideoTrack',videoTrack);
                },()=>{
                        dispatch('setIsVideoActive', false);
                        dispatch('setLocalVideoTrack', null);
                });
        });
    },
    initLocalAudioTrack({dispatch}){
        let deviceId = global.localStorage.getItem(state.storageENUM.audio);
        dispatch('createLocalAudioTrack_store', deviceId);
    },
    createLocalAudioTrack_store({getters, commit, dispatch}, id){
        let param = id ? {deviceId: {exact: id}} : {};
        createLocalAudioTrack(param).then(audioTrack => {
            if(getters['activeRoom'] && state.isAudioActive){
                getters['activeRoom'].localParticipant.publishTrack(audioTrack.mediaStreamTrack);  
            }
            commit('setLocalAudioTrack',audioTrack);
        },()=>{
                createLocalAudioTrack().then(audioTrack => {
                    if(getters['activeRoom'] && state.isAudioActive){
                        getters['activeRoom'].localParticipant.publishTrack(audioTrack.mediaStreamTrack);  
                    }
                    commit('setLocalAudioTrack',audioTrack);
                },()=>{
                        dispatch('setIsAudioActive', false);
                        commit('setLocalAudioTrack',null);
                });
        });
    },
    destroyLocalVideoTrack({getters,dispatch},track){
        if(track.isEnabled){
            if (track.detach) {
                track.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
                getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
                dispatch('setLocalVideoTrack',null);
            }
        } 
    },
    destroyLocalAudioTrack({getters,commit},track){
        if(track.isEnabled){
            getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
            commit('setLocalAudioTrack',null);
        } 
    },
    setCurrentVideoTrack({commit}, track){
        commit('setCurrentVideoTrack', track);
    },
    setCurrentAudioTrack({commit}, track){
        commit('setCurrentAudioTrack', track);
    },
    updateRemoteTrack({state, dispatch, getters}, updateObj){
        let type = updateObj.type;
        let track = updateObj.track;
        let container = updateObj.container;
        if(type === 'video'){
            let videoTrack = state.currentVideoTrack;
            if (!!videoTrack && videoTrack.detach) {
                videoTrack.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
                getters['activeRoom'].localParticipant.unpublishTrack(videoTrack.mediaStreamTrack);
            }
            dispatch('setCurrentVideoTrack', track);
            container.appendChild(track.attach());
        }
        if(type === 'audio'){
            
            let AudioTrack = state.currentAudioTrack;
            if (!!AudioTrack && AudioTrack.detach) {
                
                AudioTrack.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
                getters['activeRoom'].localParticipant.unpublishTrack(AudioTrack.mediaStreamTrack);
            }
            dispatch('setCurrentAudioTrack', track);
            let recorderStream = getters.getRecorder;
            if(recorderStream){
                // recorderStream.requestData(); //init the dataAvailable Event;
                let recorderTrackToAdd = studyRoomRecordingService.createRemoteAudioStream();
                studyRoomRecordingService.createCombinedMediaStreams(recorderTrackToAdd)
            }
            container.appendChild(track.attach());
        }
    },
    toggleVideoTrack({getters,dispatch, state}){
        if(!getters['activeRoom']){
            if(state.isVideoActive){
                dispatch('setIsVideoActive', false);
            }else{
                dispatch('setIsVideoActive', true);
            }
        }else{
            let currentTrack = _getLocalTrack(getters,'video');
            if(currentTrack){
                dispatch('setIsVideoActive', false);
                dispatch('destroyLocalVideoTrack',currentTrack.track);
            } else{
                dispatch('setIsVideoActive', true);
                dispatch('initLocalVideoTrack');
            }
        } 
    },
    toggleAudioTrack({getters,dispatch, state}){
        if(!getters['activeRoom']){
            if(state.isAudioActive){
                dispatch('setIsAudioActive', false);
            }else{
                dispatch('setIsAudioActive', true);
            }
        }else{
            let currentTrack = _getLocalTrack(getters,'audio');
            if(currentTrack){
                dispatch('setIsAudioActive', false);
                dispatch('destroyLocalAudioTrack',currentTrack.track);
            } else{
                dispatch('setIsAudioActive', true);
                dispatch('initLocalAudioTrack');  
            } 
        } 
    },

    changeVideoTrack({getters,dispatch}, newTrackId){
        if(getters['activeRoom']){
            let currentTrack = _getLocalTrack(getters,'video');
            if(currentTrack){
                //first Unpublish the current track
                //dispatch('setIsVideoActive', false);
                dispatch('destroyLocalVideoTrack',currentTrack.track);
                //create and publish new track
                //dispatch('setIsVideoActive', true);
                dispatch('createLocalVideoTrack_Store', newTrackId);
            }else{
                //dispatch('setIsVideoActive', true);
                dispatch('createLocalVideoTrack_Store', newTrackId);
            }
        }else{
            //dispatch('setIsVideoActive', true);
            dispatch('createLocalVideoTrack_Store', newTrackId);
        }
    },
    changeAudioTrack({getters,dispatch}, newTrackId){
        if(getters['activeRoom']){
            let currentTrack = _getLocalTrack(getters,'audio');
            if(currentTrack){
                //first Unpublish the current track
                // dispatch('setIsAudioActive', false);
                dispatch('destroyLocalAudioTrack',currentTrack.track);
                //create and publish new track
                // dispatch('setIsAudioActive', true);
                dispatch('createLocalAudioTrack_store', newTrackId);
            }else{
                // dispatch('setIsAudioActive', true);
                dispatch('createLocalAudioTrack_store', newTrackId);
            }
        }else{
            // dispatch('setIsAudioActive', true);
            dispatch('createLocalAudioTrack_store', newTrackId);
        }
    },
    setIsVideoActive({commit}, val){
        commit('setIsVideoActive', val);
    },
    setIsAudioActive({commit}, val){
        commit('setIsAudioActive', val);
    },
    setLocalDisplayMedia({commit}, val){
        commit('setLocalDisplayMedia', val)
    },
    setLocalUserMedia({commit}, val){
        commit('setLocalUserMedia', val)
    },
};

export default {
    state,
    mutations,
    getters,
    actions
}