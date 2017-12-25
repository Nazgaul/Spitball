import Vue from "vue";
import Vuex from "vuex";
import Search from "./search";
import User from "./User";
import createPersistedState from "vuex-persistedstate"

const plugins = [];
Vue.use(Vuex);
const store = new Vuex.Store({
    modules: { Search, User },
    plugins: [
       // createPersistedState({ paths: ["User"] })
    ]
});
export default store;
