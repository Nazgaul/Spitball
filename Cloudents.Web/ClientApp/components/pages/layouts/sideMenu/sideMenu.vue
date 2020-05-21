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

          <sideMenuListItem :dashboardProps="propsListDashboard" :item="overviewItem"/>
          <sideMenuListItem :dashboardProps="propsListDashboard" :item="myQuestionsItem"/>
          <template v-if="showMyContent">
            <sideMenuListItem :dashboardProps="propsListDashboard" :item="myContentItem"/>
          </template>
          <sideMenuListItem :dashboardProps="propsListDashboard" :item="myStudyRoomsItem"/>
          <template v-if="showMySales">
            <sideMenuListItem :dashboardProps="propsListDashboard" :item="mySalesItem"/>
          </template>
          <template v-if="showMyFollowers">
            <sideMenuListItem :dashboardProps="propsListDashboard" :item="myFollowersItem"/>
          </template>
          <template v-if="showMyPurchases">
            <sideMenuListItem :dashboardProps="propsListDashboard" :item="myPurchasesItem"/>
          </template>


          <sideMenuListItem :dashboardProps="propsListDashboard" :item="myProfileItem"/>
          <sideMenuListItem :dashboardProps="propsListDashboard" :item="myCoursesItem"/>
          <sideMenuListItem :dashboardProps="propsListDashboard" :item="myCalendarItem"/>
        </v-list>
      </div>
    </v-navigation-drawer>
</template>

<script>
import { mapGetters, mapActions } from "vuex";

import * as routeNames from '../../../../routes/routeNames.js';
import * as feedFilters from '../../../../routes/consts/feedFilters.js';
import sideMenuListItem from './sideMenuListItem.vue';

export default {
  name: "sideMenu",
  components:{sideMenuListItem},
  data() {
    return {
      sideMenulistElm: null,
      isRtl: global.isRtl,

      overviewItem:{name: this.$t('schoolBlock_overview'),route: routeNames.Dashboard, icon:'sbf-eye', sel:'sidemenu_dashboard_overview'},
      mySalesItem:{name: this.$t('schoolBlock_my_sales'),route: routeNames.MySales, icon:'sbf-my-sales', sel:'sidemenu_dashboard_mySales'},
      myFollowersItem:{name: this.$t('schoolBlock_my_followers'),route: routeNames.MyFollowers, icon:'sbf-follow', sel:'sidemenu_dashboard_myFollowers'},
      myPurchasesItem:{name: this.$t('schoolBlock_purchases'),route: routeNames.MyPurchases, icon:'sbf-cart', sel:'sidemenu_dashboard_myPurchases'},
      myContentItem:{name: this.$t('schoolBlock_my_content'),route: routeNames.MyContent, icon:'sbf-my-content', sel:'sidemenu_dashboard_myContent'},
      myStudyRoomsItem:{name: this.$t('schoolBlock_study'),route: routeNames.MyStudyRooms, icon:'sbf-studyroom-icon', sel:'sidemenu_dashboard_myStudyRooms'},
      myQuestionsItem:{name: this.$t('schoolBlock_my_Questions'),route:'myQuestions',icon:'sbf-my-questions',sel:'sidemenu_dashboard_opportunities'},

      myProfileItem:{name: this.$t('schoolBlock_my_site'),route: routeNames.Profile, icon:'sbf-user',sel:'sidemenu_settings_myProfile'},
      myCoursesItem:{name: this.$t('schoolBlock_courses'),route: routeNames.EditCourse, icon:'sbf-classes-icon', sel:'sidemenu_settings_myCourses'},
      myCalendarItem:{name: this.$t('schoolBlock_calendar'),route: routeNames.MyCalendar, icon:'sbf-calendar', sel:'sidemenu_settings_myCalendar'},
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
    propsListDashboard(){
      return { goTo:this.goTo,
               openSideMenu:this.openSideMenu}
    },
    showMyContent(){
        return this.accountUser?.haveContent;
    },
    showMySales(){
        return this.accountUser.isSold;
    },
    showMyFollowers(){
        return this.accountUser.haveFollowers;
    },
    showMyPurchases(){
        return this.accountUser.isPurchased;
    },
  },
  methods: {
    ...mapActions(["toggleShowSchoolBlock","setShowSchoolBlockMobile"]),
    goTo(name){
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
};
</script>

<style lang="less" src="./sideMenu.less"></style>