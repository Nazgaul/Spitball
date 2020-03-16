<template>
    <v-navigation-drawer touchless permanent app clipped :right="isRtl" width="220" mini-variant-width="62"
                         :temporary="!isMiniSideMenu && $vuetify.breakpoint.mdAndDown"
                         class="sideMenu"
                         :value="getShowSchoolBlock"
                         @input="updateDrawerValue"
                         :mini-variant.sync="isMiniSideMenu"
                         :class="[{'higherIndex':!isMiniSideMenu && $vuetify.breakpoint.mdAndDown}]">
      <div class="sideMenu_cont">
        <div @click="toggleMiniSideMenu" v-if="!isMiniSideMenu && $vuetify.breakpoint.mdAndDown" class="sideMenu_btn"/>
        
        <v-list class="sideMenu_list_cont" dense>
          <template v-if="showHome" >
            <sideMenuHome />
          </template>

          <sideMenuDashboard :dashboardProps="{
                              model:dashboardModel,
                              showSchoolBlock:getShowSchoolBlock,
                              goTo:goTo,
                              openSideMenu:openSideMenu}"/>

          <sideMenuSetting :settingProps="{
                              model:settingModel,
                              showSchoolBlock: getShowSchoolBlock,
                              goTo:goTo,
                              openSideMenu:openSideMenu}"/>
        </v-list>
      </div>
    </v-navigation-drawer>
</template>

<script>
import { mapGetters, mapActions } from "vuex";

import sideMenuHome from './sideMenuHome.vue';
import sideMenuDashboard from './sideMenuDashboard.vue'
import sideMenuSetting from './sideMenuSetting.vue';
import * as routeNames from '../../../../routes/routeNames.js';
import * as feedFilters from '../../../../routes/consts/feedFilters.js';

export default {
  name: "sideMenu",
  components:{sideMenuSetting,sideMenuHome,sideMenuDashboard},
  data() {
    return {
      sideMenulistElm: null,
      isRtl: global.isRtl,
      dashboardModel: false,
      settingModel:false,
      coursesModel:false,
    };
  },
  computed: {
    ...mapGetters([
      "accountUser",
      "getShowSchoolBlock",
    ]),
    isMiniSideMenu: {
      get() {
      return (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly) && !this.getShowSchoolBlock
      },
      set(val) {
        this.updateDrawerValue(val)
      }
    },
    showHome(){
      return !this.accountUser?.isTutor
    },
  },
  watch: {
    $route() {
      this.checkRoutes()
    }
  },
  methods: {
    ...mapActions(["toggleShowSchoolBlock","setShowSchoolBlockMobile"]),
    checkRoutes(){
      let isCourseRoute = [routeNames.EditCourse,routeNames.AddCourse,routeNames.SetCourse].find(route=> route === this.$route.name);
      let dashboardRoutes = [routeNames.Dashboard,routeNames.MySales,routeNames.MyFollowers,routeNames.MyPurchases,routeNames.MyContent,routeNames.MyStudyRooms];
      let settingRoutes = [isCourseRoute,routeNames.MyCalendar,routeNames.EditUniversity,routeNames.Profile];
      let coursesRoutes = [routeNames.Document,routeNames.Question,routeNames.Feed];

      this.dashboardModel = [...settingRoutes,...coursesRoutes].every(route=>route !== this.$route.name);
      this.settingModel = [...dashboardRoutes,...coursesRoutes].every(route=>route !== this.$route.name);
      this.coursesModel = [...settingRoutes,...dashboardRoutes].every(route=>route !== this.$route.name);
      
      if(this.$route.name === routeNames.Feed && this.$route.query.filter === feedFilters.Question){
        this.coursesModel = false;
        this.dashboardModel = true;
      }
    },
    goTo(name){
      if (this.accountUser == null) {
        // TODO: check if we needed
        this.$openDialog('login')
        return
      }
      if(name === "myQuestions"){
        this.$router.push({name: routeNames.Feed,query:{filter:feedFilters.Question}})
        return
      }
      this.$router.push({name})
      this.closeSideMenu();
    },
    toggleMiniSideMenu(){
      if(this.isMiniSideMenu){
        this.openSideMenu();
      }else{
        this.closeSideMenu();
      } 
    },
    openSideMenu(){
      if(this.$vuetify.breakpoint.xsOnly || this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly){
        this.setShowSchoolBlockMobile(true);
        this.toggleShowSchoolBlock(true);
      }
    },
    closeSideMenu(){
      if(this.$vuetify.breakpoint.xsOnly || this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly){
        this.setShowSchoolBlockMobile(false);
        this.toggleShowSchoolBlock(false);
      }
    },
    updateDrawerValue(val){
      if(this.isMiniSideMenu) {
        this.toggleShowSchoolBlock(!val);
      }
    },
    clickEventMiniMenuOpen(e){
      if(e.target.classList.contains('v-navigation-drawer--mini-variant')){
        this.openSideMenu();
      } 
    }
  },
  beforeDestroy(){
    this.sideMenulistElm.removeEventListener('click', this.clickEventMiniMenuOpen);
  },
  mounted(){
    this.sideMenulistElm = document.querySelector('.sideMenu');
    this.sideMenulistElm.addEventListener('click', this.clickEventMiniMenuOpen);
  },
  created() {
      this.checkRoutes()
  },
};
</script>

<style lang="less" src="./sideMenu.less"></style>