import Vue from "vue";
import Vuex from "vuex";

import devStore from './devStore'

Vue.use(Vuex);
const store = new Vuex.Store({
    modules: {
        devStore
    }
});

export default store;