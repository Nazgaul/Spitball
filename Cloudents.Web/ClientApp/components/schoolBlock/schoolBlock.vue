<template>
    <v-navigation-drawer touchless  class="school-block" width="260" @input="updateDrawerValue" :value="getShowSchoolBlock" :right="isRtl" :class="isRtl ? 'hebrew-drawer' : ''" app clipped>
      <v-list>
        <v-list-tile class="group-header search-university-title">
          <v-list-tile-action class="mr-1">
            <v-icon>sbf-uni-default</v-icon>
          </v-list-tile-action>
          <v-list-tile-title class="font-weight-regular" @click="openPersonalizeUniversity()">{{uniHeaderText}}</v-list-tile-title>
          <v-list-tile-action v-if="!schoolName" class="edit-course">
            <v-icon @click="openPersonalizeUniversity()">sbf-close</v-icon>
          </v-list-tile-action>
          <!-- <v-list-tile-action v-else class="edit-university">
            <v-icon @click="openPersonalizeUniversity()">sbf-edit-icon</v-icon>
          </v-list-tile-action> -->
        </v-list-tile>
      </v-list>
      <v-list class="class-list">
        <v-list-tile class="group-header">
          <v-list-tile-action class="mr-1">
            <v-icon>sbf-courses-icon</v-icon>
          </v-list-tile-action>
          <v-list-tile-title @click="openPersonalizeCourse()">{{coursesHeaderText}}</v-list-tile-title>
          <v-list-tile-action class="edit-course">
            <v-icon @click="openPersonalizeCourse()">sbf-close</v-icon>
          </v-list-tile-action>
        </v-list-tile>
        <v-list-tile
          class="group-items"
          :class="{'active': !selectedCourse}"
          @click="selectCourse(null, true)">
          <v-list-tile-title v-text="dictionary.allCourses"></v-list-tile-title>
        </v-list-tile>
        <v-list-tile
          class="group-items"
          v-for="(item, i) in getSelectedClasses"
          :class="{'active': item.text ? item.text === selectedCourse : item === selectedCourse}"
          :key="i"
          @click="selectCourse(item)"
        >
          <v-list-tile-title v-text="item.text ? item.text : item"></v-list-tile-title>
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
      }
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
      "getAllSteps",
      "accountUser",
      "getSearchLoading",
      "getShowSchoolBlock"
    ]),
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
    updateFilter() {
      this.UPDATE_SEARCH_LOADING(true);
      this.UPDATE_LOADING(true);
      let newQueryObject = {
        Course: this.selectedCourse
      };
      if (this.selectedCourse === "") {
        delete newQueryObject.Course;
      }
      
      this.$router.push({ query: newQueryObject });
    },

    openPersonalizeCourse() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        let steps = this.getAllSteps;
          this.$router.push({name: 'editCourse'});
      }
    },
    openPersonalizeUniversity() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        let steps = this.getAllSteps;
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
