﻿import {LUIS} from "./mutation-types"
let luisTypes=["job","food"];
let ACADEMIC="academic";
const state = {
    job:{term:"",text:""},
    food:{term:"",text:""},
    academic:{},
    currentText:""
};

const mutations = {
    [LUIS.UPDATE_TERM](state, {vertical,data}) {
        state[luisTypes.includes(vertical)?vertical:ACADEMIC]=data;
        state.currentText=data.text;
    }
};

const getters = {
    askData:state=>state.academic,
    noteData:state=>state.academic,
    flashcardData:state=>state.academic,
    tutorData:state=>state.academic,
    bookData:state=>state.academic,
    jobData:state=>state.job,
    foodData:state=>state.food,
    currentName:state=>state.currentText
};
const actions = {
    updateTerm({commit},{vertical,data}){
        commit(LUIS.UPDATE_TERM,{vertical,data})
    },
    getAIData(context,val){
      return context.getters[`${val}Data`];
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
