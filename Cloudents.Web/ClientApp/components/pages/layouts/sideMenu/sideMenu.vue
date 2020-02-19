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

          <v-list-group @click="resetItems" class="sideMenu_group" :prepend-icon="'sbf-home-sideMenu'" no-action :append-icon="''" active-class="''">
            <template v-slot:activator>
              <v-list-item class="sideMenu_list">
                <v-list-item-content>
                  <v-list-item-title>
                    <span class="sideMenu_list_title" v-language:inner="'schoolBlock_home'"/>
                  </v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>
          </v-list-group>

          <v-list-group v-model="dashboardModel" v-if="dashboardList" @click="openSideMenu" class="sideMenu_group" active-class="''" :prepend-icon="'sbf-dashboard-sideMenu'" :append-icon="''" no-action>
            <template v-slot:activator>
              <v-list-item class="sideMenu_list">
                <v-list-item-content>
                  <v-list-item-title>
                    <span class="sideMenu_list_title" v-language:inner="'schoolBlock_dashboard'"/>
                    </v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>

            <v-list-item class="group_list_sideMenu_dash" v-for="(item, index) in dashboardListFiltered" :key="index"
              :to="{name: item.route}"
              event
              @click.native.prevent="getShowSchoolBlock ? goTo(item.route) : openSideMenu()" :sel="item.sel" v-show="isShowItem(item.route)">
              <v-list-item-content> 
                <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':currentPageChecker(item.route)}]">
                  <v-icon class="group_list_icon_dash" v-html="item.icon"/>
                  <span class="group_list_title_dash ml-3">{{item.name}}</span>
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>

          </v-list-group>
          
          <v-list-group :value="!dashboardModel" active-class="''" :prepend-icon="'sbf-courses-icon'" :append-icon="''" no-action class="sideMenu_group" @click="openSideMenu">
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

export default {
  name: "sideMenu",
  components:{arrowSVG},
  data() {
    return {
      sideMenulistElm: null,
      dashboardList:[
        {name: this.$t('schoolBlock_overview'), key:'dashboard', route: 'dashboardTeacher', icon:'sbf-eye', sel:'sd_dashboard', premission: 'tutor'},
        {name: this.$t('schoolBlock_profile'), key:'profile', route: 'profile', icon:'sbf-user', sel:'sd_profile', },
        {name: this.$t('schoolBlock_my_sales'), key:'my-sales', route: 'mySales', icon:'sbf-cart',sel:'sd_sales'},
        {name: this.$t('schoolBlock_my_followers'), key:'my-followers', route: 'myFollowers', icon:'sbf-follow',sel:'sd_followers'},
        {name: this.$t('schoolBlock_purchases'), key:'my-purchases', route: 'myPurchases', icon:'sbf-cart',sel:'sd_purchases'},
        {name: this.$t('schoolBlock_my_content'), key:'my-content', route: 'myContent', icon:'sbf-my-content',sel:'sd_content'},
        {name: this.$t('schoolBlock_calendar'), key:'my-calendar',route: 'myCalendar', icon:'sbf-calendar',sel:'sd_calendar'},
        {name: this.$t('schoolBlock_study'), key:'studyRooms', route: 'roomSettings', icon:'sbf-studyroom-icon',sel:'sd_studyroom'},
        {name: this.$t('menuList_my_study_rooms'), key:'tutoring', route: 'tutoring', icon:'sbf-pc',sel:'menu_row'},
        {name: this.$t('menuList_changeUniversity'), key:'university', route: 'addUniversity', icon:'sbf-university',sel:'sd_studyroom'},
        {name: this.$t('schoolBlock_courses'), key:'courses', route: 'editCourse', icon:'sbf-classes-icon'},
      ],
      selectedCourse: "",
      isRtl: global.isRtl,
      dashboardModel: this.$route.name !== 'feed' && this.$route.name !== 'document',
    };
  },
  computed: {
    ...mapGetters([
      "getSelectedClasses",
      "accountUser",
      "getShowSchoolBlock",
      'Feeds_getIsLoading'
    ]),
    dashboardListFiltered() {
      return this.dashboardList.filter(list => {
        return list.premission !== 'tutor' || this.isTutor;
      })
    },
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
    isTutor() {
      return this.accountUser?.isTutor
    }
  },
  watch: {
    $route() {
      this.dashboardModel = this.$route.name !== 'feed' && this.$route.name !== 'document'
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
    currentCourseChecker(item){
      if(item.isDefault){
        return this.selectedCourse === '';
      }else{
        return item.text.toLowerCase() === this.selectedCourse.toLowerCase();
      }
    },
    currentPageChecker(pathName){
      if(this.$route.name === 'myStudyRooms' && pathName === 'roomSettings'){
        return true
      }
      if(this.$route.name === pathName){
        return true;
      }
    },
    isShowItem(itemRoute){
      if(itemRoute === 'myCalendar'){
        return (!!this.accountUser && this.accountUser.isTutor)
      }else{
        return true;
      }
    },
    goTo(name){
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
        return
      }
      if(name === "roomSettings"){
        this.$router.push({name:'myStudyRooms'})
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
      if(this.$route.name !== 'feed'){
          this.$router.push({name: 'feed', query: newQueryObject });
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
    if (!!this.$route.query.Course) {
      this.selectedCourse = this.$route.query.Course;
    }
  },
};
</script>

<style lang="less" src="./sideMenu.less"></style>
