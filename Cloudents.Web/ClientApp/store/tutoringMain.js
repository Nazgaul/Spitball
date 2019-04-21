import tutorService from '../components/tutor/tutorService';

const state = {
    identity: '',
    isRoom: false,
    roomId: '',
    currentActiveRoom: null,
    localParticipant: null,
    localParticipantsNetworkQuality: null,
    isLocalOffline: true,
    isRemoteOffline: true,
    isRoomLoading: false,
    isFirepadLoadedOnce: false,
    qualityDialogVisibility: false,
    notAllowedDevices: false,
    notAvaliableDevices: false,
    studyRoomData: null,
    roomStateEnum: {
        pending: "pending",
        ready: "ready",
        loading: "loading",
        active: "active"
    },
    currentRoomState: "pending",
    jwtToken: null,
};
const getters = {
    activeRoom: state => state.currentActiveRoom,
    localOffline: state => state.isLocalOffline,
    localNetworkQuality: state => state.localParticipantsNetworkQuality,
    localParticipant: state => state.localParticipant,
    remoteOffline: state => state.isRemoteOffline,
    userIdentity: state => state.identity,
    isRoomCreated: state => state.isRoom,
    roomLinkID: state => state.roomId,
    roomLoading: state => state.isRoomLoading,
    firepadLoadedOnce: state => state.isFirepadLoadedOnce,
    qualityDialog: state => state.qualityDialogVisibility,
    getNotAllowedDevices: state => state.notAllowedDevices,
    getNotAvaliableDevices: state=> state.notAvaliableDevices,
    getCurrentRoomState: state => state.currentRoomState,
    getStudyRoomData: state => state.studyRoomData,
    getJwtToken: state => state.jwtToken
};

const mutations = {
    updateAllowedDevices(state, val){
        state.notAllowedDevices = val
    },
    updateAvaliableDevices(state, val){
        state.notAvaliableDevices = val
    },
    setStudyRoomProps(state, val) {
        state.studyRoomData = val;
    },
    setlocalParticipantObj(state, val) {
        state.localParticipant = val;
    },
    setNetworkQuality(state, val) {
        state.localParticipantsNetworkQuality = val;
    },
    setRoomId(state, val) {
        state.roomId = val;
    },
    setRoomStatus(state, val) {
        state.isRoom = val;
    },
    setUserIdentity(state, val) {
        state.identity = val;
    },
    leaveIfJoinedRoom(state) {
        if(state.currentActiveRoom) {
            state.currentActiveRoom.disconnect();
        }
    },
    setRoomInstance(state, data) {
        state.currentActiveRoom = data;
    },
    setLocalStatus(state, val) {
        state.isLocalOffline = val;
    },
    setRemoteStatus(state, val) {
        state.isRemoteOffline = val;
    },
    setRoomLoading(state, val) {
        state.isRoomLoading = val;
    },
    changeFirepadLoaded(state, val) {
        state.isFirepadLoadedOnce = val;
    },
    setqualityDialogState(state, val) {
        state.qualityDialogVisibility = val;
    },
    setCurrentRoomState(state, val) {
        if(!!state.roomStateEnum[val]) {
            state.currentRoomState = val;
        }
    },
    setJwtToken(state, val){
        state.jwtToken = val;
    }
};

const actions = {
    setAvaliableDevicesStatus({commit, state}, val) {
        commit('updateAvaliableDevices', val);
    },
    setAllowedDevicesStatus({commit, state}, val) {
        commit('updateAllowedDevices', val);
    },
    updateStudyRoomProps({commit, state}, val) {
        let roomData = tutorService.createRoomProps(val);
        commit('setStudyRoomProps', roomData);
    },
    updateTestDialogState({commit, state}, val) {
        commit('setqualityDialogState', val);
    },
    updateLocalParticipant({commit, state}, val) {
        commit('setlocalParticipantObj', val);
    },
    updateLocalParticipantsNetworkQuality({commit, state}, val) {
        commit('setNetworkQuality', val);
    },
    updateCodeLoadedOnce({commit, state}, val) {
        commit('changeFirepadLoaded', val);
    },

    updateRoomLoading({commit, state}, val) {
        commit('setRoomLoading', val);
    },
    leaveRoomIfJoined({commit}) {
        commit('leaveIfJoinedRoom');
    },
    updateRoomInstance({commit, state}, data) {
        commit('setRoomInstance', data);
    },
    updateLocalStatus({commit, state}, val) {
        commit('setLocalStatus', val);
    },
    updateRemoteStatus({commit, state}, val) {
        commit('setRemoteStatus', val);
    },
    updateRoomID({commit, state}, val) {
        commit('setRoomId', val);
    },
    updateRoomStatus({commit, state}, val) {
        commit('setRoomStatus', val);
    },
    updateUserIdentity({commit, state}, val) {
        commit('setUserIdentity', val);
    },
    updateCurrentRoomState({commit}, val) {
        commit('setCurrentRoomState', val);
    },
    signalRUpdateState({commit, dispatch, state}, notificationObj) {
        //TODO Update state according to the singnalR data
        let onlineCount = notificationObj.onlineCount;
        let totalOnline = notificationObj.totalOnline;
        let isTutor = state.studyRoomData.isTutor;
        if(isTutor){
            if(state.currentRoomState !== state.roomStateEnum.active) {
                if(onlineCount == totalOnline) {
                    dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                } else {
                    dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
                }
            } else {
                if(onlineCount == totalOnline) {
                    //reconnect
                } else {
                    // think what to do in case session is active and not all are connected
                }
            }
        }
    },

    signalRSetJwtToken({commit, dispatch, state}, sessionInformation){
        let token = sessionInformation.data.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        commit('setJwtToken', token);
        if(!isTutor){
            dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
        }
    }
};
export default {
    state,
    mutations,
    getters,
    actions
};