<template>
   <div class="changeCalendarList">
      <v-icon @click="$emit('closeDialog')" class="changeCalendarList_close" small>sbf-close</v-icon>
      <calendarSelect/>
      <v-btn @click="change" class="changeCalendarList_btn white--text" round depressed color="#4452fc">Select</v-btn>
   </div>
</template>

<script>
import calendarSelect from '../../../calendar/calendarSelect.vue';
import { mapActions } from 'vuex';
export default {
   components:{calendarSelect},
   methods: {
      ...mapActions(['updateSelectedCalendarList']),
      change(){
         let self = this;
         self.loading = true;
         this.updateSelectedCalendarList().then(()=>{
            self.loading = true;
            self.$emit('closeDialog')
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