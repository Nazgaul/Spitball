<template>
   <div class="dashboardPage">
      <!-- <dashboardFilters></dashboardFilters> -->
      <component :is="currentComponentByRoute"></component>
      <sb-dialog 
         :showDialog="getShowDashboardDialog"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDashboardDialog"
         :activateOverlay="true"
         :max-width="'550px'"
         :content-class="'pop-dashboard-container'">
            <dashboardDialog></dashboardDialog>
      </sb-dialog>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';
import dashboardDialog from './dashboardDialog/dashboardDialog.vue';
import dashboardFilters from './dashboardFilters/dashboardFilters.vue';
import myPurchases from './myPurchases/myPurchases.vue';
import myCalendar from './myCalendar/myCalendar.vue';
import myFollowers from './myFollowers/myFollowers.vue';
import mySales from './mySales/mySales.vue';

export default {
   name:'dashboardPage',
   components:{
      dashboardFilters,
      dashboardDialog,
      myPurchases,
      myCalendar,
      myFollowers,
      mySales,
      sbDialog
   },
   computed:{
      ...mapGetters(['getShowDashboardDialog']),

      currentComponentByRoute(){
         return this.$route.path.slice(1);
      }
   },
   methods: {
      ...mapActions(['openDashboardDialog']),

      closeDashboardDialog() {
         this.openDashboardDialog(false);
      }
   }

}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.dashboardPage{
	padding-left: 30px;
	padding-top: 30px;
   max-width: 1150px;
   // padding: 34px;
	@media (max-width: @screen-xs) {
		padding-left: 0;
      width: 100%;
      height: 100%;
   }
}
.pop-dashboard-container {
   background: #fff;
}

</style>
