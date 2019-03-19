const state = {
    identity: '',
    isRoom: false,
    roomId: '',
    isRoomFull: false,
    currentActiveRoom: null,
    localParticipant: null,
    localParticipantsNetworkQuality : null,
    isLocalOffline: true,
    isRemoteOffline: true,
    isRoomLoading: false,
    isFirepadLoadedOnce: false
};
const getters = {
    activeRoom:  state => state.currentActiveRoom,
    localOffline: state => state.isLocalOffline,
    localNetworkQuality: state => state.localParticipantsNetworkQuality,
    localParticipant: state => state.localParticipant,
    remoteOffline: state => state.isRemoteOffline,
    userIdentity: state => state.identity,
    isRoomCreated: state => state.isRoom,
    roomLinkID: state => state.roomId,
    isRoomFull: state => state.isRoomFull,
    roomLoading : state => state.isRoomLoading,
    firepadLoadedOnce: state => state.isFirepadLoadedOnce
};

const mutations = {
    setlocalParticipantObj(state, val){
     state.localParticipant = val
    },
    setNetworkQuality(state, val){
        state.localParticipantsNetworkQuality= val
    },
    setRoomIsFull(state, val) {
        state.isRoomFull = val
    },
    setRoomId(state, val) {
        state.roomId = val
    },
    setRoomStatus(state, val) {
        state.isRoom = val;
    },
    setUserIdentity(state, val) {
        state.identity = val;
    },
    leaveIfJoinedRoom(state) {
        if(state.currentActiveRoom){
            state.currentActiveRoom.disconnect()
        }
     },
    setRoomInstance(state, data){
        state.currentActiveRoom = data
    },
    setLocalStatus(state, val){
        state.isLocalOffline = val;
    },
    setRemoteStatus(state, val){
        state.isRemoteOffline = val;
    },
    setRoomLoading(state, val){
        state.isRoomLoading = val
    },
    changeFirepadLoaded(state, val){
        state.isFirepadLoadedOnce  = val
    }

};

const actions = {
    updateLocalParticipant({commit, state}, val){
      commit('setlocalParticipantObj', val)
    },
    updateLocalParticipantsNetworkQuality({commit, state}, val){
      commit('setNetworkQuality', val)
    },
    updateCodeLoadedOnce({commit, state}, val){
        commit('changeFirepadLoaded', val)
    },

    updateRoomLoading({commit, state}, val){
        commit('setRoomLoading', val)
    },
    leaveRoomIfJoined({commit}) {
        commit('leaveIfJoinedRoom')
    },
    updateRoomInstance({commit, state}, data){
        commit('setRoomInstance', data)
    },
    updateLocalStatus({commit, state}, val){
        commit('setLocalStatus', val)
    },
    updateRemoteStatus({commit, state}, val){
        commit('setRemoteStatus', val)
    },
    updateRoomIsFull({commit, state}, val) {
        commit('setRoomIsFull', val)
    },
    updateRoomID({commit, state}, val) {
        commit('setRoomId', val)
    },
    updateRoomStatus({commit, state}, val) {
        commit('setRoomStatus', val)
    },
    updateUserIdentity({commit, state}, val) {
        commit('setUserIdentity', val)
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}