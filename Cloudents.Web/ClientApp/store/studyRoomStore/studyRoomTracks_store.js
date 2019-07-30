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
    isRemote: false,
    lastActiveLocalVideoTrack: null
};

const getters = {
    getCurrentVideoTrack:state => state.currentVideoTrack,
    getCurrentAudioTrack:state => state.currentAudioTrack,
    getLocalVideoTrack:state => state.localVideoTrack,
    getLocalAudioTrack:state => state.localAudioTrack,
    getIsRemote:state => state.isRemote,
    getLastActiveLocalVideoTrack: state => state.lastActiveLocalVideoTrack
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
    setLastActiveLocalVideoTrack(state, track){
        state.lastActiveLocalVideoTrack = track
    },
    setLocalAudioTrack(state, track){
        state.localAudioTrack = track
    },
    setIsRemote(state,val){
        state.isRemote = val
    }
};

const actions = {
    setLocalVideoTrack({commit}, videoTrack){
        commit('setLocalVideoTrack',videoTrack);
        if(!!videoTrack){
            commit('setLastActiveLocalVideoTrack',videoTrack);
        }
    },
    createLocalVideoTrack({getters,dispatch,state}, deviceId){
        let mediaStreamTrack = state.lastActiveLocalVideoTrack;
        if(mediaStreamTrack){
            dispatch('setLocalVideoTrack',mediaStreamTrack)
            let options = {};
            //TODO add track Information
            // let options = {
            //     name: `shareScreen_${this.isTutor ? "tutor" : "student"}_${
            //         this.accountUserID
            //         }`
            // }
            getters['activeRoom'].localParticipant.publishTrack(mediaStreamTrack, options);
        }else{
        createLocalVideoTrack({exact: deviceId}).then(videoTrack => {
                getters['activeRoom'].localParticipant.publishTrack(videoTrack.mediaStreamTrack);  
                    dispatch('setLocalVideoTrack',videoTrack)
                },err=>{
                    dispatch('setLocalVideoTrack', null)
                    console.log(err);
                }
            )
        }
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