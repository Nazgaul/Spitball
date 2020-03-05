import videoStreamService from '../../services/videoStreamService';
import tutorService from '../../components/studyroom/tutorService';


const state = {
    identity: '',
    roomId: '',
    currentActiveRoom: null,
    localParticipant: null,
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
        disconnected: 'disconnected',
        finished: 'finished'
    },
    tutorDialogState:'start',
    studentDialogState:'waiting',
    sessionStartClickedOnce: false,
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
    activeRoom: state => state.currentActiveRoom,
    localParticipant: state => state.localParticipant,
    userIdentity: state => state.identity,
    getCurrentRoomState: state => state.currentRoomState,
    getStudyRoomData: state => state.studyRoomData,
    getJwtToken: state => state.jwtToken,
    getRoomId: state => state.roomId,
    getStudentStartDialog: state => state.studentStartDialog,
    getTutorStartDialog: state => state.tutorStartDialog,
    getSessionStartClickedOnce: state => state.sessionStartClickedOnce,
    getEndDialog: state => state.endDialog,
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
    getIsRoomNeedPayment: state => {
        if (!state.studyRoomData) {
            return null;
        }
        if (state.studyRoomData.isTutor) {
            return false;
        }
        return state.studyRoomData.needPayment;
    }
};

const mutations = {
    setEndDialog(state, val) {
        state.endDialog = val;
    },
    updateSessionClickedOnce(state, val) {
        state.sessionStartClickedOnce = val;
    },
    setStudentStartDialog(state, val) {
        state.studentStartDialog = val;
    },
    setTutorStartDialog(state, val) {
        state.tutorStartDialog = val;
    },
    setStudyRoomProps(state, val) {
        val.isTutor = this.getters.accountUser.id == val.tutorId;
        state.studyRoomData = val;
    },
    setlocalParticipantObj(state, val) {
        state.localParticipant = val;
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
    updateStudyRoomProps({dispatch,commit}, val) {
        console.warn('DEBUG: 7 store: updateStudyRoomProps')
        dispatch('updateAllowReview',  val.allowReview);
        commit('setStudyRoomProps', val);
    },
    updateLocalParticipant({commit}, val) {
        commit('setlocalParticipantObj', val);
    },
    leaveRoomIfJoined({commit}) {
        console.warn('DEBUG: 9 store: leaveRoomIfJoined')

        commit('leaveIfJoinedRoom');
    },
    updateRoomInstance({commit}, data) {
        commit('setRoomInstance', data);
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

        let totalOnline = notificationObj.totalOnline;
        let jwtToken = notificationObj.jwtToken;
        let isTutor = state.studyRoomData.isTutor;
        if(jwtToken){
        console.warn('DEBUG: 16.1 store: if(jwtToken)')
            
            commit('setJwtToken', jwtToken);
            dispatch('updateCurrentRoomState', state.roomStateEnum.active);
            videoStreamService.createVideoSession();
        }else{
            console.warn('DEBUG: 17 store: if(!jwtToken)')

            if(isTutor) {
                console.warn('DEBUG: 17.1 store: if(isTutor)')

                if(onlineCount == totalOnline) {
                    console.warn('DEBUG: 17.2 store: onlineCount == totalOnline')
                    dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
                    //show tutor start session
                    if(!state.studyRoomData.needPayment){
                        console.warn('DEBUG: 17.3 store: !state.studyRoomData.needPayment')
                        dispatch("setTutorDialogState", state.startSessionDialogStateEnum.start);
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
                    dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
                }
            } else {
                console.warn('DEBUG: 17.8 store: if(!isTutor)')
            // if is STUDENT
            if(onlineCount == totalOnline) {
                console.warn('DEBUG: 17.9 store: onlineCount == totalOnline')
                if(!state.studyRoomData.needPayment){
                    console.warn('DEBUG: 17.9.1 store: !state.studyRoomData.needPayment')
                    dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                }
            } else {
                console.warn('DEBUG: 17.9.2 store: onlineCount !== totalOnline')

                if(!state.studyRoomData.needPayment){
                    console.warn('DEBUG: 17.9.3 store: !state.studyRoomData.needPayment')
                    if(state.currentRoomState === state.roomStateEnum.pending){
                        console.warn('DEBUG: 17.9.4 store: state.currentRoomState === state.roomStateEnum.pending')
                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
                    }else{
                        console.warn('DEBUG: 17.9.5 store: state.currentRoomState !== state.roomStateEnum.pending')
                        dispatch("setStudentDialogState", state.startSessionDialogStateEnum.disconnected);
                    }
                }
                if(!getters.getReviewDialogState){
                    console.warn('DEBUG: 17.9.7 store: !getters.getReviewDialogState')

                    dispatch('updateStudentStartDialog', true);
                }
                dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
            }
        }
        }
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
        }else{
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
