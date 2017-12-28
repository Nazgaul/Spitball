import Vue from "vue";
import Vuex from "vuex";
//This is new vuex
//import Search from "./session";
import Search from "./search";
import User from "./User";
//TODO: server side fix
import createPersistedState from "vuex-persistedstate"

const plugins = [];
Vue.use(Vuex);
const store = new Vuex.Store({
    modules: { Search, User },
    plugins: [
       //createPersistedState({ paths: ["User"] })
    ]
});
export default store;
