<template>
   <div class="changeCalendarList">
      <v-icon @click="$emit('closeDialog')" class="changeCalendarList_close" small v-text="'sbf-close'"/>
      <calendarSelect/>
      <v-btn :loading="isLoading" @click="change" class="changeCalendarList_btn white--text" rounded depressed color="#4452fc">
         <span v-language:inner="'dashboardCalendar_btn_select'"/>
      </v-btn>
   </div>
</template>

<script>
import calendarSelect from '../../../calendar/calendarSelect.vue';
import { mapActions } from 'vuex';
export default {
   components:{calendarSelect},
   data() {
      return {
         isLoading:false,
      }
   },
   methods: {
      ...mapActions(['updateSelectedCalendarList']),
      change(){
         let self = this;
         self.isLoading = true;
         this.updateSelectedCalendarList().then(()=>{
            self.isLoading = false;
            self.$emit('closeDialog')
            let snackBarObj = {
               isOn:true,
               color:'info',
               dictionary:'dashboardCalendar_snack_connect'
            }
            self.$emit('updateSnackbar',snackBarObj)
         }).catch(()=>{
            self.$emit('closeDialog')
            let snackBarObj = {
               isOn:true,
               color:'error',
               dictionary:'dashboardCalendar_snack_connect_error'
            }
            self.$emit('updateSnackbar',snackBarObj)
         })
      }
   },
}
</script>

<style lang="less">
.changeCalendarList{
   width: 560px;
   padding: 20px;
   display: flex;
   flex-direction: column;
   align-items: center;
   position: relative;
   .changeCalendarList_close{
      position: absolute;
      right: 8px;
      top: 8px;
      font-size: 12px !important;
      cursor: pointer;
   }
   .changeCalendarList_btn{
      max-width: 160px;
      width: 100%;
      text-transform: capitalize;
   }
}
</style>