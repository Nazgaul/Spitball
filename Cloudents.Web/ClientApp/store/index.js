import Vue from "vue";
import Vuex from "vuex";
import Flow from "./flow";
import Search from "./search";
import User from "./User";
import createPersistedState from "vuex-persistedstate"

Vue.use(Vuex);
export default new Vuex.Store({
    modules: { Flow, Search, User },
    plugins: [
        createPersistedState({ paths: ["User"] })
    ]
});
