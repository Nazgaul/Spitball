import Vue from "vue";
import Vuex from "vuex";
//This is new vuex
//import Search from "./session";
import Search from "./search";
import User from "./User";
//TODO: server side fix
import createPersistedState from "vuex-persistedstate"

function canWriteStorage(storage) {
    try {
        storage.setItem('@@', 1);
        storage.removeItem('@@');
        return true;
    } catch (e) { }

    return false;
}


const plugins = [createPersistedState(
    {
        paths: ["User"],
        storage: canWriteStorage(localStorage) ? localStorage : sessionStorage
    }
)];

Vue.use(Vuex);
const store = new Vuex.Store({
    modules: { Search, User },
    plugins: plugins
});
export default store;
