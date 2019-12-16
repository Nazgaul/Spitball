import videoStreamService from '../../services/videoStreamService';
import { LanguageService } from '../../services/language/languageService';
import tutorService from '../../components/studyroom/tutorService';


const state = {
    isRoomFull: false,
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
    studyRoomData: null,
    roomStateEnum: {
        pending: "pending",
        ready: "ready",
        loading: "loading",
        active: "active"
    },
    startSessionDialogStateEnum:{
        start: 'start',
        waiting: 'waiting',
        needPayment: 'needPayment',
        disconnected: 'disconnected',
        finished: 'finished'
    },
    tutorDialogState:'start',
    studentDialogState:'waiting',
    sessionStartClickedOnce: false,
    sessionEndClicked: false,
    currentRoomState: "pending",
    jwtToken: null,
    studentStartDialog: false,
    tutorStartDialog: false,
    endDialog: false,
    settingsDialogState: false,
    activeNavIndicator: 'white-board',
    deviceValidationError:false,
    DevicesObject: tutorService.createDevicesObj(),
    sessionTimeStart: null,
    sessionTimeEnd: null,
    showUserConsentDialog: false,
    
};
const getters = {
    getIsRoomFull: state => state.isRoomFull,
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
    getJwtToken: state => state.jwtToken,
    getRoomId: state => state.roomId,
    getStudentStartDialog: state => state.studentStartDialog,
    getTutorStartDialog: state => state.tutorStartDialog,
    getSessionStartClickedOnce: state => state.sessionStartClickedOnce,
    getEndDialog: state => state.endDialog,
    getSessionEndClicked: state => state.sessionEndClicked,
    getBrowserSupportDialog: state => state.browserSupportDialog,
    getStudyRoomSettingsDialog: state => state.settingsDialogState,
    showDeviceValidationError: state => state.deviceValidationError,
    getDevicesObj: state=> state.DevicesObject,
    getActiveNavIndicator: state => state.activeNavIndicator,
    getTutorDialogState: state => state.tutorDialogState,
    getStudentDialogState: state => state.studentDialogState,
    getSessionTimeStart: state => state.sessionTimeStart,
    getSessionTimeEnd: state => state.sessionTimeEnd,
    getShowUserConsentDialog: state => state.showUserConsentDialog,
};

const mutations = {
    setIsRoomFull(state,val){
        state.isRoomFull = val;
    },
    setEndDialog(state, val) {
        state.endDialog = val;
    },
    updateSessionClickedOnce(state, val) {
        state.sessionStartClickedOnce = val;
    },
    updateSessionEndClicked(state, val) {
        state.sessionEndClicked = val;
    },
    setStudentStartDialog(state, val) {
        state.studentStartDialog = val;
    },
    setTutorStartDialog(state, val) {
        state.tutorStartDialog = val;
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
    setStudyRoomSettingsDialog(state, val){
        state.settingsDialogState = val;
    },
    setActiveNavIndicator(state,{activeNav}){
        state.activeNavIndicator = activeNav;
    },
    setDeviceValidationError(state, val){
        state.deviceValidationError = val;
    },
    setTutorDialogState(state, val){
        state.tutorDialogState = val;
    },
    setStudentDialogState(state, val){
        state.studentDialogState = val;
    },
    setSessionTimeStart(state, val){
        state.sessionTimeStart = val;
    },
    setSessionTimeEnd(state, val){
        state.sessionTimeEnd = val;
    },
    setShowUserConsentDialog(state, val){
        state.showUserConsentDialog = val;
    },
};

const actions = {
    updateEndDialog({commit, state}, val){
        commit('setEndDialog', val);
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
    updateStudyRoomProps({dispatch,commit,state}, val) {
        //update leaveReview store, to prevent leaving of multiple reviews
        dispatch('updateAllowReview',  val.allowReview);
        commit('setStudyRoomProps', val);
        if(!val.isTutor && val.needPayment){
            setTimeout(()=>{
                videoStreamService.enterRoom();
            }, 500);
        }
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
    updateCurrentRoomState({commit, state, dispatch}, val) {
        commit('setCurrentRoomState', val);
        if(state.roomStateEnum[val] === state.roomStateEnum['active']){
            setTimeout(()=>{
                dispatch('hideRoomToasterMessage');
            }, 3000)
        }
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
        // if(onlineCount === 2){
        //     commit('setIsRoomFull',true);
        // } else{
        //     commit('setIsRoomFull',false);
        // }

        let totalOnline = notificationObj.totalOnline;
        let jwtToken = notificationObj.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        let toasterParams = {};
        if(jwtToken){
            commit('setJwtToken', jwtToken);
            dispatch('updateCurrentRoomState', state.roomStateEnum.active);
            videoStreamService.createVideoSession();
            if(isTutor){
                toasterParams.text = LanguageService.getValueByKey('studyRoom_student_entered_room');
                dispatch('showRoomToasterMessage', toasterParams);
            }else{
                toasterParams.text = LanguageService.getValueByKey('studyRoom_tutor_entered_room');
                dispatch('showRoomToasterMessage', toasterParams);
            }
        }else{
            if(isTutor) {
                if(onlineCount == totalOnline) {
                    dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                    toasterParams.text = LanguageService.getValueByKey('studyRoom_student_entered_room');
                    dispatch('showRoomToasterMessage', toasterParams);
                    //show tutor start session
                    if(!state.studyRoomData.needPayment){
                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.start);
                        // dispatch("updateTutorStartDialog", true);
                    }else{
                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.needPayment);
                    }
                } else {
                    if(state.currentRoomState !== state.roomStateEnum.active){
                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.waiting);
                    }else{
                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.disconnected);
                    }
                    dispatch('updateTutorStartDialog', true);
                    // dispatch("updateTutorStartDialog", false);
                    toasterParams.text = LanguageService.getValueByKey('studyRoom_alone_in_room');
                    toasterParams.timeout = 3600000;
                    dispatch('showRoomToasterMessage', toasterParams);
                    dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
                }
            } else {
            // if is STUDENT
            if(onlineCount == totalOnline) {
                // toasterParams.text = LanguageService.getValueByKey('studyRoom_tutor_entered_room');
                // dispatch('showRoomToasterMessage', toasterParams);
                if(!state.studyRoomData.needPayment){
                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                }else{
                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.needPayment);
                }
                toasterParams.text = LanguageService.getValueByKey('studyRoom_waiting_for_tutor_toaster');
                toasterParams.timeout = 3600000;
                dispatch('showRoomToasterMessage', toasterParams);
            } else {
                if(!state.studyRoomData.needPayment){
                    console.log(state.currentRoomState);
                    if(state.currentRoomState === state.roomStateEnum.pending){
                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                    }else{
                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.disconnected);
                    }
                }else{
                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.needPayment);
                }
                
                dispatch('updateStudentStartDialog', true);
                dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
                toasterParams.text = LanguageService.getValueByKey('studyRoom_alone_in_room');
                toasterParams.timeout = 3600000;
                dispatch('showRoomToasterMessage', toasterParams);
                //hide student start se3ssion
                // dispatch("updateStudentStartDialog", false);
            }
        }
        }
    },
    showRoomToasterMessage({dispatch}, toasterParams) {
        return;
        let toasterObj = {
            toasterText: toasterParams.text,
            showToaster: true,
            toasterType: toasterParams.type ? toasterParams.type : '',
            toasterTimeout: toasterParams.timeout
        };
        dispatch('updateToasterParams', toasterObj);
    },
    hideRoomToasterMessage({dispatch}) {
        let toasterObj = {
            showToaster: false,
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
                dispatch("setStudentDialogState", state.startSessionDialogStateEnum.start);
                // dispatch("updateStudentStartDialog", true);
                dispatch('hideRoomToasterMessage');
            }, 3000);
        }else{
            setTimeout(()=>{
                dispatch("setTutorDialogState", state.startSessionDialogStateEnum.waiting);
            }, 2500);
            
        }
    },
    releasePaymeStatus_studyRoom({dispatch,state}){
        state.studyRoomData.needPayment = false;
        let isTutor = state.studyRoomData.isTutor;
        if(isTutor) {
            dispatch("setTutorDialogState", state.startSessionDialogStateEnum.start);
            // dispatch("updateTutorStartDialog", true);
        }else{
            dispatch("updatePaymentDialogState", false);
            dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
        }
    },
    setRoomId({commit}, val) {
        commit('setRoomId', val);
    },
    setBrowserSupportDialog({commit}, val){
      commit('setBrowserSupportDialog', val);
    },
    setStudyRoomSettingsDialog({commit}, val){
        commit('setStudyRoomSettingsDialog', val);
    },
    setDeviceValidationError({commit}, val){
        commit('setDeviceValidationError', val);
    },
    setTutorDialogState({commit}, val){
        commit('setTutorDialogState', val);
    },
    setStudentDialogState({commit}, val){
        commit('setStudentDialogState', val);
    },
    setSessionTimeStart({commit}){
        commit('setSessionTimeStart', Date.now());
    },
    setSessionTimeEnd({commit}){
        commit('setSessionTimeEnd', Date.now());
    },
    setShowUserConsentDialog({commit}, val){
        commit('setShowUserConsentDialog', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
};
