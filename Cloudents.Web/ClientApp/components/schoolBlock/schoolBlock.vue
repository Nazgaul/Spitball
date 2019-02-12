<template>
  <v-navigation-drawer stateless permanent class="school-block">
    <v-list>
        <v-list-tile class="group-header">
          <v-list-tile-action>
            <v-icon>sbf-university-columns</v-icon>
          </v-list-tile-action>
          <v-list-tile-title @click="openPersonalizeUniversity()">{{schoolName}}</v-list-tile-title>
        </v-list-tile>
    </v-list>
    <v-list>
        <v-list-tile class="group-header">
          <v-list-tile-action>
            <v-icon>sbf-courses-icon</v-icon>
          </v-list-tile-action>
          <v-list-tile-title @click="openPersonalizeCourse()">My Courses</v-list-tile-title>
          <v-list-tile-action class="edit-course">
            <v-icon @click="openPersonalizeCourse()">sbf-close</v-icon>
          </v-list-tile-action>
        </v-list-tile>
        <v-list-tile class="group-items"
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

export default {
  name: "schoolBlock",
  data() {
    return {
      selectedCourse: "",
      lock: false,
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
      "getSearchLoading"
    ]),
    isLoggedIn() {
      return !!this.accountUser;
    },
    schoolName() {
      return this.getSchoolName;
    }
  },
  watch:{
    getSearchLoading(val){
      if(!val){
        this.lock = false;
      }
    },
    '$route'(val) {
        if(!!this.$route.query){
          if(!this.$route.query.Course){
            this.selectedCourse = "";
          }
        }else{
          this.selectedCourse = "";
        }
     },
  },
  methods: {
    ...mapActions([
      "updateLoginDialogState",
      "updateCurrentStep",
      "changeSelectUniState"
    ]),
    ...mapMutations(["UPDATE_SEARCH_LOADING", "UPDATE_LOADING"]),
    
    selectCourse(item) {
      if(!this.lock){
        this.lock = true;      
        let text = item.text ? item.text : item;
        if (this.selectedCourse === text) {
          this.selectedCourse = "";
        } else {
          this.selectedCourse = text;
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
      if(this.selectedCourse === ""){
        delete newQueryObject.Course;
      }
      let filter = this.$route.query.Filter;
      if (filter) {
        newQueryObject.Filter = filter;
      } else {
        delete newQueryObject.Filter;
      }
      this.$router.push({ query: newQueryObject });
    },

    openPersonalizeCourse() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        let steps = this.getAllSteps;
        this.updateCurrentStep(steps.set_class);
        this.changeSelectUniState(true);
      }
    },
    openPersonalizeUniversity() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        let steps = this.getAllSteps;
        this.updateCurrentStep(steps.set_school);
        this.changeSelectUniState(true);
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
