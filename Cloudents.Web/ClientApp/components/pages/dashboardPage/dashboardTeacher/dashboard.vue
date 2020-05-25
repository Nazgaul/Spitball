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
  .dashboardMain {
    width: 100%;
    max-width: 890px;
    margin: 24px 34px;
  }
</style>
