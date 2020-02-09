<template>
  <div id="dashboard">
      <div class="dashboardMain mr-md-6">
        <analyticOverview></analyticOverview>
        <teacherTasks v-if="$vuetify.breakpoint.smAndDown"></teacherTasks>
        <marketingTools></marketingTools>
        <uploadContent v-if="$vuetify.breakpoint.smAndUp"></uploadContent>
        <spitballTips v-if="$vuetify.breakpoint.mdAndUp"></spitballTips>
        <answerStudent v-if="$vuetify.breakpoint.smAndDown"></answerStudent>
      </div>
      <div class="dashboardSide" v-if="$vuetify.breakpoint.mdAndUp">
        <img class="mb-2" src="./images/group-16.png" alt="image">
        <teacherTasks v-if="$vuetify.breakpoint.mdAndUp"></teacherTasks>
        <answerStudent v-if="$vuetify.breakpoint.mdAndUp"></answerStudent>
    </div>
  </div>
</template>
<script>
import analyticOverview from './analyticOverview.vue';
import marketingTools from './marketingTools.vue';
import uploadContent from './uploadContent.vue';
import spitballTips from './spitballTips.vue';
import answerStudent from './answerStudent.vue';
import teacherTasks from './teacherTasks.vue';

export default {
  components: {
    analyticOverview,
    marketingTools,
    uploadContent,
    spitballTips,
    answerStudent,
    teacherTasks
  }
}
</script>
<style lang="less">
  @import '../../../../styles/mixin.less';

  #dashboard {
    margin: 24px 34px;
    display: flex;

    @media (max-width: @screen-xs) {
      margin: 0
    }
    .dashboardMain {
      width: 100%;
    }
    .dashboardSide {
      width: 100%;
      max-width: 408px;
    }
  }
</style>
