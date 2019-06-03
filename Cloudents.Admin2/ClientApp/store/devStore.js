const state = {
    isDev: false
};
const mutations = {
    devStore_setIsDevState(state, data) {
        state.isDev = data;
    }
};
const getters = {
    devStore_getIsDev: (state) => state.isDev

};
const actions = {
    devStore_updateIsDev({commit}, data) {
        if(MD5(data) === 'c583f119d2d547eaac531e64bba7e430'){
            commit('devStore_setIsDevState', true);
        }
    }
};
export default {
    state, mutations, getters, actions
};