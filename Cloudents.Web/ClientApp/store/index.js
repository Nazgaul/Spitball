import Vue from 'vue';
import Vuex from 'vuex';
import state from './flow';
import page from './page';

Vue.use(Vuex)
export default new Vuex.Store({
    modules: { state,page }
});