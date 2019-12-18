<template>
    <v-navigation-drawer touchless
                         permanent
                         :temporary="!isMiniSideMenu && $vuetify.breakpoint.mdAndDown"
                         class="sideMenu"
                         width="276"
                         :value="getShowSchoolBlock"
                         @input="updateDrawerValue"
                         :mini-variant-width="62"
                         :mini-variant.sync="isMiniSideMenu"
                         :right="isRtl" 
                         :class="[{'higherIndex':!isMiniSideMenu && $vuetify.breakpoint.mdAndDown}]"
                         app
                         clipped>
      <div class="sideMenu_cont">
        <div @click="toggleMiniSideMenu" v-if="!isMiniSideMenu && $vuetify.breakpoint.mdAndDown" class="sideMenu_btn"/>
      <v-list class="sideMenu_list_cont" dense>
        <v-list-group active-class="''" :prepend-icon="'sbf-home-sideMenu'" :append-icon="''" no-action class="sideMenu_group" @click="resetItems">
          <template v-slot:activator>
            <v-list-tile class="sideMenu_list">
              <v-list-tile-content>
                <v-list-tile-title>
                  <span class="sideMenu_list_title" v-language:inner="'schoolBlock_home'"/>
                  </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </template>
        </v-list-group>

        <v-list-group v-model="dashboardModel" active-class="''" v-if="dashboardList" :prepend-icon="'sbf-dashboard-sideMenu'" :append-icon="''" no-action class="sideMenu_group" @click="openSideMenu">
          <template v-slot:activator>
            <v-list-tile class="sideMenu_list">
              <v-list-tile-content>
                <v-list-tile-title>
                  <span class="sideMenu_list_title" v-language:inner="'schoolBlock_dashboard'"/>
                  </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </template>
              <v-list-tile class="group_list_sideMenu_dash" v-for="(item, index) in dashboardList" :key="index"
              :to="{name: item.key}"
              event
               @click.native.prevent="getShowSchoolBlock ? goTo(item.key, item) : openSideMenu()" :sel="item.sel">
                <v-list-tile-content> 
                  <v-list-tile-title :class="['group_list_titles_dash',{'active_list_dash':currentPageChecker(item.key)}]">
                    <v-icon class="group_list_icon_dash" v-html="item.icon"/>
                    <span class="group_list_title_dash ml-2">{{item.name}}</span>
                  </v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>

              <v-list-tile class="group_list_sideMenu_dash" :to="{ name: 'tutoring'}" sel="menu_row">
              <v-list-tile-content>
                <v-list-tile-title :class="['group_list_titles_dash']" >
                  <v-icon style="font-size: 17px;" class="group_list_icon_dash" v-html="'sbf-pc'"/>
                  <span class="group_list_title_dash ml-2" v-language:inner="'menuList_my_study_rooms'"/>
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>

          <v-list-tile @click="openSblToken" class="group_list_sideMenu_dash">
            <v-list-tile-content>
              <v-list-tile-title :class="['group_list_titles_dash']">
                <getPts class="pts_svg"/>
                <!-- <v-icon class="group_list_icon_dash" v-html="'sbf-points'"/> -->
                <span class="group_list_title_dash ml-2" v-language:inner="'menuList_points'"/>

              </v-list-tile-title>  
            </v-list-tile-content>
          </v-list-tile>

          <v-list-tile class="group_list_sideMenu_dash" event @click.native.prevent="openPersonalizeUniversity()" :to="{name: 'addUniversity'}">
            <v-list-tile-content>
              <v-list-tile-title :class="['group_list_titles_dash',{'active_list_dash':currentPageChecker('addUniversity')}]">
                <v-icon class="group_list_icon_dash" v-html="'sbf-university'"/>
                <span class="group_list_title_dash ml-2" v-language:inner="'menuList_changeUniversity'"/>
              </v-list-tile-title> 
            </v-list-tile-content>
          </v-list-tile>

        </v-list-group>
        
        <v-list-group :value="true" active-class="''" :prepend-icon="'sbf-courses-icon'" :append-icon="''" no-action class="sideMenu_group" @click="openSideMenu" >
          <template v-slot:activator>
            <v-list-tile class="sideMenu_list">
              <v-list-tile-content>
                <v-list-tile-title>
                  <span class="sideMenu_list_title" v-text="courseSelectText"/>
                  </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </template>
          <v-list-tile
            class="group_list_sideMenu_course" v-for="(item, index) in selectedClasses" :key="index" 
            :to="{name: $route.name}"
            event
            @click.native.prevent="getShowSchoolBlock ? selectCourse(item) : openSideMenu()" :sel="item.isDefault? 'all_courses' : ''">
            <v-list-tile-content>
              <v-list-tile-title :class="['group_list_titles_course',{'active_link_course': currentCourseChecker(item)}]">
                <arrowSVG v-if="currentCourseChecker(item)" class="arrow_course"/>
                <span :class="['group_list_title_course',currentCourseChecker(item)? 'padding_current_course':'ml-4']" v-text="item.text ? item.text : dictionary.allCourses"/>
              </v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        
        </v-list-group>
      </v-list>
      </div>
    </v-navigation-drawer>
</template>

<script>
import { mapGetters, mapActions, mapMutations } from "vuex";

import arrowSVG from './image/left-errow.svg';

import {LanguageService} from "../../../../services/language/languageService";
import analyticsService from '../../../../services/analytics.service';
import getPts from './image/get-points.svg';




export default {
  name: "sideMenu",
  components:{arrowSVG,getPts},
  data() {
    return {
      sideMenulistElm: null,
      dashboardModel: false,
      dashboardList:[
        {name: LanguageService.getValueByKey('schoolBlock_profile'), key:'profile', icon:'sbf-user', sel:'sd_profile'},
        {name: LanguageService.getValueByKey('schoolBlock_wallet'), key:'wallet', icon:'sbf-wallet' ,sel:'sd_wallet'},
        {name: LanguageService.getValueByKey('schoolBlock_study'), key:'studyRooms', icon:'sbf-studyroom-icon',sel:'sd_studyroom'},
        {name: LanguageService.getValueByKey('schoolBlock_my_sales'), key:'my-sales', icon:'sbf-cart',sel:'sd_sales'},
        {name: LanguageService.getValueByKey('schoolBlock_my_content'), key:'my-content', icon:'sbf-cart',sel:'sd_content'},
        // {name: LanguageService.getValueByKey('schoolBlock_lessons'), key:'lessons', icon:'sbf-lessons'},
        {name: LanguageService.getValueByKey('schoolBlock_courses'), key:'editCourse', icon:'sbf-classes-icon'},
        // {name: LanguageService.getValueByKey('schoolBlock_posts'), key:'posts', icon:'sbf-studyroom-icon'},
        // {name: LanguageService.getValueByKey('schoolBlock_purchases'), key:'myPurchases', icon:'sbf-cart',sel:'sd_purchases'},
        // {name: 'myCalendar', key:'myCalendar', icon:'sbf-cart',sel:'sd_calendar'},
        // {name: 'myFollowers', key:'myFollowers', icon:'sbf-cart',sel:'sd_followers'},
      ],
      selectedCourse: "",
      lock: false,
      isRtl: global.isRtl,
      dictionary:{
        myCourses: LanguageService.getValueByKey('schoolBlock_my'),
        addcourses: LanguageService.getValueByKey('schoolBlock_add_your_courses'),
        allCourses: LanguageService.getValueByKey('schoolBlock_allCourses'),
      },
      inUniselect: this.$route.path.indexOf('courses') > -1 || this.$route.path.indexOf('university') > -1,
      inStudyRoomLobby: this.$route.path.indexOf('study-rooms') > -1,
      items: [],
    };
  },
  computed: {
    ...mapGetters([
      "getSelectedClasses",
      "accountUser",
      "getSearchLoading",
      "getShowSchoolBlock",
    ]),
    isMiniSideMenu(){
      return (this.$vuetify.breakpoint.mdOnly || this.$vuetify.breakpoint.smOnly) && !this.getShowSchoolBlock
    },
    courseSelectText(){
      return !!this.accountUser ? this.dictionary.myCourses : this.dictionary.addcourses;
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
          text: this.dictionary.allCourses,
          isDefault: true
        }

        selectedClasses.unshift(defaultCourse);
        return selectedClasses;
    },
  },
  watch: {
    getSearchLoading(val) {
      if (!val) {
        this.lock = false;
      }
    },
    $route(val) {
      this.inUniselect = val.path.indexOf('courses') > -1 || val.path.indexOf('university') > -1;
      this.inStudyRoomLobby = val.path.indexOf('study-rooms') > -1;
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
    ...mapActions(['updateShowBuyDialog','resetSearch',"updateLoginDialogState","toggleShowSchoolBlock","setShowSchoolBlockMobile"]),
    ...mapMutations(["UPDATE_SEARCH_LOADING", "UPDATE_LOADING"]),
    openSblToken(){
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
      } else{
        analyticsService.sb_unitedEvent("BUY_POINTS", "ENTER");
        this.updateShowBuyDialog(true);
      }
    },  
    openPersonalizeUniversity() {
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({ name: "addUniversity" });
        this.closeSideMenu()
      }
    },
    courseSelectClick(){
      !!this.accountUser ? this.selectCourse(null, true) : this.openPersonalizeCourse();
    },
    currentCourseChecker(item){
      if(item.isDefault){
        return this.selectedCourse === '';
      }else{
        return item.text ? item.text.toLowerCase() === this.selectedCourse.toLowerCase() : item === this.selectedCourse;
      }
    },
    currentPageChecker(pathName){
      if(pathName == "studyRooms") {
        return this.$route.path.indexOf('study-rooms') > -1;
      } else{
        return this.$route.path.indexOf(pathName) > -1;
      }
    },
    goTo(path){
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
        return
      }
      if(path === "profile"){
        this.$router.push({name:'profile',params:{id:this.accountUser.id,name:this.accountUser.name}})
      }
      if(path === "wallet"){
        this.$router.push({name:'wallet'})
      }
      if(path === "studyRooms"){
          this.$router.push({name:'studyRooms'})
      }
      if(path === "lessons"){
        // this.$router.push({name:'lessons'})
      }
      if(path === "posts"){
        // this.$router.push({name:'posts'})
      }
      if(path === "my-sales"){
        this.$router.push({name: 'mySales'})
      }
      if(path === "my-content"){
        this.$router.push({name: 'myContent'})
      }
      if(path === "editCourse"){
        this.$router.push({name:'editCourse'})
      }
      this.closeSideMenu();
    },
    resetItems(){
      this.resetSearch()
      this.openSideMenu();
      this.UPDATE_SEARCH_LOADING(true);
      this.$router.push('/');
        this.$nextTick(() => {
        setTimeout(()=>{
            this.UPDATE_SEARCH_LOADING(false);
        }, 200);
      });
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
        //this is required to set the current drawer state on the store, because when the 
        //created event is getting called again (during route change)
        //we need the last updated drawer state to be considered as default.
        // console.log(`drawer value is ${val}`);
        this.toggleShowSchoolBlock(val);
      
    },
    isInSearchMode(){
      return (!!this.$route.query && !!this.$route.query.term) || (!!this.$route.query && (!!this.$route.query.Filter || !!this.$route.query.Source))
    },
    selectCourse(item, isDefault) {
      if(item.isDefault){
        isDefault = true;
      }
      if(!item && isDefault){
        this.updateFilter();
        this.$router.push('/');
        return;
      }
      if((this.inUniselect || this.inStudyRoomLobby) && !item){
        this.updateFilter();
        return;
      }
      if (!this.lock) {
        this.lock = true;
        
        if(!!isDefault){
          if(!this.selectedCourse){
            if(this.isInSearchMode()){
              this.selectedCourse = "";
            }else{
              this.lock = false;
              return;
            }
          }else{
            this.selectedCourse = ""
          }
        }else{
          let text = item.text ? item.text : item;
          if (this.selectedCourse === text) {
            if(this.isInSearchMode()){
              this.selectedCourse = text;
            }else{
              this.lock = false;
              return;
            }
          } else {
            this.selectedCourse = text;
          }
        }        
        this.updateFilter();
        this.toggleMiniSideMenu()
      }
    },
    isOutsideFeed(){
        return this.$route.name !== 'feed';
    },
    updateFilter() {
      this.UPDATE_SEARCH_LOADING(true);
      this.UPDATE_LOADING(true);
      let newQueryObject = {
        Course: this.selectedCourse
      };
      if (this.selectedCourse === "") {
        delete newQueryObject.Course;
      }
      if(this.isOutsideFeed()){
          this.$router.push({path: '/feed', query: newQueryObject });
      }else{
          this.$router.push({ query: newQueryObject });
      }
    },
    openPersonalizeCourse() {
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
        return;
      }
      this.$router.push({name: 'editCourse'});
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
    // debugger
    this.sideMenulistElm = document.querySelector('.sideMenu');
    // if(this.$vuetify.breakpoint)
    // let marginTop = this.sideMenulistElm.style.marginTop;
    // marginTop = +marginTop.slice(0,marginTop.length - 2)+1;
    // this.sideMenulistElm.style.marginTop = marginTop + 'px'
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
