import dashboardService from '../services/dashboardService.js';
import walletService from '../services/walletService.js';
import searchService from '../services/searchService';

const state = {
   salesItems: [],
   contentItems: [],
   purchasesItems: [],
   balancesItems: [],
   studyRoomItems: [],
   followersItems: [],
};

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
   setBalancesItems(state,val) {
      state.balancesItems = val;
   },
   setStudyRoomItems(state,val) {
      state.studyRoomItems = val;
   },
   setFollowersItems(state,val) {
      state.followersItems = val;
   },
   dashboard_setPrice(state,{newPrice,itemId}){
      state.contentItems.forEach(item =>{
         if(item.id === itemId){
            item.price = newPrice;
         }
      });
   },
   dashboard_setName(state,{newName,itemId}){
      state.contentItems.forEach(item =>{
         if(item.id === itemId){
            item.name = newName;
         }
      });
   },
};

const getters = {
   getSalesItems: state => state.salesItems,
   getContentItems: state => state.contentItems,
   getPurchasesItems: state => state.purchasesItems,
   getBalancesItems: state => state.balancesItems,
   getStudyRoomItems: state => state.studyRoomItems,
   getFollowersItems: state => state.followersItems,
};

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      });
   },
   updateContentItems({commit}){
      dashboardService.getContentItems().then(items=>{
         commit('setContentItems', items);
      });
   },
   updatePurchasesItems({commit}){
      dashboardService.getPurchasesItems().then(items=>{
         commit('setPurchasesItems', items);
      });
   },
   updateBalancesItems({commit}){
      walletService.getBalances().then(items=>{
         commit('setBalancesItems', items);
      });
   },
   updateStudyRoomItems({commit}){
      dashboardService.getStudyRoomItems().then(items=>{
         commit('setStudyRoomItems', items);
      });
   },
   updateFollowersItems({commit}){
      dashboardService.getFollowersItems().then(items=>{
         commit('setFollowersItems', items);
      });
   },
   dashboard_updatePrice({commit},paramObj){
      commit('dashboard_setPrice',paramObj);
   },
   dashboard_updateName({commit},paramObj){
      commit('dashboard_setName',paramObj);
   },
   dashboard_sort({state},{listName,sortBy,sortedBy}){
      if(sortBy == 'date' || sortBy == 'lastSession'){
         if(sortedBy === sortBy){
            state[listName].reverse();
         }else{
            state[listName].sort((a,b)=> new Date(b[sortBy]) - new Date(a[sortBy]));
         }
         return;
      }
      if(sortedBy === sortBy){
         state[listName].reverse();
      }else{
         state[listName].sort((a,b)=> {
            if(a[sortBy] == undefined) return 1;
            if(b[sortBy] == undefined) return -1;

            if(a[sortBy] > b[sortBy])return -1;
            if(b[sortBy] > a[sortBy])return 1;
            return 0;
         });
         return;
      }
   },
   updateStudnetsAnswersQuestion() {
      return searchService.activateFunction.feed({filter: "Question"}).then((data) => {
         return data;
     }, (err) => {
         return Promise.reject(err);
     }).finally(()=>{
         return
     });
   }
};

export default {
   state,
   mutations,
   getters,
   actions
}