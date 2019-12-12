import dashboardService from '../services/dashboardService.js';

const state = {
   salesItems: [],
   dashboardDialog: false,
}

const mutations = {
   setSalesItems(state,val) {
      state.salesItems = val;
   },
   setDashboardDialog(state, val) {
      state.dashboardDialog = val;
   }
}

const getters = {
   getSalesItems: state => state.salesItems,
   getShowDashboardDialog: state => state.dashboardDialog,
}

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      })
   },
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