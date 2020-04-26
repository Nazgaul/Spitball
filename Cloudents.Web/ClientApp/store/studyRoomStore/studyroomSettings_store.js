import studyRoomService from '../../services/studyRoomService.js'
const state = {
    visitedSettingPage: false,
    roomIsTutorSettings:false,
};

const getters = {
    // getVisitedSettingPage:state=> state.visitedSettingPage,
    getRoomIsTutorSettings:state=> state.roomIsTutorSettings,
};

const mutations = {
    setVisitedSettingPage(state, val){
        state.visitedSettingPage = val;
    },
    studySettings_room_props(state,props){
        state.roomIsTutorSettings = this.getters.accountUser.id == props.tutorId;
    }
};

const actions = {
    setVisitedSettingPage({commit}, val){
        commit('setVisitedSettingPage', val);
    },
    updateStudyRoomInformationForSettings({ commit }, roomId){
        studyRoomService.getRoomInformation(roomId).then((roomProps) => {
            commit('studySettings_room_props',roomProps)
        })
    },
 
};

export default {
    state,
    mutations,
    getters,
    actions
}