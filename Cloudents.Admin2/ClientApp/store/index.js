import vue from "vue";
import vuex from "vuex";

import userMain from './userMain';
vue.use(vuex);
const store = new vuex.Store({
    modules: {
        userMain
    }
});

export default store;