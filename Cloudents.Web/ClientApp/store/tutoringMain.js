import tutorService from '../components/tutor/tutorService';

const state = {
    identity: '',
    isRoom: false,
    // roomId: '',
    currentActiveRoom: null,
    localParticipant: null,
    localParticipantsNetworkQuality: null,
    isLocalOffline: true,
    isRemoteOffline: true,
    isRoomLoading: false,
    isFirepadLoadedOnce: false,
    qualityDialogVisibility: false,
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
    roomLoading: state => state.isRoomLoading,
    firepadLoadedOnce: state => state.isFirepadLoadedOnce,
    qualityDialog: state => state.qualityDialogVisibility,
    getCurrentRoomState: state => state.currentRoomState,
    getStudyRoomData: state => state.studyRoomData,
    getJwtToken: state => state.jwtToken
};

const mutations = {
    setStudyRoomProps(state, val) {
        state.studyRoomData = val;
    },
    setlocalParticipantObj(state, val) {
        state.localParticipant = val;
    },
    setNetworkQuality(state, val) {
        state.localParticipantsNetworkQuality = val;
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
    updateStudyRoomProps(context, val) {
        let roomData = tutorService.createRoomProps(val);
        let allowReview = roomData.allowReview;
        context.dispatch('updateAllowReview',allowReview);
        context.commit('setStudyRoomProps', roomData);
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