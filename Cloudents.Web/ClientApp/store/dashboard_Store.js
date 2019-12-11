import dashboardService from '../services/dashboardService.js';

const state = {
   salesItems: [],

}

const mutations = {
   setSalesItems(state,val){
      state.salesItems = val
   }
   
}

const getters = {
   getSalesItems: state => state.salesItems,
   
}

const actions = {
   updateSalesItems({commit}){
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      })
   }
   
}

export default {
   state,
   mutations,
   getters,
   actions
}