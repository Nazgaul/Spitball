import userOnlineStatusService from './../services/userOnlineStatusService'

const state = {
    userStatus:{}
};

const mutations = {
    setUserStatus(state, userStatusObj){
        let id = userStatusObj.id;
        let isOnline = userStatusObj.online;
        state.userStatus = {...state.userStatus, [id]:isOnline};
    }
};

const getters = {
    getUserStatus: state => state.userStatus,
};

const actions = {
    setUserStatus({commit}, data){
        let userStatusObj = userOnlineStatusService.createUserStatus(data);
        commit('setUserStatus', userStatusObj);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
