<template>
    <v-navigation-drawer
      v-model="drawer"
      v-click-outside="clickedOutside"
      class="sideMenu"
      :class="{'higherIndex':isMediumAndUp}"
      :mini-variant.sync="isMiniSideMenu"
      :temporary="isMediumAndUp"
      :right="$vuetify.rtl"
      :permanent="!$vuetify.breakpoint.xsOnly"
      width="220"
      mini-variant-width="62"
      hide-overlay
      app
      fixed
      clipped
      touchless
    >
      <div class="sideMenu_cont">
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
                        <span class="group_list_title_dash ms-3">{{item.name}}</span>
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
            </v-list>
        </div>
    </v-navigation-drawer>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames.js';

export default {
  name: "sideMenu",
  data() {
    return {
      isMiniSideMenuState: true,
      drawer: false,
      dashboardList: {
        myDashboard:{name: this.$t('schoolBlock_dashboard'), route: routeNames.Dashboard, icon:'sbf-dashboard-sideMenu', sel:'sidemenu_dashboard_overview'},
        myCourses:{name: this.$t('schoolBlock_my_courses'), route: routeNames.MyCourses, icon:'sbf-my-content', sel:'sidemenu_dashboard_myCourses'},
        myBroadcast:{name: this.$t('schoolBlock_live_session'), route: routeNames.MyStudyRoomsBroadcast, icon:'sbf-myLive', sel:'sidemenu_dashboard_live_session'},
        myFollowersItem:{name: this.$t('schoolBlock_my_followers'), route: routeNames.MyFollowers, icon:'sbf-follow', sel:'sidemenu_dashboard_myFollowers'},
        myCoupons:{name: this.$t('schoolBlock_coupons'), route: routeNames.MyCoupons, icon:'sbf-my-coupon', sel:'sidemenu_dashboard_myCoupons'},
        mySalesItem:{name: this.$t('schoolBlock_my_sales'), route: routeNames.MySales, icon:'sbf-my-sales', sel:'sidemenu_dashboard_mySales'},
        myMarketingTools:{name: this.$t('schoolBlock_my_marketing'), route: routeNames.Marketing, icon:'sbf-myMarketing', sel:'sidemenu_settings_myMarketing'},
        myPurchasesItem:{name: this.$t('schoolBlock_purchases'), route: routeNames.MyPurchases, icon:'sbf-cart', sel:'sidemenu_dashboard_myPurchases'},
        mySearch:{name: this.$t('schoolBlock_search'), route: routeNames.Learning, icon:'sbf-searchP', sel:'sidemenu_dashboard_learn'},
        mySessions:{name: this.$t('schoolBlock_private_session'), route: routeNames.MyStudyRooms, icon:'sbf-studyroom-icon', sel:''},
        myCalendarItem:{name: this.$t('schoolBlock_calendar'), route: routeNames.MyCalendar, icon:'sbf-calendar', sel:'sidemenu_settings_myCalendar'},
      },
    };
  },
  computed: {
    isMediumAndUp() {
      if(this.$vuetify.breakpoint.xsOnly) return true
      return !this.isMiniSideMenuState && this.$vuetify.breakpoint.mdAndDown
    },
    isMiniSideMenu: {
      get() {
        return this.isMiniSideMenuState && (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly)
      },
      set(val) {
        this.isMiniSideMenuState = val
      }
    },
  },
  methods: {
    clickedOutside() {
      if(this.$vuetify.breakpoint.mdAndDown) {
        this.isMiniSideMenuState = true
      }
    },
    goTo(name){
      if(this.isMiniSideMenuState && this.$vuetify.breakpoint.mdAndDown) {
        this.isMiniSideMenuState = false
        return
      }
      this.isMiniSideMenuState = true
      this.$router.push({name})
    },
    currentPageChecker(pathName){
        if(pathName.toLowerCase().includes('course') && this.$route.path.includes('courses')){
          return true;
        }
        if(this.$route.name === pathName){
          return true;
        }
    },
  },
  created() {
    this.$root.$on('openSideMenu', () => {
      this.drawer = true
    })
  }
};
</script>

<style lang="less" src="./sideMenu.less"></style>