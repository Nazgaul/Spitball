<template>
  <div class="myCalendar">
     <template v-if="isReady">
         <v-card class="myCalendar-container" v-if="isShowEmptyState">
            <calendarEmptyState @updateCalendar="updateCalendar"/>
         </v-card>
         <template v-if="isShowCalendarSettings">
            <v-card class="myCalendar-container mb-2 mb-sm-3">
               <calendarHours class="calendarAvailability"/>
               <v-btn :loading="isLoadingAvailability" @click="changeAvailability" class="btn white--text" rounded depressed color="#4452fc">
                  <span v-t="'dashboardCalendar_btn_update'"/>
               </v-btn>
            </v-card> 
            <v-card class="myCalendar-container">
               <p class="choose-title" v-t="'dashboardCalendar_title_choose'"/>
               <selectCalendar class="calendarList"/>
               <v-btn :loading="isLoadingList" @click="changeList" class="btn white--text" rounded depressed color="#4452fc">
                  <span v-t="'dashboardCalendar_btn_update'"/>
               </v-btn>
            </v-card>
         </template>
     </template>
      <v-progress-circular class="progress-calendar" v-show="isLoading" indeterminate :size="300" width="3" color="#4c59ff"/>
      <v-snackbar v-model="snackBarObj.isOn" :color="snackBarObj.color" :top="true" :timeout="3000">
         <span v-t="snackBarObj.dictionary"></span>
      </v-snackbar>
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
      dictionary:{
         type: Object,
         required: true
      },
   },
   data() {
      return {
         isLoading:true,
         isReady:false,
         showEmptyState:false,
         showCalendarSettings:false,
         isLoadingAvailability:false,
         isLoadingList:false,
         snackBarObj:{
            isOn:false,
            color:'info',
            dictionary:''
         }
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
      ...mapActions(['updateCalendarStatusDashboard','gapiLoad','updateAvailabilityCalendar','updateSelectedCalendarList']),
      changeAvailability(){
         this.isLoadingAvailability = true;
         this.updateAvailabilityCalendar().then(()=>{
            this.isLoadingAvailability = false;
            this.snackBarObj = {
               isOn:true,
               color:'info',
               dictionary:'dashboardCalendar_snack_availability'
            }
         }).catch(()=>{
            this.isLoadingAvailability = false;
            this.snackBarObj = {
               isOn:true,
               color:'error',
               dictionary:'dashboardCalendar_snack_availability_error'
            }
         })
      },
      changeList(){
         let self = this;
         self.isLoadingList = true;
         this.updateSelectedCalendarList().then(()=>{
            self.isLoadingList = false;
            this.snackBarObj = {
               isOn:true,
               color:'info',
               dictionary:'dashboardCalendar_snack_connect'
            }
         }).catch(()=>{
            this.snackBarObj = {
               isOn:true,
               color:'error',
               dictionary:'dashboardCalendar_snack_connect_error'
            }
         })
      },
      updateCalendar(){
         this.isReady = false;
         this.isLoading = true;
         this.updateCalendarDashboard()
      },
      updateCalendarDashboard(){
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
      }
   },
   created() {
      this.updateCalendarDashboard()
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.myCalendar{
   width: 100%;
   max-width: 700px;
   .myCalendar-container{
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
      padding: 14px 22px;
      text-align: center;  
      @media (max-width: @screen-xs) {
         box-shadow: none;
         padding: 10px;
      }
      .choose-title{
         text-align: initial;
         font-size: 16px;
         font-weight: 600;
         color: #4d4b69;
         margin-bottom: 14px;
      }
      .calendarAvailability{
         text-align: initial !important;

      }
      .calendarList{
         max-width: 434px;
         text-align: initial;
      }
      .btn{
         margin-top: 40px;
         @media (max-width: @screen-xs) {
            margin-top: 30px;
         }
         height: 40px;
         max-width: 140px;
         width: 100%;
         text-transform: capitalize;
      }
   }
   .myCalendar_calendar{
      max-width: 700px;
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