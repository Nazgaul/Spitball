import Vue from 'vue';
import Vuex from 'vuex';
import Flow from './flow';
import CurrentPage from './currentPage';
import User from './User';
import createPersistedState from 'vuex-persistedstate'

Vue.use(Vuex)
export default new Vuex.Store({
    modules: { Flow, CurrentPage,User },
    plugins: [createPersistedState({ storage: window.sessionStorage, paths: ['CurrentPage'] }),
        createPersistedState({ paths: ['User'] })
    ]
});
