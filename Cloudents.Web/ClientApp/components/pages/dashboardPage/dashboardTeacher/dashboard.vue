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
        <img class="mb-2 blockImage" :src="require(`${showBanner}.png`)" alt="image" @click="startIntercom">
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
    },
    showBanner() {
      debugger
      return global.lang !== 'he' ? './images/group-16' : './images/banner-he';
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
  @mainBlock: 670px;
  @sideBlock: 410px;
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
      max-width: @mainBlock;
    }
    .dashboardSide {
      width: 100%;
      max-width: @sideBlock;
      .blockImage {
        cursor: pointer;
      }
    }
  }
</style>
