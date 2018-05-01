import store from './luisData'
import Vue from 'vue'
import Vuex from 'vuex'
import {LUIS} from "./mutation-types";
let aiGeneralTypes=['ask','note','tutor','book','flashcard'];
describe('luis term store', function () {
    test('defaults',()=>{
        expect(store.state.ask).toEqual(store.state.flashcard)
    });
    describe('mutations', function () {
        test('update term academic',()=>{
            let state={luis:{ask:""}};
            aiGeneralTypes.forEach(value => {
                let valueText=`${value}Text`;
                let term={term:[value],text:valueText};
                store.mutations[LUIS.UPDATE_TERM](state,{vertical:value,data:term});
                expect(state.academic).toEqual(term);
                expect(state.currentVertical).toEqual(value);
                expect(state.currentText).toEqual(valueText)
            });

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'job',data:{term:["yifat"],text:"job"}});
            expect(state.academic.text).toEqual(aiGeneralTypes[aiGeneralTypes.length-1]+'Text');
            expect(state.currentText).toEqual('job');

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'food',data:{term:["yifat"],text:"food"}});
            expect(state.academic.text).toEqual(aiGeneralTypes[aiGeneralTypes.length-1]+'Text');
            expect(state.currentText).toEqual('food');

        });

        test('clean data', ()=> {
            let state={};
            Object.values = (obj) => Object.keys(obj).map(key => obj[key]);
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'job',data:{term:["yifat"],text:"job"}});
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'food',data:{term:["yifat"],text:"food"}});
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'ask',data:{text:'ask'}});
            expect(Object.values(state).filter(v=>v!=="").length).toBeTruthy();
            store.mutations[LUIS.CLEAN_DATA](state);
            expect(Object.values(state).filter(v=>v!=="").length).toBeFalsy();
        });

        test('update current vertical', ()=> {
            let state={};
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'ask',data:{text:'ask'}});
            expect(state.currentVertical).toEqual('ask');
            expect(state.currentText).toEqual('ask');
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'job',data:{text:'job'}});
            expect(state.currentVertical).toEqual('job');
            expect(state.currentText).toEqual('job');
            store.mutations[LUIS.UPDATE_CURRENT_VERTICAL](state,'ask');
            expect(state.currentVertical).toEqual('ask');
            expect(state.currentText).toEqual('ask');
            store.mutations[LUIS.UPDATE_CURRENT_VERTICAL](state,'food');
            expect(state.currentVertical).toEqual('food');
            expect(state.currentText).toEqual('');
        });
        test('update filter courses', ()=> {
            let state={};
            store.mutations[LUIS.UPDATE_FILTER_COURSES](state,[1234]);
            expect(state.filterCourses.length).toBeTruthy();

        });
    });
    describe('actions', function () {
        let $store;

        beforeAll(()=>{
            Vue.use(Vuex);
            store.actions.updateHistorySetVertical=jest.fn();
            $store=new Vuex.Store(store)
        });

        test('clean data', ()=> {
            let term={term:["yifat"],text:"yifat"};
            $store.commit(LUIS.UPDATE_TERM,{vertical:'ask',data:term});
            expect($store.getters.currentText).toEqual(term.text);
            $store.dispatch("cleanData")
            expect($store.state.currentText).toBe('');
        });
        test('set currentVertical', ()=> {
            let term={term:["yifat"],text:"yifat"};
            $store.commit(LUIS.UPDATE_TERM,{vertical:'ask',data:term});
            expect($store.state.currentVertical).toEqual('ask');
            $store.dispatch("setCurrentVertical",'flashcard');
            expect($store.state.currentVertical).toBe('flashcard');
            $store.dispatch("setCurrentVertical",'gyuj');
            expect($store.state.currentVertical).toBe('gyuj');
        });
        describe('update term', function () {
            test('update general', ()=> {
                let term={term:["yifat"],text:"yifat"};
                $store.dispatch('updateAITerm',{vertical:'ask',data:term});
                expect($store.state.academic.text).toBe('yifat');
                expect(store.actions.updateHistorySetVertical).toHaveBeenCalledTimes(1);
                expect(store.actions.updateHistorySetVertical.mock.calls[0][1]).toEqual({term:'yifat',vertical:'ask'});
                $store.dispatch('updateAITerm',{vertical:'flashcard',data:{...term,text:'flashcard'}});
                expect($store.state.academic.text).toBe('flashcard')
            });
        });
        describe('get AI data for vertical', function () {
            let general={text:"general",term:["general"]};
            let food={text:"food",term:["food"]};
            let job={text:"job",term:["job"]};
            let course_verticals=['note','flashcard'];
            beforeAll(()=>{
                $store.commit(LUIS.UPDATE_TERM,{vertical:"ask",data:general});
                $store.commit(LUIS.UPDATE_TERM,{vertical:"job",data:job});
                $store.commit(LUIS.UPDATE_TERM,{vertical:"food",data:food});
            });
            // test('getLuis general', ()=> {
            //     aiGeneralTypes.forEach(async (term)=>{
            //         let val= await $store.dispatch("getAIDataForVertical",term);
            //         course_verticals.includes(term)
            //         expect(val).toEqual(course_verticals.includes(term)?{...general,course:""}:general);
            //     });
            // });
            // test('getLuis job and food',async ()=> {
            //     let val= await $store.dispatch("getAIDataForVertical",'food');
            //     expect(val).toEqual(food);
            //     val= await $store.dispatch("getAIDataForVertical",'job');
            //     expect(val).toEqual(job);
            // });
        });
        test('set filteredClasses', ()=> {
            let classes=[12345];
            expect($store.state.filterCourses.length).toBeFalsy();
            $store.dispatch("setFilteredCourses",classes);
            expect($store.state.filterCourses.length).toBeTruthy();
        });
    });
    describe('getters', function () {
        let $store;
        let general={text:"general",term:["general"]};
        let food={text:"food",term:["food"]};
        let job={text:"job",term:["job"]};
        beforeAll(()=>{
            Vue.use(Vuex);
            $store=new Vuex.Store(store);
        });
        describe('get luis term', function () {

            test('getAI general', ()=> {
                aiGeneralTypes.forEach((val)=>{
                    $store.commit(LUIS.UPDATE_TERM,{vertical:val,data:general});
                    let value= $store.getters.getVerticalData(val);
                    expect(value).toEqual(general);
                    expect($store.state.currentVertical).toEqual(val);
                });
            });
            test('getLuis job and food',()=> {
                $store.commit(LUIS.UPDATE_TERM,{vertical:"food",data:food});
                let value= $store.getters.getVerticalData("food");
                expect(value).toEqual(food);
                $store.commit(LUIS.UPDATE_TERM,{vertical:"job",data:job});
                expect($store.getters.getVerticalData("job")).toEqual(job);
            });
        });
    });
});