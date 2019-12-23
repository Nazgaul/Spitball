<template>
  <div class="myCalendar">
     <div class="myCalendar_top">
        <v-btn :loading="isLoadingConnet" @click="openConnet" class="myCalendar_btns white--text" round depressed color="#4452fc">Connet Google Calendars</v-btn>
        <v-btn @click="openAvailability" class="myCalendar_btns white--text" round depressed color="#4452fc">Change Availability</v-btn>
     </div>
     <!-- <calendar v-if="!!accountUser"/> -->
  </div>
</template>

<script>
import calendar from '../../../calendar/calendar.vue';

import { mapGetters, mapActions } from 'vuex';
export default {
   components:{
      calendar,
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
      }
   },
   computed: {
      ...mapGetters(['accountUser'])
   },
   methods: {
      ...mapActions(['getCalendarListAction']),
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

}
</script>

<style lang="less">
.myCalendar{
   .myCalendar_top{
      display: flex;
      justify-content: center;
      .myCalendar_btns{
         text-transform: capitalize;
      }
   }
   
}
</style>