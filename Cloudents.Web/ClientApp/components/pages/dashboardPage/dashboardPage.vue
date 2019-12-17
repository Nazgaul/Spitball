<template>
   <div class="dashboardPage">
      <component :is="currentComponentByRoute"></component>
      <sb-dialog 
         :showDialog="getShowDashboardDialog"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDashboardDialog"
         :activateOverlay="true"
         :max-width="'550px'"
         :content-class="'pop-dashboard-container'">
            
      </sb-dialog>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import mySales from './mySales/mySales.vue';

import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';

export default {
   name:'dashboardPage',
   components:{
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
	padding-right: 30px;
   // max-width: 1150px;
	@media (max-width: @screen-xs) {
      padding-left: 6px;
	   padding-right: 6px;
      width: 100%;
      height: 100%;
   }
}
.pop-dashboard-container {
   background: #fff;
}

</style>
