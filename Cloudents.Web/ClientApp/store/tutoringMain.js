import videoStreamService from '../services/videoStreamService';
import { LanguageService } from '../services/language/languageService';

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
    browserSupportDialog: false,
    notAllowedDevices: false,
    notAvaliableDevices: false,
    studyRoomData: null,
    roomStateEnum: {
        pending: "pending",
        ready: "ready",
        loading: "loading",
        active: "active"
    },
    sessionStartClickedOnce: false,
    sessionEndClicked: false,
    currentRoomState: "pending",
    jwtToken: null,
    studentStartDialog: false,
    tutorStartDialog: false,
    endDialog: false
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
    getNotAllowedDevices: state => state.notAllowedDevices,
    getNotAvaliableDevices: state => state.notAvaliableDevices,
    getCurrentRoomState: state => state.currentRoomState,
    getStudyRoomData: state => state.studyRoomData,
    getJwtToken: state => state.jwtToken,
    getRoomId: state => state.roomId,
    getStudentStartDialog: state => state.studentStartDialog,
    getTutorStartDialog: state => state.tutorStartDialog,
    getSessionStartClickedOnce: state => state.sessionStartClickedOnce,
    getEndDialog: state => state.endDialog,
    getSessionEndClicked: state => state.sessionEndClicked,
    getBrowserSupportDialog: state => state.browserSupportDialog
};

const mutations = {
    setEndDialog(state, val) {
        state.endDialog = val;
    },

    updateSessionClickedOnce(state, val) {
        state.sessionStartClickedOnce = val;
    },
    updateSessionEndClicked(state, val) {
        state.sessionEndClicked = val;
    },
    updateAllowedDevices(state, val) {
        state.notAllowedDevices = val;
    },
    setStudentStartDialog(state, val) {
        state.studentStartDialog = val;
    },
    setTutorStartDialog(state, val) {
        state.tutorStartDialog = val;
    },
    updateAvaliableDevices(state, val) {
        state.notAvaliableDevices = val;
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
    setJwtToken(state, val) {
        state.jwtToken = val;
    },
    setRoomId(state, val) {
        state.roomId = val;
    },
    setBrowserSupportDialog(state, val){
      state.browserSupportDialog = val;
    },
};

const actions = {
    updateEndDialog({commit, state}, val){
        commit('setEndDialog', val)
    },
    setSesionClickedOnce({commit, state}, val) {
        commit('updateSessionClickedOnce', val);
    },
    setSesionEndClicked({commit, state}, val) {
        commit('updateSessionEndClicked', val);
    },
    setAvaliableDevicesStatus({commit, state}, val) {
        commit('updateAvaliableDevices', val);
    },
    setAllowedDevicesStatus({commit, state}, val) {
        commit('updateAllowedDevices', val);
    },
    updateStudyRoomProps(context, val) {
        //update leaveReview store, to prevent leaving of multiple reviews
        context.dispatch('updateAllowReview',  val.allowReview);
        context.commit('setStudyRoomProps', val);
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
    updateStudentStartDialog({commit}, val) {
        commit('setStudentStartDialog', val);
    },
    updateTutorStartDialog({commit}, val) {
        commit('setTutorStartDialog', val);
    },
    signalR_UpdateState({commit, dispatch, state}, notificationObj) {
        //TODO Update state according to the singnalR data
        let onlineCount = notificationObj.onlineCount;
        let totalOnline = notificationObj.totalOnline;
        let jwtToken = notificationObj.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        let toasterParams = {};
        if(isTutor) {
            if(!jwtToken) {
                if(onlineCount == totalOnline) {
                    dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                    toasterParams.text = LanguageService.getValueByKey('studyRoom_student_entered_room');
                    dispatch('showRoomToasterMessage', toasterParams);
                    //show tutor start session
                    dispatch("updateTutorStartDialog", true);
                } else {
                    dispatch("updateTutorStartDialog", false);
                    toasterParams.text = LanguageService.getValueByKey('studyRoom_alone_in_room');
                    toasterParams.timeout = 3600000;
                    dispatch('showRoomToasterMessage', toasterParams);
                    dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
                }
            } else {
                if(onlineCount == totalOnline) {
                    //reconnect
                    commit('setJwtToken', jwtToken);
                    videoStreamService.createVideoSession();
                } else {
                    // think what to do in case session is active and not all are connected
                }
            }
        } else {
            if(onlineCount == totalOnline) {
                toasterParams.text = LanguageService.getValueByKey('studyRoom_tutor_entered_room');
                dispatch('showRoomToasterMessage', toasterParams);
            } else {
                toasterParams.text = LanguageService.getValueByKey('studyRoom_alone_in_room');
                toasterParams.timeout = 3600000;
                dispatch('showRoomToasterMessage', toasterParams);
                //hide student start se3ssion
                dispatch("updateStudentStartDialog", false);
            }
        }
    },
    showRoomToasterMessage({dispatch}, toasterParams) {
        let toasterObj = {
            toasterText: toasterParams.text,
            showToaster: true,
            toasterType: toasterParams.type ? toasterParams.type : '',
            toasterTimeout: toasterParams.timeout
        };
        dispatch('updateToasterParams', toasterObj);
    },
    signalR_SetJwtToken({commit, dispatch, state}, sessionInformation) {
        let token = sessionInformation.data.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        commit('setJwtToken', token);
        if(!isTutor) {
            //show student start se3ssion
            // SPITBALL-1197 Tutoring - Session stuck on start (fix)
            setTimeout(()=>{
                dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                dispatch("updateStudentStartDialog", true);
            }, 3000)

        }
    },
    signalR_ReleasePaymeStatus({commit, dispatch, state}) {
        state.studyRoomData.needPayment = false;
        let isTutor = state.studyRoomData.isTutor;
        if(isTutor) {
            dispatch("updateStudentStartDialog", true);
        }
    },
    setRoomId({commit}, val) {
        commit('setRoomId', val);
    },
    setBrowserSupportDialog({commit}, val){
      commit('setBrowserSupportDialog', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
};
