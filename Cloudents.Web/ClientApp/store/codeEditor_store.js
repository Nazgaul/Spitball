const state = {
    isDarkTheme: true,
};
const getters = {
    getIsDarkTheme:state => state.isDarkTheme,
};

const mutations = {
    setThemeMode(state,val){
        state.isDarkTheme = val
    }
};

const actions = {
    updateThemeMode({commit},val){
        commit('setThemeMode',val)
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}