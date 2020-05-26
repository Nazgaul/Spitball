<template>
  <div class="dashboardMain mr-md-6">
    <dashboardTutorActions />
    <dashboardNotifications />
    <marketingTools />
    <spitballTips />
  </div>
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
  .dashboardMain {
    max-width: 890px;
    margin: 24px 34px;

    @media (max-width: @screen-xs) {
      margin: 24px 0;
    }
  }
</style>
