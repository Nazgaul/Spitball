import Vue from 'vue';
import Vuex from 'vuex';
import Flow from './flow';
import CurrentPage from './currentPage';
import createPersistedState from 'vuex-persistedstate'

Vue.use(Vuex)
export default new Vuex.Store({
    modules: { Flow, CurrentPage },
    //issue in saving Flow need to save specific module
    plugins: [createPersistedState({ paths: ['CurrentPage']})]
});
