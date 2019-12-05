import analyticsService from '../services/analytics.service';

const state = {
   
}
const mutations = {
   
}
const getters = {
   
}
const actions = {
   updateAnalytics_unitedEvent(context,[...args]){
      analyticsService.sb_unitedEvent(...args)    
   },
}
export default {
   state,
   mutations,
   getters,
   actions
}