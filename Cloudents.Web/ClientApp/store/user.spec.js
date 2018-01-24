import store from './User'
import Vue from 'vue'
import Vuex from 'vuex'
import { USER} from "./mutation-types";
describe('user store', function () {
    describe('mutations', function () {
        test('facet', ()=> {
            let state={};
            let facetList=["full time", "part time", "contractor", "internship", "campus rep"];
            store.mutations[USER.UPDATE_FACET](state,facetList);
            expect(state.facet.length).toBeTruthy();
            expect(state.facet).toBe(facetList);
        });
    });
});