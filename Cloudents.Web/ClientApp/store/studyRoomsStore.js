import studyRoomsService from '../services/studyRoomsService'

const state = {
    studyRooms:[],
 };
 
 const getters = {
     getStudyRooms:  state => state.studyRooms,
 };
 
 const mutations = {
     setStudyRooms(state, val) {
         state.studyRooms = val
     },
 };
 
 const actions = {
    fetchStudyRooms({commit}){
        studyRoomsService.getRooms().then((rooms)=>{
            commit('setStudyRooms', rooms);
        })
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