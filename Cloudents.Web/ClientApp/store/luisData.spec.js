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
        test('update term academic' +
            '',()=>{
            let term={term:["yifat"],text:"yifat"};
            let state={luis:{ask:term}};
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'ask',data:{term:["yifat"],text:"yifat"}});
            expect(state.academic).toEqual(term);
            expect(state.currentText).toEqual(term.text);
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'flashcard',data:{term:["yifat"],text:"flashcard"}});
            expect(state.academic.text).toEqual('flashcard');
            expect(state.currentText).toEqual('flashcard');
            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'note',data:{term:["yifat"],text:"note"}});
            expect(state.academic.text).toEqual('note');
            expect(state.currentText).toEqual('note');

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'book',data:{term:["yifat"],text:"book"}});
            expect(state.academic.text).toEqual('book');
            expect(state.currentText).toEqual('book');

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'note',data:{term:["yifat"],text:"tutor"}});
            expect(state.academic.text).toEqual('tutor');
            expect(state.currentText).toEqual('tutor');

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'job',data:{term:["yifat"],text:"job"}});
            expect(state.academic.text).toEqual('tutor');
            expect(state.currentText).toEqual('job');

            store.mutations[LUIS.UPDATE_TERM](state,{vertical:'food',data:{term:["yifat"],text:"food"}});
            expect(state.academic.text).toEqual('tutor');
            expect(state.currentText).toEqual('food');

        })
    });
    describe('actions', function () {
        let $store;

        beforeAll(()=>{
            Vue.use(Vuex);
            $store=new Vuex.Store(store)
        });

        describe('update term', function () {
            test('update general', ()=> {
                let term={term:["yifat"],text:"yifat"};
              $store.dispatch('updateTerm',{vertical:'ask',data:term});
                console.log(JSON.stringify($store.state.academic));
                expect($store.state.academic.text).toBe('yifat');
                $store.dispatch('updateTerm',{vertical:'flashcard',data:{...term,text:'flashcard'}});
                expect($store.state.academic.text).toBe('flashcard')
            });
        });
        describe('get luis term', function () {
            let general={text:"general",term:["general"]};
            let food={text:"food",term:["food"]};
            let job={text:"job",term:["job"]};
            beforeAll(()=>{
                $store.commit(LUIS.UPDATE_TERM,{vertical:"ask",data:general});
                $store.commit(LUIS.UPDATE_TERM,{vertical:"job",data:job});
                $store.commit(LUIS.UPDATE_TERM,{vertical:"food",data:food})
            });
            test('getLuis general', ()=> {
                aiGeneralTypes.forEach(async (term)=>{
                    let val= await $store.dispatch("getAIData",term);
                    expect(val).toEqual(general);
                });
            });
            test('getLuis job and food',async ()=> {
                    let val= await $store.dispatch("getAIData",'food');
                    expect(val).toEqual(food);
                    val= await $store.dispatch("getAIData",'job');
                    expect(val).toEqual(job);
            });
        });
    });
});