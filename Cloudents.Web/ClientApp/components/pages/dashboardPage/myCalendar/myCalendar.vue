<template>
  <div class="myCalendar">
      <v-card class="myCalendar-container" v-if="isReady">
         <calendarEmptyState v-if="isShowEmptyState"/>
         <template v-if="isShowCalendarSettings">
            <selectCalendar/>
            <calendarHours class="my-3"/>
         </template>
      </v-card>
      <!-- <template v-if="isShowCalendarSettings">
         <selectCalendar/>
         <calendarHours class="my-3"/>
      </template> -->


     <div>
      <!-- {{getCalendarsList}} -->
     </div>
      <v-progress-circular class="progress-calendar" v-show="isLoading" indeterminate :size="300" width="3" color="#4c59ff"/>
     <!-- <div class="myCalendar_top" v-if="getShowCalendar">
        <v-btn @click="openConnect" class="myCalendar_btns white--text mr-4" :loading="isLoadingConnet" rounded depressed color="#4452fc">
           <span v-language:inner="'dashboardCalendar_btn_connect'"/>
        </v-btn>
        <v-btn @click="openAvailability" class="myCalendar_btns white--text" rounded depressed color="#4452fc">
           <span v-language:inner="'dashboardCalendar_btn_availability'"/>
         </v-btn>
     </div> -->
      <!-- <calendarTab class="myCalendar_calendar"/> -->
  </div>
</template>

<script>
import calendarEmptyState from '../../../calendar/calendarEmptyState.vue';
import selectCalendar from '../../../calendar/calendarSelect.vue';
import calendarHours from '../../../calendar/calendarHours.vue';


import { mapGetters, mapActions } from 'vuex';
export default {
   components:{
      calendarEmptyState,
      selectCalendar,
      calendarHours,
   },
   props:{
      globalFunctions: {
         type: Object,
      },
      dictionary:{
         type: Object,
         required: true
      },
   },
   data() {
      return {
         isLoadingConnet:false,
         isLoading:true,
         isReady:false,
         showEmptyState:false,
         showCalendarSettings:false,
      }
   },
   computed: {
      ...mapGetters(['accountUser','getShowCalendar','getCalendarAvailabilityState','getCalendarsList']),
      isShowEmptyState(){
         return (this.isReady && !this.isLoadingthis && this.showEmptyState && !this.showCalendarSettings)
      },
      isShowCalendarSettings(){
         return (this.isReady && !this.isLoadingthis && this.showCalendarSettings && !this.showEmptyState)
      }
   },
   methods: {
      ...mapActions(['getCalendarListAction','updateCalendarStatusDashboard','gapiLoad']),
      openConnect(){
         this.isLoadingConnet = true;
         this.getCalendarListAction().then(()=>{
            this.isLoadingConnet = false;
            this.globalFunctions.openDialog('changeCalendarList',{})
         })
      },
      openAvailability(){
         this.globalFunctions.openDialog('changeCalendarAvailability',{})
      }
   },
   created() {
      let self = this;
      self.updateCalendarStatusDashboard()
         .then(isSharedCalendar=>{
            if(!isSharedCalendar){
               self.$loadScript("https://apis.google.com/js/api.js").then(() => {
                  self.gapiLoad('calendar').then(()=>{
                     self.showEmptyState = true;
                     self.showCalendarSettings = false;
                     self.isReady = true;
                     self.isLoading = false;
                  });
               })
            }else{
               self.showCalendarSettings = true;
               self.showEmptyState = false;
               self.isReady = true;
               self.isLoading = false;
            }
         })
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.myCalendar{
   width: fit-content;
   .myCalendar-container{
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
      padding: 40px 22px;
      @media (max-width: @screen-xs) {
         box-shadow: none;
         padding: 10px;
            // margin-bottom: 40px;
      }
   }
   .myCalendar_top{
      width: 720px;
      max-width: 720px;
      display: flex;
      justify-content: center;
      .myCalendar_btns{
         text-transform: capitalize;
      }
   }
   .myCalendar_calendar{
      max-width: 720px;
      &.calendar-container{
         margin: unset;
         .calendar-header{
            display: none;
         }
         .my-event{
            pointer-events: none;
         }
      }
   }
   
}
</style>