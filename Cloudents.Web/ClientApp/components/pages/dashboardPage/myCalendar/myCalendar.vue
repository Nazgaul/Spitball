<template>
  <div class="myCalendar">
     <div class="myCalendar_top" v-if="getShowCalendar">
        <v-btn @click="openConnect" class="myCalendar_btns white--text mr-4" :loading="isLoadingConnet" rounded depressed color="#4452fc">
           <span v-language:inner="'dashboardCalendar_btn_connect'"/>
        </v-btn>
        <v-btn @click="openAvailability" class="myCalendar_btns white--text" rounded depressed color="#4452fc">
           <span v-language:inner="'dashboardCalendar_btn_availability'"/>
         </v-btn>
     </div>
      <calendarTab class="myCalendar_calendar"/>
  </div>
</template>

<script>
import calendarTab from '../../../calendar/calendarTab.vue';

import { mapGetters, mapActions } from 'vuex';
export default {
   components:{
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
      ...mapGetters(['accountUser','getShowCalendar'])
   },
   methods: {
      ...mapActions(['getCalendarListAction']),
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
}
</script>

<style lang="less">
.myCalendar{
   width: fit-content;
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