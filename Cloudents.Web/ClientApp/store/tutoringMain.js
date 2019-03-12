const state = {
    identity: '',
    isRoom: false,
    roomId: '',
    isRoomFull: false,
    currentActiveRoom: {},
    isLocalOffline: true,
    isRemoteOffline: true
};
const getters = {
    activeRoom:  state => state.currentActiveRoom,
    localOffline: state => state.isLocalOffline,
    remoteOffline: state => state.isRemoteOffline,
    userIdentity: state => state.identity,
    isRoomCreated: state => state.isRoom,
    roomLinkID: state => state.roomId,
    isRoomFull: state => state.isRoomFull
};

const mutations = {
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
        state.currentActiveRoom.disconnect()
    },
    setRoomInstance(state, data){
        state.currentActiveRoom = data
    },
    setLocalStatus(state, val){
        state.isLocalOffline = val;
    },
    setRemoteStatus(state, val){
        state.isRemoteOffline = val;
    }
};

const actions = {
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