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
        <img class="mb-2 blockImage" src="./images/group-16.png" alt="image" @click="startIntercom">
        <teacherTasks v-if="$vuetify.breakpoint.mdAndUp"></teacherTasks>
        <answerStudent v-if="$vuetify.breakpoint.mdAndUp"></answerStudent>
      </div>
  </div>
</template>
<script>
import intercomService from "../../../../services/intercomService";

const analyticOverview = () => import('../../global/analyticOverview/analyticOverview.vue');
const marketingTools = () => import('./marketingTools.vue');
const uploadContent = () => import('./uploadContent.vue');
const spitballTips = () => import('./spitballTips.vue');
const answerStudent = () => import('./answerStudent.vue');
const teacherTasks = () => import('./teacherTasks.vue');

export default {
  components: {
    analyticOverview,
    marketingTools,
    uploadContent,
    spitballTips,
    answerStudent,
    teacherTasks
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
  },
  methods: {
    startIntercom() {
      intercomService.showDialog();
    },  
  },
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
    @media (max-width: @screen-sm) {
      justify-content: center;
    }
    .dashboardMain {
      width: 100%;
      max-width: 670px;
    }
    .dashboardSide {
      width: 100%;
      max-width: 408px;
      .blockImage {
        cursor: pointer;
        width: inherit;
      }
    }
  }
</style>
