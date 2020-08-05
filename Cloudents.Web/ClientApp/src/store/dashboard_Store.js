import axios from 'axios'

import dashboardService from '../services/dashboardService.js';
import salesService from '../services/salesService.js';

const state = {
   salesItems: [],
   coursesItems: [],
   purchasesItems: [],
   studyRoomItems: [],
   followersItems: [],
};

const mutations = {
   setSalesItems(state,val) {
      state.salesItems = val;
   },
   setCoursesItems(state, data) {
      function CourseItem(objInit) {
         this.documents = objInit.documents
         this.id = objInit.id
         this.image = objInit.image
         this.isPublish = objInit.isPublish
         this.lessons = objInit.lessons
         this.name = objInit.name
         this.price = objInit.price
         this.users = objInit.users
         this.type = objInit.type;
         this.startOn = objInit.startOn ? new Date(objInit.startOn) : '';
      }
      for (let i = 0; i < data.length; i++) {
         state.coursesItems.push(new CourseItem(data[i]));
      }
      
   },
   setPurchasesItems(state,val) {
      state.purchasesItems = val;
   },
   setStudyRoomItems(state,val) {
      state.studyRoomItems = val;
   },
   setFollowersItems(state,val) {
      state.followersItems = val;
   },
   // dashboard_setName(state,{newName,itemId}){
   //    state.contentItems.forEach(item =>{
   //       if(item.id === itemId){
   //          item.name = newName;
   //       }
   //    });
   // },
   setSaleItem(state, sessionId) {
      //update on the fly in my-sales approve button
      let index = state.salesItems.findIndex(item => item.sessionId === sessionId)
      state.salesItems[index].paymentStatus = "Pending";
   },
   resetCourseItems(state) {
      state.coursesItems = []
   }
};

const getters = {
   getSalesItems: state => state.salesItems,
   getCoursesItems: state => state.coursesItems,
   getPurchasesItems: state => state.purchasesItems,
   getStudyRoomItems: state => state.studyRoomItems,
   getFollowersItems: state => state.followersItems,
};

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      });
   },
   updateCoursesItems({commit}){
      axios.get('course').then(({data})=>{
         commit('setCoursesItems', data);
      }).catch(ex => {
         console.log(ex);
      })
   },
   updatePurchasesItems({commit}){
      dashboardService.getPurchasesItems().then(items=>{
         commit('setPurchasesItems', items);
      });
   },
   updateStudyRoomItems({commit}, type){
      return dashboardService.getStudyRoomItems(type).then(items=>{
         commit('setStudyRoomItems', items);
      });
   },
   updateFollowersItems({commit}){
      return dashboardService.getFollowersItems().then(items=>{
         commit('setFollowersItems', items);
      });
   },
   // dashboard_updateName({commit},paramObj){
   //    commit('dashboard_setName',paramObj);
   // },
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

      }
   },
   // updateTutorActions() {
   //    return dashboardService.getTutorActions()
   // },
   updateSpitballBlogs() {
      return dashboardService.getSpitballBlogs()
   },
   updateMarketingBlogs() {
      return dashboardService.getMarketingBlogs()
   },
   updateSalesSessions(context, params) {
      return dashboardService.getSalesSessions(params);
   },
   updateSessionDuration(context, session) {
      return dashboardService.updateSessionDuration(session)
   },
   deleteStudyRoomSession(context, id) {
      return dashboardService.removeStudyRoomSession(id)
   },
   updateBillOffline(context,params){
      return salesService.updateBillOffline(params);
   },
   updateCoursePosition({commit}, {oldIndex, newIndex}) {
      debugger
      let params = {
          oldPosition: oldIndex,
          newPosition: newIndex
      }
      axios.post(`course/move`, params)
  }
};

export default {
   state,
   mutations,
   getters,
   actions
}