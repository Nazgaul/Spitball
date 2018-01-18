import Vuex from 'vuex'
import { shallow, createLocalVue } from 'vue-test-utils'
import luisData from 'ClientApp/store/luisData.js'
const localVue = createLocalVue();

localVue.use(Vuex);

describe('luisData.vue', () => {
    let actions;
    let store;

    beforeEach(() => {
        actions = {
            updateTerm: jest.fn()
        };
        store = new Vuex.Store({
            state: {},
            actions
        })
    });
}