<template>
  <div class="dashboardMain me-md-6">
    <dashboardTutorActions />
    <dashboardNotifications />
    <!-- <dashboardMarketingTools /> -->
    <marketingBlogs v-if="$vuetify.breakpoint.xsOnly" />
    <spitballTips v-else />
  </div>
</template>

<script>
const marketingBlogs = () => import('../../marketing/marketingBlogs/marketingBlogs.vue')
const dashboardTutorActions = () => import('./dashboardTutorActions.vue');
const dashboardNotifications = () => import('./dashboardNotifications.vue');
// const dashboardMarketingTools = () => import('../dashboardTeacher/dashboardMarketingTools.vue')
const spitballTips = () => import('./spitballTips.vue');

export default {
  components: {
    dashboardTutorActions,
    dashboardNotifications,
    marketingBlogs,
    // dashboardMarketingTools,
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
      margin: 8px 0;
    }
  }
</style>
