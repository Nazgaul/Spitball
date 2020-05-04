import vue from "vue";
import vuex from "vuex";

import userMain from './userMain';
import injectStore from './injectStore';

vue.use(vuex);

const store = new vuex.Store({
    modules: {
        userMain,
        injectStore
    }
});

export default store;