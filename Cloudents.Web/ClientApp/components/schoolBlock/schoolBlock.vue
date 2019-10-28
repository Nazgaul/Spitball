<template>
    <v-navigation-drawer touchless
                         class="school-block"
                         width="260"
                         :value="getShowSchoolBlock"
                         @input="updateDrawerValue"
                         :right="isRtl" :class="isRtl ? 'hebrew-drawer' : ''" app clipped>
      <v-list>
        <v-list-tile class="group-header search-university-title pl-1"
                     @click.native.prevent="openStudyRooms()"
                     :class="{'active': inStudyRoomLobby}">
          <v-list-tile-action>
            <v-icon>sbf-studyroom-icon</v-icon>
          </v-list-tile-action>
          <v-list-tile-title v-text="dictionary.myStudyRooms" class="pl-1"></v-list-tile-title>
        </v-list-tile>
      </v-list>
      <v-list class="class-list">
        <v-list-tile class="group-header cursor-pointer" :class="{'active': !selectedCourse && !inStudyRoomLobby}">
          <v-list-tile-action class="ml-1 mr-1">
            <v-icon>sbf-courses-icon</v-icon>
          </v-list-tile-action>
          <v-list-tile-title @click="accountUser ? selectCourse(null, true) : openPersonalizeCourse()"
                             v-text="accountUser ? dictionary.allCourses : dictionary.addcourses"></v-list-tile-title>
          <v-list-tile-action class="edit-course px-3" @click="openPersonalizeCourse()">
            <v-icon>sbf-close</v-icon>
          </v-list-tile-action>
        </v-list-tile>
        <v-list-tile
          class="group-items"
          :to="{name: $route.name}"
          v-for="(item, i) in selectedClasses"
          :class="{'active': item.text ? item.text.toLowerCase() === selectedCourse.toLowerCase() : item === selectedCourse}"
          :key="i"
          event
          @click.native.prevent="selectCourse(item)">
          <v-list-tile-title v-text="item.text ? item.text : item" class="pad-left"></v-list-tile-title>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
</template>

<script>
import { mapGetters, mapActions, mapMutations } from "vuex";
import {LanguageService} from "../../services/language/languageService"

export default {
  name: "schoolBlock",
  data() {
    return {
      selectedCourse: "",
      lock: false,
      isRtl: global.isRtl,
      dictionary:{
        addUniversity: LanguageService.getValueByKey('schoolBlock_add_your_university'),
        addcourses: LanguageService.getValueByKey('schoolBlock_add_your_courses'),
        myCourses: LanguageService.getValueByKey('schoolBlock_my_courses'),
        allCourses: LanguageService.getValueByKey('schoolBlock_all_courses'),
        myStudyRooms: LanguageService.getValueByKey('schoolBlock_my_study_rooms'),
      },
      inUniselect: this.$route.path.indexOf('courses') > -1 || this.$route.path.indexOf('university') > -1,
      inStudyRoomLobby: this.$route.path.indexOf('study-rooms') > -1
    };
  },
  props: {
    isDisabled: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    ...mapGetters([
      "getSelectedClasses",
      "getSchoolName",
      "accountUser",
      "getSearchLoading",
      "getShowSchoolBlock",
      "accountUser"
    ]),
    selectedClasses(){
      let selectedClasses = JSON.parse(JSON.stringify(this.getSelectedClasses))
      return selectedClasses.sort((a, b) => a.text.toLowerCase() > b.text.toLowerCase() ? 1 : -1)
    },
    schoolName() {
      return this.getSchoolName;
    },
    uniHeaderText(){
      if(!!this.schoolName){
        return this.schoolName;
      }else{
        return this.dictionary.addUniversity;
      }
    },
    coursesHeaderText(){
      if(this.getSelectedClasses.length > 0){
        return this.dictionary.myCourses;
      }else{
        return this.dictionary.addcourses;
      }
    },
    isLoggedIn() {
      return !!this.accountUser;
    }
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
    ...mapActions([
      "updateLoginDialogState","toggleShowSchoolBlock"
    ]),
    ...mapMutations(["UPDATE_SEARCH_LOADING", "UPDATE_LOADING"]),
    updateDrawerValue(val){
        //this is required to set the current drawer state on the store, because when the 
        //created event is getting called again (during route change)
        //we need the last updated drawer state to be considered as default.
        console.log(`drawer value is ${val}`);
        this.toggleShowSchoolBlock(val);
      
    },
    isInSearchMode(){
      return (!!this.$route.query && !!this.$route.query.term) || (!!this.$route.query && (!!this.$route.query.Filter || !!this.$route.query.Source))
    },
    selectCourse(item, isDefault) {
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
      }
    },
    isOutsideNoteAsk(){
        return this.$route.name !== 'ask' && this.$route.name !== 'note' && this.$route.name !== 'tutors'
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
      if(this.isOutsideNoteAsk()){
          this.$router.push({name: 'feed', query: newQueryObject });
      }else{
          this.$router.push({ query: newQueryObject });
      }
    },
    openStudyRooms(){
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({
            name:'studyRooms',
        })
      }
    },
    openPersonalizeCourse() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({name: 'editCourse'});
      }
    },
    openPersonalizeUniversity() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({name: 'addUniversity'})
      }
    }
  },
  created() {
    if (!!this.$route.query.Course) {
      this.selectedCourse = this.$route.query.Course;
    }
  }
};
</script>

<style lang="less" src="./schoolBlock.less"></style>
