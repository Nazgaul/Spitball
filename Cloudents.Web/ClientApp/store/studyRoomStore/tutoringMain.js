// import videoStreamService from '../../services/videoStreamService';


const state = {
//     currentActiveRoom: null,
//     studyRoomData: null,
//     roomStateEnum: {
//         pending: "pending",
//         ready: "ready",
//         loading: "loading",
//         active: "active"
//     },
//     startSessionDialogStateEnum:{
//         start: 'start',
//         waiting: 'waiting',
//         disconnected: 'disconnected',
//         finished: 'finished'
//     },
//     sessionStartClickedOnce: false,
//     currentRoomState: "pending",
//     jwtToken: null,
//     sessionTimeStart: null,
//     sessionTimeEnd: null,
};
const getters = {
//     activeRoom: state => state.currentActiveRoom,
//     : state => state.currentRoomState,
//     getStudyRoomData: state => state.studyRoomData,
//     getJwtToken: state => state.jwtToken,
//     : state => state.sessionStartClickedOnce,
//     : state => state.sessionTimeStart,
//     : state => state.sessionTimeEnd,
};

const mutations = {
//     updateSessionClickedOnce(state, val) {
//         state.sessionStartClickedOnce = val;
//     },
//     leaveIfJoinedRoom(state) {
//         if(state.currentActiveRoom) {
//             state.currentActiveRoom.disconnect();
//         }
//     },
//     setRoomInstance(state, data) {
//         state.currentActiveRoom = data;
//     },
//     setCurrentRoomState(state, val) {
//         if(!!state.roomStateEnum[val]) {
//             state.currentRoomState = val;
//         }
//     },
//     setJwtToken(state, val) {
//         state.jwtToken = val;
//     },
//     setStudentDialogState(state, val){
//         state.studentDialogState = val;
//     },
//     setSessionTimeStart(state, val){
//         state.sessionTimeStart = val;
//     },
//     setSessionTimeEnd(state, val){
//         state.sessionTimeEnd = val;
//     },
};

const actions = {
//     setSesionClickedOnce({commit}, val) {
//         commit('updateSessionClickedOnce', val);
//     },
//     leaveRoomIfJoined({commit}) {
//         console.warn('DEBUG: 9 store: leaveRoomIfJoined')

//         commit('leaveIfJoinedRoom');
//     },
//     updateRoomInstance({commit}, data) {
//         commit('setRoomInstance', data);
//     },
//     updateCurrentRoomState({commit}, val) {
//         console.warn('DEBUG: 13 store: updateCurrentRoomState')
//         commit('setCurrentRoomState', val);
//     },
//     signalR_UpdateState({commit, dispatch, state, getters}, notificationObj) {
//         console.warn('DEBUG: 16 store: signalR_UpdateState')

//         //TODO Update state according to the singnalR data
//         let onlineCount = notificationObj.onlineCount;

//         let totalOnline = notificationObj.totalOnline;
//         let jwtToken = notificationObj.jwtToken;
//         let isTutor = state.studyRoomData.isTutor;
//         if(jwtToken){
//         console.warn('DEBUG: 16.1 store: if(jwtToken)')
            
//             commit('setJwtToken', jwtToken);
//             dispatch('updateCurrentRoomState', state.roomStateEnum.active);
//             videoStreamService.createVideoSession();
//         }else{
//             console.warn('DEBUG: 17 store: if(!jwtToken)')

//             if(isTutor) {
//                 console.warn('DEBUG: 17.1 store: if(isTutor)')

//                 if(onlineCount == totalOnline) {
//                     console.warn('DEBUG: 17.2 store: onlineCount == totalOnline')
//                     dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
//                     //show tutor start session
//                     if(!state.studyRoomData.needPayment){
//                     }
//                 } else {
//                     console.warn('DEBUG: 17.5 store: onlineCount !== totalOnline')

//                     if(state.currentRoomState !== state.roomStateEnum.active){
//                         console.warn('DEBUG: 17.6 store: state.currentRoomState !== state.roomStateEnum.active')

//                     }else{

//                     }
//                     dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
//                 }
//             } else {
//                 console.warn('DEBUG: 17.8 store: if(!isTutor)')
//             // if is STUDENT
//             if(onlineCount == totalOnline) {
//                 console.warn('DEBUG: 17.9 store: onlineCount == totalOnline')
//                 if(!state.studyRoomData.needPayment){
//                     console.warn('DEBUG: 17.9.1 store: !state.studyRoomData.needPayment')
//                     dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
//                 }
//             } else {
//                 console.warn('DEBUG: 17.9.2 store: onlineCount !== totalOnline')

//                 if(!state.studyRoomData.needPayment){
//                     console.warn('DEBUG: 17.9.3 store: !state.studyRoomData.needPayment')
//                     if(state.currentRoomState === state.roomStateEnum.pending){
//                         console.warn('DEBUG: 17.9.4 store: state.currentRoomState === state.roomStateEnum.pending')
//                         dispatch("setStudentDialogState", state.startSessionDialogStateEnum.waiting);
//                     }else{
//                         console.warn('DEBUG: 17.9.5 store: state.currentRoomState !== state.roomStateEnum.pending')
//                         dispatch("setStudentDialogState", state.startSessionDialogStateEnum.disconnected);
//                     }
//                 }
//                 dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
//             }
//         }
//         }
//     },
//     signalR_SetJwtToken({commit, dispatch, state}, sessionInformation) {
//         console.warn('DEBUG: 18 store: signalR_SetJwtToken')

//         let token = sessionInformation.data.jwtToken;
//         let isTutor = state.studyRoomData.isTutor;
//         commit('setJwtToken', token);
//         if(!isTutor) {
//             console.warn('DEBUG: 18.1 store: if(!isTutor)')

//             //show student start se3ssion
//             // SPITBALL-1197 Tutoring - Session stuck on start (fix)
//             setTimeout(()=>{
//                 dispatch("updateCurrentRoomState", state.roomStateEnum.ready);
//                 dispatch("setStudentDialogState", state.startSessionDialogStateEnum.start);
//             }, 3000);
//         }else{

            
//         }
//     },
//     setStudentDialogState({commit}, val){
//         commit('setStudentDialogState', val);
//     },
//     setSessionTimeStart({commit}){
//         commit('setSessionTimeStart', Date.now());
//     },
//     setSessionTimeEnd({commit}){
//         commit('setSessionTimeEnd', Date.now());
//     },
};
export default {
    state,
    mutations,
    getters,
    actions
};
