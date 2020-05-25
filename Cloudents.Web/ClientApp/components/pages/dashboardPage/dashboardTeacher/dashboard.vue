<template>
  <div id="dashboard">
      <div class="dashboardMain mr-md-6">
        <tutorActions></tutorActions>
        <teacherTasks v-if="$vuetify.breakpoint.smAndDown"></teacherTasks>
        <marketingTools></marketingTools>
        <uploadContent v-if="$vuetify.breakpoint.smAndUp"></uploadContent>
        <spitballTips v-if="$vuetify.breakpoint.mdAndUp"></spitballTips>
        <answerStudent v-if="$vuetify.breakpoint.smAndDown"></answerStudent>
      </div>
  </div>
</template>
<script>
const tutorActions = () => import('./tutorActions.vue');
const marketingTools = () => import('./marketingTools.vue');
const uploadContent = () => import('./uploadContent.vue');
const spitballTips = () => import('./spitballTips.vue');
const answerStudent = () => import('./answerStudent.vue');
const teacherTasks = () => import('./teacherTasks.vue');

export default {
  components: {
    tutorActions,
    marketingTools,
    uploadContent,
    spitballTips,
    answerStudent,
    teacherTasks,
  },
  watch: {
    isTutor(newVal) {
        if(!newVal) {
          this.$router.push({name: 'feed'});
        }
    }
  },
  computed: {
    isTutor() {
      let user = this.$store.getters.accountUser
      return user && user.isTutor;
    }
  }
}
</script>
<style lang="less">
  @import '../../../../styles/mixin.less';
  @mainBlock: 890px;
  #dashboard {
    margin: 24px 34px;
    display: flex;
    @media (max-width: @screen-xs) {
      margin: 0
    }
    @media (max-width: @screen-md) {
      justify-content: center;
    }
    .dashboardMain {
      width: 100%;
      max-width: @mainBlock;
    }
  }
</style>
