const state = {
    currentVideoTrack: null,
    currentAudioTrack: null
};

const getters = {
    getCurrentVideoTrack:state => state.currentVideoTrack,
    getCurrentAudioTrack:state => state.currentAudioTrack,
};

const mutations = {
    setCurrentVideoTrack(state, track){
        state.currentVideoTrack = track;
    },
    setCurrentAudioTrack(state, track){
        state.currentAudioTrack = track;
    }
};

const actions = {
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
                getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
                getters['activeRoom'].localParticipant.removeTrack(track.mediaStreamTrack);
                // getters['activeRoom'].participants[0].value._removeTrackPublication()
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
                getters['activeRoom'].localParticipant.unpublishTrack(track.mediaStreamTrack);
            }
            dispatch('setCurrentAudioTrack', track);
            container.appendChild(track.attach());
        }
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}