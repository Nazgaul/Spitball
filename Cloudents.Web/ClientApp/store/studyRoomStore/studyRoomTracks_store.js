import { createLocalVideoTrack,createLocalAudioTrack } from "twilio-video";

const _getLocalTrack = (getters,type) =>{
    if(getters['activeRoom'] && getters['activeRoom'].localParticipant) {
        let localTracks = getters['activeRoom'].localParticipant[`${type}Tracks`].entries()
            let currentTrack;
            for(let trackObj of localTracks){
                if(trackObj){
                    currentTrack = trackObj
                }
            }
        return !!currentTrack ? currentTrack[1] : currentTrack
    }
}

const state = {
    currentVideoTrack: null,
    currentAudioTrack: null,
    localVideoTrack: null,
    localAudioTrack: null,
};

const getters = {
    getCurrentVideoTrack:state => state.currentVideoTrack,
    getCurrentAudioTrack:state => state.currentAudioTrack,
    getLocalVideoTrack:state => state.localVideoTrack,
    getLocalAudioTrack:state => state.localAudioTrack,
};

const mutations = {
    setCurrentVideoTrack(state, track){
        state.currentVideoTrack = track;
    },
    setCurrentAudioTrack(state, track){
        state.currentAudioTrack = track;
    },
    setLocalVideoTrack(state, track){
        state.localVideoTrack = track
    },
    setLocalAudioTrack(state, track){
        state.localAudioTrack = track
    }
};

const actions = {
    setLocalVideoTrack({commit}, videoTrack){
        commit('setLocalVideoTrack',videoTrack)
    },
    createLocalVideoTrack({getters,dispatch}, cameraId){
        createLocalVideoTrack({exact: cameraId}).then(videoTrack => {
            getters['activeRoom'].localParticipant.publishTrack(videoTrack.mediaStreamTrack);  
                dispatch('setLocalVideoTrack',videoTrack)
            }
        )
    },
    createLocalAudioTrack({getters,commit}){
        createLocalAudioTrack().then(audioTrack => {
            getters['activeRoom'].localParticipant.publishTrack(audioTrack.mediaStreamTrack);  
            commit('setLocalAudioTrack',audioTrack)
            }
        )
    },
    destroyLocalVideoTrack({getters,dispatch},track){
        if(track.isEnabled){
            if (track.detach) {
                track.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
                getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
                dispatch('setLocalVideoTrack',null)
            }
        } 
    },
    destroyLocalAudioTrack({getters,commit},track){
        if(track.isEnabled){
            getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
            commit('setLocalAudioTrack',null)
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
            container.appendChild(track.attach());
        }
    },
    toggleVideoTrack({getters,dispatch}){
        if(!getters['activeRoom']) return
        
        let currentTrack = _getLocalTrack(getters,'video')
        if(currentTrack){
            dispatch('destroyLocalVideoTrack',currentTrack.track)
        } else{
            dispatch('createLocalVideoTrack')
        }
    },
    toggleAudioTrack({getters,dispatch}){
        if(!getters['activeRoom']) return 

        let currentTrack = _getLocalTrack(getters,'audio')
        if(currentTrack){
            dispatch('destroyLocalAudioTrack',currentTrack.track)
        } else{
            dispatch('createLocalAudioTrack')   
        } 
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}