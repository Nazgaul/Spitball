<template>
  <!-- <div id="dashboard"> -->
      <div class="dashboardMain mr-md-6">
        <dashboardTutorActions></dashboardTutorActions>
        <dashboardNotifications></dashboardNotifications>
        <marketingTools></marketingTools>
        <spitballTips v-if="$vuetify.breakpoint.mdAndUp"></spitballTips>
      </div>
  <!-- </div> -->
</template>
<script>
const dashboardTutorActions = () => import('./dashboardTutorActions.vue');
const dashboardNotifications = () => import('./dashboardNotifications.vue');
const marketingTools = () => import('./marketingTools.vue');
const spitballTips = () => import('./spitballTips.vue');

export default {
  components: {
    dashboardTutorActions,
    dashboardNotifications,
    marketingTools,
    spitballTips,
  }
}
</script>
<style lang="less">
  @import '../../../../styles/mixin.less';
  @mainBlock: 890px;
  // #dashboard {
    // margin: 24px 34px;
    // display: flex;
    // @media (max-width: @screen-xs) {
    //   margin: 0
    // }
    // @media (max-width: @screen-md) {
    //   justify-content: center;
    // }
    .dashboardMain {
      width: 100%;
      max-width: @mainBlock;
      margin: 24px 34px;
      // display: flex;
    }
  // }
</style>
