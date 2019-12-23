<template>
   <div class="changeCalendarAvailability">
      <v-icon @click="$emit('closeDialog')" class="changeCalendarAvailability_close" small>sbf-close</v-icon>
      <calendarHours/>
      <v-btn :loading="isLoading" @click="change" class="changeCalendarAvailability_btn white--text" round depressed color="#4452fc">Change</v-btn>
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
      ...mapActions(['updateAvailabilityHours','updateToasterParams']),
      change(){
         this.isLoading = true;
         this.updateAvailabilityHours().then(()=>{
            this.isLoading = false;
            this.$emit('closeDialog')
            this.updateToasterParams({
               toasterText: 'bla bla bla calendar hours changed',
               showToaster: true
            });
         }).catch(()=>{
            this.isLoading = false;
            this.updateToasterParams({
               toasterText: 'bla bla bla calendar connected',
               showToaster: true,
               toasterType: 'error-toaster'
            });
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