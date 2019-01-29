import Vue from "vue";
import Vuex from "vuex";

import devStore from './devStore'
import userMain from './userMain';
Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        devStore,
        userMain
    }
});

export default store;