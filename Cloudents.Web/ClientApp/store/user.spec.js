import store from './User'
import Vue from 'vue'
import Vuex from 'vuex'
import { USER} from "./mutation-types";
import * as consts from './constants';
describe('user store', function () {
    describe('mutations', function () {
        test('facet', ()=> {
            let state={};
            let facetList=["full time", "part time", "contractor", "internship", "campus rep"];
            store.mutations[USER.UPDATE_FACET](state,facetList);
            expect(state.facet.length).toBeTruthy();
            expect(state.facet).toBe(facetList);
        });
        describe('history set', function () {
            test('add term', ()=> {
                let state={};
                store.mutations[USER.UPDATE_SEARCH_SET](state,'yifat');
                expect(state.historyTermSet.length).toBeTruthy();
                expect(state.historyTermSet).toEqual(['yifat']);
            });
            test('add exist term', ()=> {
                let state={historyTermSet:["yifat"]};
                store.mutations[USER.UPDATE_SEARCH_SET](state,'yifat');
                expect(state.historyTermSet).toEqual(['yifat']);
            });
            test('add exist term in middle of list', ()=> {
                let state={historyTermSet:["yifat","RAM","IRENA"]};
                store.mutations[USER.UPDATE_SEARCH_SET](state,'RAM');
                expect(state.historyTermSet).toEqual(["yifat","IRENA","RAM"]);
            });
            test('add empty term', ()=> {
                let state={historyTermSet:["yifat","RAM","IRENA"]};
                store.mutations[USER.UPDATE_SEARCH_SET](state,'');
                expect(state.historyTermSet).toEqual(["yifat","RAM","IRENA"]);
                store.mutations[USER.UPDATE_SEARCH_SET](state,' ');
                expect(state.historyTermSet).toEqual(["yifat","RAM","IRENA"]);
            });
            test('add more than the limit', ()=> {
                let state={historyTermSet:[]};
                let arr=new Array(consts.MAX_HISTORY_LENGTH).fill();
                for(let i in arr){
                    store.mutations[USER.UPDATE_SEARCH_SET](state,`RAM${i}`);
                }
                expect(state.historyTermSet[0]).toBe('RAM0');
                store.mutations[USER.UPDATE_SEARCH_SET](state,`RAM${consts.MAX_HISTORY_LENGTH+1}`);
                expect(state.historyTermSet[0]).toBe('RAM1');
                expect(new Set(state.historyTermSet).has('RAM0')).toBeFalsy();
            });
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