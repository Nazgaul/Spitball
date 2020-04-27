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
//     sessionStartClickedOnce: false,
//     currentRoomState: "pending",
//     sessionTimeStart: null,
//     sessionTimeEnd: null,
};
const getters = {
//     activeRoom: state => state.currentActiveRoom,
//     : state => state.currentRoomState,
//     getStudyRoomData: state => state.studyRoomData,
//     : state => state.sessionStartClickedOnce,
//     : state => state.sessionTimeStart,
//     : state => state.sessionTimeEnd,
};

const mutations = {
//     updateSessionClickedOnce(state, val) {
//         state.sessionStartClickedOnce = val;
//     },
//     setRoomInstance(state, data) {
//         state.currentActiveRoom = data;
//     },
//     setCurrentRoomState(state, val) {
//         if(!!state.roomStateEnum[val]) {
//             state.currentRoomState = val;
//         }
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
//     },
//     updateRoomInstance({commit}, data) {
//         commit('setRoomInstance', data);
//     },
//     updateCurrentRoomState({commit}, val) {
//         console.warn('DEBUG: 13 store: updateCurrentRoomState')
//         commit('setCurrentRoomState', val);
//     },



//     signalR_UpdateState({commit, dispatch, state, getters}, notificationObj) {
//         //TODO Update state according to the singnalR data
//         let onlineCount = notificationObj.onlineCount;

//         let totalOnline = notificationObj.totalOnline;
//         let isTutor = state.studyRoomData.isTutor;
            
//             dispatch('updateCurrentRoomState', state.roomStateEnum.active);
//             videoStreamService.createVideoSession();
//         }else{

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
//                     dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
//                 }
//             } else {
//                 console.warn('DEBUG: 17.8 store: if(!isTutor)')
//             // if is STUDENT
//             if(onlineCount == totalOnline) {
//                 console.warn('DEBUG: 17.9 store: onlineCount == totalOnline')
//                 if(!state.studyRoomData.needPayment){
//                 }
//             } else {
//                 console.warn('DEBUG: 17.9.2 store: onlineCount !== totalOnline')

//                 if(!state.studyRoomData.needPayment){
//                     console.warn('DEBUG: 17.9.3 store: !state.studyRoomData.needPayment')
//                     if(state.currentRoomState === state.roomStateEnum.pending){
//                     }else{
//                     }
//                 }
//                 dispatch("updateCurrentRoomState", state.roomStateEnum.pending);
//             }
//         }
//         }
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
