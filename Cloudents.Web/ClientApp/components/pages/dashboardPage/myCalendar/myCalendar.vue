<template>
  <div class="myCalendar">
     <div class="myCalendar_top">
        <v-btn :loading="isLoadingConnet" @click="openConnet" class="myCalendar_btns white--text" round depressed color="#4452fc">Connet Google Calendars</v-btn>
        <v-btn @click="openAvailability" class="myCalendar_btns white--text" round depressed color="#4452fc">Change Availability</v-btn>
     </div>
     <calendarTab class="myCalendar_calendar" v-if="!!accountUser && isReady"/>
  </div>
</template>

<script>
import calendar from '../../../calendar/calendar.vue';
import calendarTab from '../../../calendar/calendarTab.vue';

import { mapGetters, mapActions } from 'vuex';
export default {
   components:{
      calendar,
      calendarTab
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
         isReady:false,
      }
   },
   computed: {
      ...mapGetters(['accountUser'])
   },
   methods: {
      ...mapActions(['getCalendarListAction','initCalendar']),
      openConnet(){
         this.isLoadingConnet = true;
         this.getCalendarListAction().then(()=>{
            this.isLoadingConnet = false;
            this.globalFunctions.openDialog('changeCalendarList',{})
         })
      },
      openAvailability(){
         this.globalFunctions.openDialog('changeCalendarAvailability',{})
         // changeCalendarAvailability
      }
   },
   mounted() {
       let self = this;
        this.$loadScript("https://apis.google.com/js/api.js").then(() => {
           if(self.accountUser.isTutor){
              let tutorId = self.accountUser.id
               self.initCalendar(tutorId).then(()=>{
                  self.isReady = true;
               })
           }
        })
   },
}
</script>

<style lang="less">
.myCalendar{
   width: fit-content;
   .myCalendar_top{
      width: 620px;
      padding-bottom: 20px;
      display: flex;
      justify-content: center;
      .myCalendar_btns{
         text-transform: capitalize;
      }
   }
   .myCalendar_calendar{
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