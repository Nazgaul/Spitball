<template>
      <!-- :value="getShowSchoolBlock"
      :mini-variant.sync="isMiniSideMenu"
      @input="updateDrawerValue" -->
    <v-navigation-drawer
      v-model="drawer"
      class="sideMenu"
      :class="{'higherIndex':isMediumAndUp}"
      :mini-variant.sync="isMiniSideMenu"
      :temporary="isMediumAndUp"
      :right="isRtl"
      width="220"
      mini-variant-width="62"
      app
      permanent
      clipped
      touchless
    >

      <div class="sideMenu_cont">
        <!-- <div @click="toggleMiniSideMenu" v-if="isMediumAndUp" class="sideMenu_btn"/> -->
          <div @click="isMiniSideMenu = true" v-if="isMediumAndUp" class="sideMenu_btn"></div>
        
          <v-list class="sideMenu_list_cont" dense>

                <v-list-item
                  v-for="(item, key) in dashboardList"
                  class="group_list_sideMenu_dash"
                  @click="goTo(item.route)"
                  :sel="item.sel"
                  :key="key"
                >
                  <v-list-item-content> 
                    <v-list-item-title
                      class="group_list_titles_dash"
                      :class="{'active_list_dash': currentPageChecker(item.route)}"
                    >
                        <v-icon size="18" class="group_list_icon_dash">{{item.icon}}</v-icon>
                        <span class="group_list_title_dash ml-3" v-t="item.name"></span>
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>

                <!-- <sideMenuListItem
                  v-for="(val, key) in dashboardList"
                  @goTo="goTo"
                  :currentPageChecker="currentPageChecker"
                  :item="val"
                  :key="key"
                /> -->

                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myDashboard"/> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myQuestionsItem"/> -->
                <!-- <template v-if="showMyContent"> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myContentItem"/> -->
                <!-- </template> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myBroadcast"/> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="mySessions"/> -->

                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myStudyRoomsItem"/> -->
                <!-- <template v-if="showMySales"> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myMarketingTools"/> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="mySalesItem"/> -->
                <!-- </template> -->
                <!-- <template v-if="showMyFollowers"> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myFollowersItem"/> -->
                <!-- </template> -->
                <!-- <template v-if="showMyPurchases"> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myPurchasesItem"/> -->
                <!-- </template> -->

                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myProfileItem"/> -->
                <!-- <sideMenuListItem :dashboardProps="propsListDashboard" :item="myCoursesItem"/>
                <sideMenuListItem :dashboardProps="propsListDashboard" :item="myCalendarItem"/> -->

            </v-list>
        </div>
    </v-navigation-drawer>
</template>

<script>
// import { mapGetters, mapActions } from "vuex";

import * as routeNames from '../../../../routes/routeNames.js';
import * as feedFilters from '../../../../routes/consts/feedFilters.js';
// import sideMenuListItem from './sideMenuListItem.vue';

export default {
  name: "sideMenu",
  // components:{ sideMenuListItem },
  data() {
    return {
      // sideMenulistElm: null,
      isMiniSideMenuState: false,
      isRtl: global.isRtl,
      drawer: false,
      dashboardList: {
        myDashboard:{name: this.$t('schoolBlock_dashboard'), route: routeNames.Dashboard, icon:'sbf-dashboard-sideMenu', sel:'sidemenu_dashboard_overview'},
        myQuestionsItem:{name: this.$t('schoolBlock_my_Questions'), route:'myQuestions', icon:'sbf-my-questions', sel:'sidemenu_dashboard_opportunities'},
        myContentItem:{name: this.$t('schoolBlock_my_content'), route: routeNames.MyContent, icon:'sbf-my-content', sel:'sidemenu_dashboard_myContent'},
        myBroadcast:{name: this.$t('schoolBlock_live_session'), route: routeNames.MyStudyRoomsBroadcast, icon:'sbf-myLive', sel:'sidemenu_dashboard_live_session'},
        mySessions:{name: this.$t('schoolBlock_private_session'), route: routeNames.MyStudyRooms, icon:'sbf-studyroom-icon', sel:''},
        myMarketingTools:{name: this.$t('schoolBlock_my_marekting'), route: routeNames.Marketing, icon:'sbf-myMarketing', sel:'sidemenu_settings_myMarketing'},
        mySalesItem:{name: this.$t('schoolBlock_my_sales'), route: routeNames.MySales, icon:'sbf-my-sales', sel:'sidemenu_dashboard_mySales'},
        myFollowersItem:{name: this.$t('schoolBlock_my_followers'), route: routeNames.MyFollowers, icon:'sbf-follow', sel:'sidemenu_dashboard_myFollowers'},
        myPurchasesItem:{name: this.$t('schoolBlock_purchases'), route: routeNames.MyPurchases, icon:'sbf-cart', sel:'sidemenu_dashboard_myPurchases'},
        // myProfileItem:{name: this.$t('schoolBlock_my_site'), route: routeNames.Profile, icon:'sbf-user', sel:'sidemenu_settings_myProfile'},
        myCoursesItem:{name: this.$t('schoolBlock_courses'), route: routeNames.EditCourse, icon:'sbf-classes-icon', sel:'sidemenu_settings_myCourses'},
        myCalendarItem:{name: this.$t('schoolBlock_calendar'), route: routeNames.MyCalendar, icon:'sbf-calendar', sel:'sidemenu_settings_myCalendar'},
      },
    };
  },
  computed: {
    // ...mapGetters([
    //   "accountUser",
    //   "getShowSchoolBlock",
    // ]),
    isMediumAndUp() {
      return !this.isMiniSideMenuState && this.$vuetify.breakpoint.mdAndDown
    },
    isMiniSideMenu: {
      get() {
        return this.isMiniSideMenuState && (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly)
        // return (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly)
      },
      set(val) {
        this.isMiniSideMenuState = val
        // if(this.isMiniSideMenuState && (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly)) {
        //   this.isMiniSideMenuState = true
        // }
        // this.updateDrawerValue(val)
      }
    },
    // propsListDashboard(){
    //   return { goTo: this.goTo }
    // },
    // showMyContent(){
    //     return this.accountUser?.haveContent;
    // },
    // showMySales(){
    //     return this.accountUser.isSold;
    // },
    // showMyFollowers(){
    //     return this.accountUser.haveFollowers;
    // },
    // showMyPurchases(){
    //     return this.accountUser.isPurchased;
    // },
  },
  methods: {
    // ...mapActions(["toggleShowSchoolBlock","setShowSchoolBlockMobile"]),
    goTo(name){
      if(this.isMiniSideMenuState && this.$vuetify.breakpoint.mdAndDown) {
        this.isMiniSideMenuState = false
        return
      }
      if(this.$vuetify.breakpoint.mdAndDown) {
        this.isMiniSideMenuState = true
      }
      if(name === "myQuestions"){
        this.$router.push({name: routeNames.Feed,query:{filter:feedFilters.Question}})
        return
      }
      this.$router.push({name})
      // this.closeSideMenu();
    },
    currentPageChecker(pathName){
        let isMyQuestions = (this.$route.name === routeNames.Feed && this.$route.query?.filter === feedFilters.Question)
        if(pathName === 'myQuestions' && isMyQuestions){
          return true;
        }
        if(pathName.toLowerCase().includes('course') && this.$route.path.includes('courses')){
          return true;
        }
        if(this.$route.name === pathName){
          return true;
        }
    }
    // toggleMiniSideMenu(){
    //   if(this.isMiniSideMenu){
    //     this.openSideMenu();
    //   }else{
    //     this.closeSideMenu();
    //   } 
    // },
    // openSideMenu(){
    //   if(this.$vuetify.breakpoint.xsOnly || this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly){
    //     this.setShowSchoolBlockMobile(true);
    //     this.toggleShowSchoolBlock(true);
    //   }
    // },
    // closeSideMenu(){
    //   if(this.$vuetify.breakpoint.xsOnly || this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly){
    //     this.setShowSchoolBlockMobile(false);
    //     this.toggleShowSchoolBlock(false);
    //   }
    // },
    // updateDrawerValue(val){
    //   if(this.isMiniSideMenu) {
    //     this.toggleShowSchoolBlock(!val);
    //   }
    // },
    // clickEventMiniMenuOpen(e){
    //   if(e.target.classList.contains('v-navigation-drawer--mini-variant')){
    //     this.openSideMenu();
    //   } 
    // }
  },
  // beforeDestroy(){
  //   this.sideMenulistElm.removeEventListener('click', this.clickEventMiniMenuOpen);
  // },
  // mounted(){
  //   this.sideMenulistElm = document.querySelector('.sideMenu');
  //   this.sideMenulistElm.addEventListener('click', this.clickEventMiniMenuOpen);
  // },
};
</script>

<style lang="less" src="./sideMenu.less"></style>