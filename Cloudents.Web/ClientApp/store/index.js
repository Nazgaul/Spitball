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
const storageFallback = [{
    provider: localStorage,
    enable: true,
},
{
    provider: sessionStorage,
    enable: true,
}]
for (let v in storageFallback) {
    let item = storageFallback[v];
    item.enable = canWriteStorage(item.provider);
}

let storage = storageFallback.find(f => f.enable);
const plugins = [];
if (storage) {
    plugins.push(createPersistedState(
        {
            paths: ["User"],
            storage: storage.provider
        }
    ));

}

Vue.use(Vuex);
const store = new Vuex.Store({
    modules: { Search, User },
    plugins: plugins
});
export default store;
