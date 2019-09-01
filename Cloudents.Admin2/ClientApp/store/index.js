import Vue from "vue";
import Vuex from "vuex";

import userMain from './userMain';
Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        userMain
    }
});

export default store;