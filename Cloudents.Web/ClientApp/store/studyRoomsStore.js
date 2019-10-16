import studyRoomsService from '../services/studyRoomsService'

const state = {
    studyRooms:[]
 };
 
 const getters = {
     getStudyRooms:  state => state.studyRooms,
 };
 
 const mutations = {
     setStudyRooms(state, val) {
         state.studyRooms = val;
     },
 };
 
 const actions = {
    fetchStudyRooms({commit, dispatch}){
        studyRoomsService.getRooms().then((rooms)=>{
            rooms.forEach(room=>{
                let userStatus = {
                    id: room.userId,
                    online: room.online
                };
                dispatch('setUserStatus', userStatus);
            });
            commit('setStudyRooms', rooms);
        });
    },
     createStudyRoom({commit}, userId){
        return studyRoomsService.createRoom(userId);
     },
 };
 export default {
     state,
     mutations,
     getters,
     actions
 }