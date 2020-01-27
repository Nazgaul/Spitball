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
    snapshotDialog: false,    
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
    getSnapshotDialog: state => state.snapshotDialog,
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
    setSnapshotDialog(state, val){
        state.snapshotDialog = val;
    },
};

const actions = {
    updateEndDialog({commit}, val){
        console.warn('DEBUG: 4 store: updateEndDialog')
        commit('setEndDialog', val);
    },
    setSesionClickedOnce({commit}, val) {
        console.warn('DEBUG: 5 store: setSesionClickedOnce VAL:',val)
        commit('updateSessionClickedOnce', val);
    },
    setSesionEndClicked({commit}, val) {
        console.warn('DEBUG: 6 store: setSesionEndClicked')
        commit('updateSessionEndClicked', val);
    },
    setAvaliableDevicesStatus({commit}, val) {
        commit('updateAvaliableDevices', val);
    },
    setAllowedDevicesStatus({commit}, val) {
        commit('updateAllowedDevices', val);
    },
    updateStudyRoomProps({dispatch,commit}, val) {
        console.warn('DEBUG: 7 store: updateStudyRoomProps')
        //update leaveReview store, to prevent leaving of multiple reviews
        dispatch('updateAllowReview',  val.allowReview);
        commit('setStudyRoomProps', val);
        if(!val.isTutor && val.needPayment){
            setTimeout(()=>{
                console.warn('DEBUG: 7.1 store: !val.isTutor && val.needPayment')

                videoStreamService.enterRoom();
            }, 500);
        }
    },
    updateTestDialogState({commit}, val) {
        commit('setqualityDialogState', val);
    },
    updateLocalParticipant({commit}, val) {
        commit('setlocalParticipantObj', val);
    },
    updateLocalParticipantsNetworkQuality({commit}, val) {
        commit('setNetworkQuality', val);
    },
    updateCodeLoadedOnce({commit}, val) {
        commit('changeFirepadLoaded', val);
    },
    updateRoomLoading({commit}, val) {
        console.warn('DEBUG: 8 store: updateRoomLoading')

        commit('setRoomLoading', val);
    },
    leaveRoomIfJoined({commit}) {
        console.warn('DEBUG: 9 store: leaveRoomIfJoined')

        commit('leaveIfJoinedRoom');
    },
    updateRoomInstance({commit}, data) {
        commit('setRoomInstance', data);
    },
    updateLocalStatus({commit}, val) {
        console.warn('DEBUG: 10 store: updateLocalStatus')
        commit('setLocalStatus', val);
    },
    updateRemoteStatus({commit}, val) {
        console.warn('DEBUG: 11 store: updateRemoteStatus, VAL:',val)

        commit('setRemoteStatus', val);
    },
    updateRoomStatus({commit}, val) {
        console.warn('DEBUG: 12 store: updateRoomStatus')
        commit('setRoomStatus', val);
    },
    updateUserIdentity({commit}, val) {
        commit('setUserIdentity', val);
    },
    updateCurrentRoomState({commit, state, dispatch}, val) {
        console.warn('DEBUG: 13 store: updateCurrentRoomState')

        commit('setCurrentRoomState', val);
        if(state.roomStateEnum[val] === state.roomStateEnum['active']){
            
            setTimeout(()=>{
                console.warn('DEBUG: 13.1 store: state.roomStateEnum[val] === state.roomStateEnum[1active1]')
                dispatch('hideRoomToasterMessage');
            }, 3000);
        }
    },
    updateStudentStartDialog({commit}, val) {
        console.warn('DEBUG: 14 store: updateStudentStartDialog')

        commit('setStudentStartDialog', val);
    },
    updateTutorStartDialog({commit}, val) {
        console.warn('DEBUG: 15 store: updateTutorStartDialog, VAL:',val)

        commit('setTutorStartDialog', val);
    },
    signalR_UpdateState({commit, dispatch, state, getters}, notificationObj) {
        console.warn('DEBUG: 16 store: signalR_UpdateState')

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
        console.warn('DEBUG: 16.1 store: if(jwtToken)')
            
            commit('setJwtToken', jwtToken);
            dispatch('updateCurrentRoomState', state.roomStateEnum.active);
            videoStreamService.createVideoSession();
            if(isTutor){
                console.warn('DEBUG: 16.2 store: if(isTutor)')

                toasterParams.text = LanguageService.getValueByKey('studyRoom_student_entered_room');
                dispatch('showRoomToasterMessage', toasterParams);
            }else{
                console.warn('DEBUG: 16.3 store: if(!isTutor)')

                toasterParams.text = LanguageService.getValueByKey('studyRoom_tutor_entered_room');
                dispatch('showRoomToasterMessage', toasterParams);
            }
        }else{
            console.warn('DEBUG: 17 store: if(!jwtToken)')

            if(isTutor) {
                console.warn('DEBUG: 17.1 store: if(isTutor)')

                if(onlineCount == totalOnline) {
                    console.warn('DEBUG: 17.2 store: onlineCount == totalOnline')

                    dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                    toasterParams.text = LanguageService.getValueByKey('studyRoom_student_entered_room');
                    dispatch('showRoomToasterMessage', toasterParams);
                    //show tutor start session
                    if(!state.studyRoomData.needPayment){
                        console.warn('DEBUG: 17.3 store: !state.studyRoomData.needPayment')

                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.start);
                        // dispatch("updateTutorStartDialog", true);
                    }else{
                        console.warn('DEBUG: 17.4 store: state.studyRoomData.needPayment')

                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.needPayment);
                    }
                } else {
                    console.warn('DEBUG: 17.5 store: onlineCount !== totalOnline')

                    if(state.currentRoomState !== state.roomStateEnum.active){
                        console.warn('DEBUG: 17.6 store: state.currentRoomState !== state.roomStateEnum.active')

                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.waiting);
                    }else{
                        console.warn('DEBUG: 17.7 store: state.currentRoomState === state.roomStateEnum.active')

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
                console.warn('DEBUG: 17.8 store: if(!isTutor)')
            // if is STUDENT
            if(onlineCount == totalOnline) {
                console.warn('DEBUG: 17.9 store: onlineCount == totalOnline')

                // toasterParams.text = LanguageService.getValueByKey('studyRoom_tutor_entered_room');
                // dispatch('showRoomToasterMessage', toasterParams);
                if(!state.studyRoomData.needPayment){
                    console.warn('DEBUG: 17.9.1 store: !state.studyRoomData.needPayment')

                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                }else{
                    console.warn('DEBUG: 17.9.2 store: state.studyRoomData.needPayment')

                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.needPayment);
                }
                toasterParams.text = LanguageService.getValueByKey('studyRoom_waiting_for_tutor_toaster');
                toasterParams.timeout = 3600000;
                dispatch('showRoomToasterMessage', toasterParams);
            } else {
                console.warn('DEBUG: 17.9.2 store: onlineCount !== totalOnline')

                if(!state.studyRoomData.needPayment){
                    console.warn('DEBUG: 17.9.3 store: !state.studyRoomData.needPayment')

                    console.log(state.currentRoomState);
                    if(state.currentRoomState === state.roomStateEnum.pending){
                        console.warn('DEBUG: 17.9.4 store: state.currentRoomState === state.roomStateEnum.pending')

                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                    }else{
                        console.warn('DEBUG: 17.9.5 store: state.currentRoomState !== state.roomStateEnum.pending')

                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.disconnected);
                    }
                }else{
                    console.warn('DEBUG: 17.9.6 store: state.studyRoomData.needPayment')

                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.needPayment);
                }
                if(!getters.getReviewDialogState){
                    console.warn('DEBUG: 17.9.7 store: !getters.getReviewDialogState')

                    dispatch('updateStudentStartDialog', true);
                }
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
    showRoomToasterMessage() {
        return;
    },
    hideRoomToasterMessage({dispatch}) {
        let toasterObj = {
            showToaster: false,
        };
        dispatch('updateToasterParams', toasterObj);
    },
    signalR_SetJwtToken({commit, dispatch, state}, sessionInformation) {
        console.warn('DEBUG: 18 store: signalR_SetJwtToken')

        let token = sessionInformation.data.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        commit('setJwtToken', token);
        if(!isTutor) {
            console.warn('DEBUG: 18.1 store: if(!isTutor)')

            //show student start se3ssion
            // SPITBALL-1197 Tutoring - Session stuck on start (fix)
            setTimeout(()=>{
                dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                dispatch("setStudentDialogState", state.startSessionDialogStateEnum.start);
                // dispatch("updateStudentStartDialog", true);
                dispatch('hideRoomToasterMessage');
            }, 3000);
        }else{
            console.warn('DEBUG: 18.2 store: if(isTutor)')

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
        console.warn('DEBUG: 19 store: setTutorDialogState, VAL:',val)
        commit('setTutorDialogState', val);
    },
    setStudentDialogState({commit}, val){
        console.warn('DEBUG: 20 store: setStudentDialogState')

        commit('setStudentDialogState', val);
    },
    setSessionTimeStart({commit}){
        commit('setSessionTimeStart', Date.now());
    },
    setSessionTimeEnd({commit}){
        commit('setSessionTimeEnd', Date.now());
    },
    setShowUserConsentDialog({commit}, val){
        console.warn('DEBUG: 21 store: setShowUserConsentDialog')

        commit('setShowUserConsentDialog', val);
    },
    setSnapshotDialog({commit}, val){
        commit('setSnapshotDialog', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
};
