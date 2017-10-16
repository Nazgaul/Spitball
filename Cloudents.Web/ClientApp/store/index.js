import Vue from 'vue';
import Vuex from 'vuex';
import Flow from './flow';
import CurrentPage from './currentPage';
import createPersistedState from 'vuex-persistedstate'

Vue.use(Vuex)
export default new Vuex.Store({
    modules: { Flow, CurrentPage },
    plugins: [createPersistedState({ storage: window.sessionStorage,paths: ['CurrentPage']})]
});
