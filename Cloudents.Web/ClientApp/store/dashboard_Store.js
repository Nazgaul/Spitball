import dashboardService from '../services/dashboardService.js';

const state = {
   salesItems: [],
   contentItems: [],
}

const mutations = {
   setSalesItems(state,val) {
      state.salesItems = val;
   },
   setContentItems(state,val) {
      state.contentItems = val;
   },
   dashboard_updatePrice(state,{newPrice,itemId}){
      state.contentItems.map(item =>{
         if(item.id === itemId){
            item.price = newPrice;
         }
      })
   }
}

const getters = {
   getSalesItems: state => state.salesItems,
   getContentItems: state => state.contentItems,
}

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      })
   },
   updateContentItems({commit}){
      dashboardService.getContentItems().then(items=>{
         commit('setContentItems', items);
      }) 
   }
}

export default {
   state,
   mutations,
   getters,
   actions
}