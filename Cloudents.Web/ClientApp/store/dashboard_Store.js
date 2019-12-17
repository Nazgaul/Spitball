import dashboardService from '../services/dashboardService.js';

const state = {
   salesItems: [],
   contentItems: [],
   purchasesItems: [],
}

const mutations = {
   setSalesItems(state,val) {
      state.salesItems = val;
   },
   setContentItems(state,val) {
      state.contentItems = val;
   },
   setPurchasesItems(state,val) {
      state.purchasesItems = val;
   },
   dashboard_updatePrice(state,{newPrice,itemId}){
      state.contentItems.map(item =>{
         if(item.id === itemId){
            item.price = newPrice;
         }
      })
   },
   dashboard_updateName(state,{newName,itemId}){
      state.contentItems.map(item =>{
         if(item.id === itemId){
            item.name = newName;
         }
      })
   },
}

const getters = {
   getSalesItems: state => state.salesItems,
   getContentItems: state => state.contentItems,
   getPurchasesItems: state => state.purchasesItems,
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
   },
   updatePurchasesItems({commit}){
      dashboardService.getPurchasesItems().then(items=>{
         commit('setPurchasesItems', items);
      }) 
   }
}

export default {
   state,
   mutations,
   getters,
   actions
}