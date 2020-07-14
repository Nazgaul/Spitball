import { TOASTER } from './mutation-types'
import * as componentConsts from '../components/pages/global/toasterInjection/componentConsts.js'

const state = {
    toasterTypes:{
        default: '',
    },
    params: {
        toasterText: '',
        showToaster:false,
        toasterType: '', //class name
        toasterTimeout: 5000
    },
    component: []
};
const mutations = {
    //OLD CODE IGNORE!!!!
    [TOASTER.UPDATE_PARAMS](state,val) {
        if(!val.hasOwnProperty('toasterTimeout')){
            val.toasterTimeout = 5000;
        }else if(val.toasterTimeout === undefined){
            val.toasterTimeout = 5000;
        }
        
        state.params={...state.params,...val};
        if(!val.toasterType){
            state.params.toasterType = '';
        }
    },
    setComponent(state, component) {
        if(component){
            state.component = [component]
        }else{
            state.component = [];
        }
    },
    clearComponent(state) {
        state.component = [];
    },
    addComponent(state,componentName){
        // should be outside but for now its here...
        if(componentName == componentConsts.PAYMENT_DIALOG && this.getters.isFrymo){
            return // only in studyroom cuz we dont have frymo payment
        }
        if(state.component.includes(componentName)){
            // 
        }else{
            state.component.push(componentName) 
        }
    },
    removeComponent(state,componentName){
        const index = state.component.indexOf(componentName);
        if (index > -1) {
            state.component.splice(index, 1);
        }
    }
};
const getters = {
    getShowToaster:  state => state.params.showToaster,
    getToasterText: state => state.params.toasterText,
    getShowToasterType: state => state.params.toasterType,
    getToasterTimeout: state => state.params.toasterTimeout,
    getComponent: state => state.component,
    getIsComponentActiveByName: (state) => (componentName) => state.component.includes(componentName),
};
const actions = {
    updateToasterParams({commit}, val){
        commit(TOASTER.UPDATE_PARAMS, val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}