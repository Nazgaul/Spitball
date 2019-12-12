<template>
   <div class="dashboardPage">
      <!-- <dashboardFilters></dashboardFilters> -->
      <component :is="currentComponentByRoute"></component>
   </div>
</template>

<script>
import dashboardFilters from './dashboardFilters/dashboardFilters.vue';
import myPurchases from './myPurchases/myPurchases.vue';
import myCalendar from './myCalendar/myCalendar.vue';
import myFollowers from './myFollowers/myFollowers.vue';
import mySales from './mySales/mySales.vue';

export default {
   name:'dashboardPage',
   components:{
      dashboardFilters,
      myPurchases,
      myCalendar,
      myFollowers,
      mySales,
   },
   computed:{
      currentComponentByRoute(){
         return this.$route.path.slice(1);
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

</style>
