﻿import {LUIS} from "./mutation-types"
let luisTypes=["job","food"];
let ACADEMIC="academic";
const state = {
    job:{},
    food:{},
    academic:{},
    currentText:"",
    currentVertical:"",
    filterCourses:[]
};
let getLuisType=(type)=>luisTypes.includes(type)?type:ACADEMIC;
const mutations = {
    [LUIS.UPDATE_TERM](state, {vertical,data}) {
        state[getLuisType(vertical)]=data;
        state.currentVertical=vertical;
        state.currentText=data.text;
    },
    [LUIS.UPDATE_FILTER_COURSES](state, data) {
        state.filterCourses=data;
    },
    [LUIS.UPDATE_CURRENT_VERTICAL](state,data){
        state.currentVertical=data;
        state.currentText=state[getLuisType(data)]?state[getLuisType(data)].text:""
    },
    [LUIS.CLEAN_DATA](state){
        for (const prop of Object.keys(state)) {
            state[prop]="";
        }
    }
};

const getters = {
    askData:state=>state.academic||{},
    noteData:state=>({...state.academic,course:state.filterCourses}||{}),
    flashcardData:state=>({...state.academic,course:state.filterCourses}||{}),
    tutorData:state=>state.academic||{},
    bookData:state=>state.academic||{},
    jobData:state=>state.job||{},
    foodData:state=>state.food||{},
    currentText:state=>state.currentText,
    getAIData:state=>state[getLuisType(state.currentVertical)]
};
const actions = {
    updateAITerm({commit}, {vertical,data}){
        commit(LUIS.UPDATE_TERM,{vertical,data})
    },
    getAIDataForVertical(context, val){
      return context.getters[`${val}Data`];
    },
    setFileredCourses({commit}, data){
        commit(LUIS.UPDATE_FILTER_COURSES,data);
    },
    setCurrentVertical({commit},data){
        commit(LUIS.UPDATE_CURRENT_VERTICAL,data)
    },
    cleanData({commit}){
        commit(LUIS.CLEAN_DATA);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
