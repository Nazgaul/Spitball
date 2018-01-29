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
    describe('getters', function () {
        let $store;

        beforeAll(()=>{
            Vue.use(Vuex);
            $store=new Vuex.Store(store);
        });
        test('location as string and object', ()=> {
            let location={latitude:"31.919",longitude:"34.80"};
            let location2={latitude:"3",longitude:"-6"};
            let getterLocation=$store.getters.location;
            expect(typeof getterLocation).toEqual('object');
            $store.commit(USER.UPDATE_USER,{location:`${location.latitude},${location.longitude}`});
            getterLocation=$store.getters.location;
            expect(typeof getterLocation).toEqual('object');
            expect(getterLocation.latitude).toBe(location.latitude);
            expect(getterLocation.longitude).toBe(location.longitude);
            $store.commit(USER.UPDATE_USER,{location:location2});
            getterLocation=$store.getters.location;
            expect(typeof getterLocation).toEqual('object');
            expect(getterLocation.latitude).toBe(location2.latitude);
            expect(getterLocation.longitude).toBe(location2.longitude);
        });
    });
});