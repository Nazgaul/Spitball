<template>
   <div class="changeCalendarAvailability">
      <v-icon @click="$emit('closeDialog')" class="changeCalendarAvailability_close" small v-text="'sbf-close'"/>
      <calendarHours/>
      <v-btn :loading="isLoading" @click="change" class="changeCalendarAvailability_btn white--text" rounded depressed color="#4452fc">
           <span v-language:inner="'dashboardCalendar_btn_change'"/>
      </v-btn>
   </div>
</template>
<script>
import calendarHours from '../../../calendar/calendarHours.vue';
import { mapActions } from 'vuex';

export default {
   components:{calendarHours},
   data() {
      return {
         isLoading:false,
      }
   },
   methods: {
      ...mapActions(['updateAvailabilityCalendar']),
      change(){
         this.isLoading = true;
         this.updateAvailabilityCalendar().then(()=>{
            this.isLoading = false;
            this.$emit('closeDialog')
            let snackBarObj = {
               isOn:true,
               color:'info',
               dictionary:'dashboardCalendar_snack_availability'
            }
            this.$emit('updateSnackbar',snackBarObj)
         }).catch(()=>{
            this.isLoading = false;
            let snackBarObj = {
               isOn:true,
               color:'error',
               dictionary:'dashboardCalendar_snack_availability_error'
            }
            this.$emit('updateSnackbar',snackBarObj)
         })
      }
   },
}
</script>

<style lang="less">
.changeCalendarAvailability{
   padding: 20px;
   display: flex;
   flex-direction: column;
   align-items: center;
   position: relative;
   .changeCalendarAvailability_close{
      position: absolute;
      right: 8px;
      top: 8px;
      font-size: 12px !important;
      cursor: pointer;
   }
   .changeCalendarAvailability_btn{
      max-width: 160px;
      width: 100%;
      text-transform: capitalize;
   }
}
</style>