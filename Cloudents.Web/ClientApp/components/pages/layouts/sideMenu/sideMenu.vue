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
          <sideMenuHome @click.native="resetItems"/>

          <sideMenuDashboard @click="openSideMenu" :dashboardProps="{
                              model:dashboardModel,
                              showSchoolBlock:getShowSchoolBlock,
                              goTo:goTo,
                              openSideMenu:openSideMenu}"/>

          <sideMenuSetting @click="openSideMenu" :settingProps="{
                              model:settingModel,
                              showSchoolBlock: getShowSchoolBlock,
                              goTo:goTo,
                              openSideMenu:openSideMenu}"/>
          
            <v-list-group v-model="coursesModel" active-class="''" :prepend-icon="'sbf-courses-icon'" :append-icon="''" no-action class="sideMenu_group" @click="openSideMenu">
            <template v-slot:activator>
              <v-list-item class="sideMenu_list">
                <v-list-item-content>
                  <v-list-item-title>
                    <span class="sideMenu_list_title" v-text="$t('schoolBlock_my')"/>
                    </v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>

            <v-list-item
              class="group_list_sideMenu_course" v-for="(item, index) in selectedClasses" :key="index" 
              color="#fff"
              :to="{name: $route.name,query:{Course: (!item.isDefault)? item.text : undefined}}"
              event
              @click.native.prevent="getShowSchoolBlock ? selectCourse(item) : openSideMenu()" :sel="item.isDefault? 'all_courses' : ''">
              <v-list-item-content>
                <v-list-item-title :class="['group_list_titles_course',{'active_link_course': currentCourseChecker(item)}]">
                  <arrowSVG v-if="currentCourseChecker(item)" class="arrow_course"/>
                  <span :class="['group_list_title_course text-truncate',currentCourseChecker(item)? 'padding_current_course':'ml-4']" v-text="item.text ? item.text : $t('schoolBlock_allCourses')"/>
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-group>
        </v-list>
      </div>
    </v-navigation-drawer>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import arrowSVG from './image/left-errow.svg';

import sideMenuHome from './sideMenuHome.vue';
import sideMenuDashboard from './sideMenuDashboard.vue'
import sideMenuSetting from './sideMenuSetting.vue';
import * as routeNames from '../../../../routes/routeNames.js';
import * as feedFilters from '../../../../routes/consts/feedFilters.js';

export default {
  name: "sideMenu",
  components:{arrowSVG,sideMenuSetting,sideMenuHome,sideMenuDashboard},
  data() {
    return {
      sideMenulistElm: null,
      selectedCourse: "",
      isRtl: global.isRtl,
      dashboardModel: false,
      settingModel:false,
      coursesModel:false,
    };
  },
  computed: {
    ...mapGetters([
      "getSelectedClasses",
      "accountUser",
      "getShowSchoolBlock",
      'Feeds_getIsLoading'
    ]),
    isMiniSideMenu: {
      get() {
      return (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly) && !this.getShowSchoolBlock
      },
      set(val) {
        this.updateDrawerValue(val)
      }
    },
    selectedClasses(){
        let selectedClasses = JSON.parse(JSON.stringify(this.getSelectedClasses))
        
        selectedClasses = selectedClasses.sort((a, b) => a.text.toLowerCase() > b.text.toLowerCase() ? 1 : -1);
        let defaultCourse = {
          isFollowing: true,
          isLoading: false,
          isPending: false,
          isSelected: true,
          isTeaching: false,
          students: 0,
          text: this.$t('schoolBlock_allCourses'),
          isDefault: true
        }
        selectedClasses.unshift(defaultCourse);
        return selectedClasses;
    },
  },
  watch: {
    $route() {
      this.checkRoutes()
      if (!!this.$route.query) {
        if (!this.$route.query.Course) {
          this.selectedCourse = "";
        }
      } else {
        this.selectedCourse = "";
      }
    }
  },
  methods: {
    ...mapActions(["updateLoginDialogState","toggleShowSchoolBlock","setShowSchoolBlockMobile"]),
    checkRoutes(){
      let dashboardRoutes = [routeNames.Dashboard,routeNames.MySales,routeNames.MyFollowers,routeNames.MyPurchases,routeNames.MyContent,routeNames.MyStudyRooms];
      let settingRoutes = [routeNames.EditCourse,routeNames.MyCalendar,routeNames.EditUniversity,routeNames.Profile];
      let coursesRoutes = [routeNames.Document,routeNames.Question,routeNames.Feed];

      this.dashboardModel = [...settingRoutes,...coursesRoutes].every(route=>route !== this.$route.name);
      this.settingModel = [...dashboardRoutes,...coursesRoutes].every(route=>route !== this.$route.name);
      this.coursesModel = [...settingRoutes,...dashboardRoutes].every(route=>route !== this.$route.name);
      
      if(this.$route.name === routeNames.Feed && this.$route.query.filter === feedFilters.Question){
        this.coursesModel = false;
        this.dashboardModel = true;
      }
    },
    currentCourseChecker(item){
      if(item.isDefault){
        return this.selectedCourse === '';
      }else{
        return item.text.toLowerCase() === this.selectedCourse.toLowerCase();
      }
    },
    goTo(name){
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
        return
      }
      if(name === "myQuestions"){
        this.$router.push({name: routeNames.Feed,query:{filter:feedFilters.Question}})
        return
      }
      this.$router.push({name})
      this.closeSideMenu();
    },
    resetItems(){
      this.openSideMenu();
      this.$router.push('/')
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
    selectCourse(item) {
      if(!this.Feeds_getIsLoading){
        if(item.isDefault){
          this.selectedCourse = "";
        }else{
          this.selectedCourse = item.text;
        }
        this.updateFilter();
        this.toggleMiniSideMenu()
      }
    },
    updateFilter() {
      let newQueryObject = {Course: this.selectedCourse || undefined};
      if(this.$route.name !== routeNames.Feed){
          this.$router.push({name: routeNames.Feed, query: newQueryObject });
      }else{
        let queryObj = {
          Course: this.selectedCourse || undefined,
          term: this.$route.query.term,
          filter: this.$route.query.filter,
        }
        this.$router.push({ query: queryObj })
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
    if (!!this.$route.query.Course) {
      this.selectedCourse = this.$route.query.Course;
    }
  },
};
</script>

<style lang="less" src="./sideMenu.less"></style>
