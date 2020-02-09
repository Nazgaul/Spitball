<template>
  <div id="dashboard">
      <div class="dashboardMain mr-md-6">
        <analyticOverview></analyticOverview>
        <teacherTasks v-if="$vuetify.breakpoint.mdAndDown"></teacherTasks>
        <marketingTools></marketingTools>
        <uploadContent v-if="$vuetify.breakpoint.smAndUp"></uploadContent>
        <spitballTips v-if="$vuetify.breakpoint.mdAndUp"></spitballTips>
        <answerStudent v-if="$vuetify.breakpoint.smAndDown"></answerStudent>
      </div>
      <div class="dashboardSide" v-if="$vuetify.breakpoint.mdAndUp">
        <img class="mb-2" src="./images/group-16.png" alt="">
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
  name: "",
  components: {
    analyticOverview,
    marketingTools,
    uploadContent,
    spitballTips,
    answerStudent,
    teacherTasks
  },
  data: () => ({

  })
}
</script>
<style lang="less">
  #dashboard {
    margin: 24px 34px;
    display: flex;

    @media (max-width: 599px) {
      margin: 0
    }
    .dashboardMain {
      width: 100%;
    }
    .dashboardSide {
      width: 100%;
      max-width: 408px;
      // flex: 1;
    }
  }
</style>
