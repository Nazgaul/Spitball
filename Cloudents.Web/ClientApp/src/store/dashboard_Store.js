import axios from 'axios'
import Moment from 'moment';

import dashboardService from '../services/dashboardService.js';
import salesService from '../services/salesService.js';
function _getColors(count){
   let colors = ['#4c59ff', '#41c4bc', '#4094ff', '#ff6f30', '#ebbc18', '#69687d', 
      '#1b2441','#5833cf', '#4daf50', '#995bea', '#074b8f', '#860941', '#757575', '#317ca0'] // 14;
   
   while (colors.length < count){
      let color = '#'+Math.floor(Math.random()*16777215).toString(16).padStart(6, '0');
      if(!colors.includes(color)){
         colors.push(color)
      }
   }
   return colors
}
const state = {
   salesItems: [],
   coursesItems: [],
   purchasesItems: [],
   studyRoomItems: [],
   followersItems: [],
   scheduledClasses: [],
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

   setSaleItem(state, sessionId) {
      //update on the fly in my-sales approve button
      let index = state.salesItems.findIndex(item => item.sessionId === sessionId)
      state.salesItems[index].paymentStatus = "Pending";
   },
   setUpdateItemPosition(state, pos) {
      const movedItem = state.coursesItems.splice(pos.oldPosition, 1)[0]
      state.coursesItems.splice(pos.newPosition, 0, movedItem)
   },
   resetCourseItems(state) {
      state.coursesItems = []
   },
   setScheduledClasses(state,classes){
      let coursesIdxs = [...new Set(classes.map(c=>c.courseId))];
      let colors = _getColors(coursesIdxs.length);
      state.scheduledClasses = classes.map( c => {
         return {
            courseId: c.courseId,
            courseName: c.courseName,
            studentEnroll: c.studentEnroll,
            date: c.broadcastTime,
            name: c.studyRoomName || '',
            id: c.studyRoomId,
            start: Moment(c.broadcastTime).format('YYYY-MM-DD HH:mm'),
            end: Moment(c.broadcastTime).format('YYYY-MM-DD HH:mm'),
            color: Moment(c.broadcastTime).isBefore()? 'grey' : colors[coursesIdxs.findIndex(i=> i===c.courseId)]
         }
      });
    }
};

const getters = {
   getSalesItems: state => state.salesItems,
   getCoursesItems: state => state.coursesItems,
   getPurchasesItems: state => state.purchasesItems,
   getStudyRoomItems: state => state.studyRoomItems,
   getFollowersItems: state => state.followersItems,
   getScheduledClasses: state => state.scheduledClasses,
};

const actions = {
   updateSalesItems({commit}) {
      dashboardService.getSalesItems().then(items=>{
         commit('setSalesItems', items);
      });
   },
   updateCoursesItems({commit}){
      return axios.get('course').then(({data})=>{
         commit('setCoursesItems', data);
      }).catch(ex => {
         console.error(ex);
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
      let params = {
         oldPosition: oldIndex,
         newPosition: newIndex
      }
      commit('setUpdateItemPosition', params)
      axios.post(`course/move`, params)
   },
   updateScheduledClasses({commit}){
      axios.get('/dashboard/upcoming').then(({data})=>{
        commit('setScheduledClasses',data)
      })
    }
};

export default {
   state,
   mutations,
   getters,
   actions
}