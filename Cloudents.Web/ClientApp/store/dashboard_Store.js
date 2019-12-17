import dashboardService from '../services/dashboardService.js';

const state = {
   salesItems: [],
   // contentItems: [],
   dashboardDialog: false,
}

const mutations = {
   setSalesItems(state,val) {
      state.salesItems = val;
   },
   // setContentItems(state,val) {
   //    state.contentItems = val;
   // },
   setDashboardDialog(state, val) {
      state.dashboardDialog = val;
   }
}

const getters = {
   getSalesItems: state => state.salesItems,
   // getContentItems: state => state.contentItems,
   getShowDashboardDialog: state => state.dashboardDialog,
}

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      })
   },
   // updateContentItems({commit}){
   //    dashboardService.getContentItems().then(items=>{
   //       commit('setContentItems', items);
   //    }) 
   // },
   openDashboardDialog({commit}, val) {
      commit('setDashboardDialog', val);
   }
   
}

export default {
   state,
   mutations,
   getters,
   actions
}