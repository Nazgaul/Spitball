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
import schoolBlockService from "../../services/schoolBlockService";
import editActionBlock from "./helpers/editActionBlock.vue";

export default {
  components: { editActionBlock },
  name: "schoolBlock",
  data() {
    return {
      showAllClassesBlock: false,
      selectedChips: {},
      mobileFilterState: false,
      minMode: true,
      selectedCourse: ""
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
      "accountUser"
    ]),
    classesToShow() {
      return this.$vuetify.breakpoint.smAndUp ? 5 : 3;
    },
    classesPlus() {
      if (!!this.classesList)
        return `+${this.classesList.length - this.classesToShow}`;
    },
    isLoggedIn() {
      return !!this.accountUser;
    },
    isClassesSet() {
      return this.getSelectedClasses.length > 0;
    },
    schoolName() {
      return this.getSchoolName;
    }
  },
  methods: {
    ...mapActions([
      "updateLoginDialogState",
      "updateCurrentStep",
      "changeSelectUniState"
    ]),
    ...mapMutations(["UPDATE_SEARCH_LOADING", "UPDATE_LOADING"]),
    
    selectCourse(item) {
      let text = item.text ? item.text : item;
      if (this.selectedCourse === text) {
        this.selectedCourse = "";
      } else {
        this.selectedCourse = text;
      }
    },
    updateClass(val) {
      if (!!this.selectedChips[val.text]) {
        //remove from selected chips dictionary
        val.isSelected = false;
        delete this.selectedChips[val.text];
      } else {
        //add
        val.isSelected = true;
        this.selectedChips[val.text] = true;
      }

      this.sortClassesByIsSelected(this.classesList, "isSelected");
      this.updateFilter();
      this.$forceUpdate();
    },
    updateFilter() {
      this.UPDATE_SEARCH_LOADING(true);
      this.UPDATE_LOADING(true);
      let newQueryArr = Object.keys(this.selectedChips);
      let newQueryObject = {
        Course: newQueryArr
      };
      let filter = this.$route.query.Filter;
      if (filter) {
        newQueryObject.Filter = filter;
      } else {
        delete newQueryObject.Filter;
      }
      this.$router.push({ query: newQueryObject });
    },
    sortClassesByIsSelected(arr, sortBy) {
      arr.sort(function(obj1, obj2) {
        // Ascending: first age less than the previous
        return obj2[sortBy] - obj1[sortBy];
      });
    },
    openAllClasses() {
      if (this.$vuetify.breakpoint.smAndUp) {
        this.showAllClassesBlock = true;
      } else {
        //this.classesToShow = this.classesList.length;
        this.minMode = false;
        this.mobileFilterState = true;
      }
    },
    closeAllClasses() {
      this.showAllClassesBlock = false;
    },

    openPersonalizeCourse() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.closeAllClasses();
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
  beforeMount: function() {
    if (document) {
      document.addEventListener("click", this.detectOutsideClick);
    }
  },
  beforeDestroy: function() {
    if (document) {
      document.removeEventListener("click", this.detectOutsideClick);
    }
  },
  created() {
    if (!!this.$route.query.Course) {
      let courses = [].concat(this.$route.query.Course);
      courses.forEach(courseName => {
        this.selectedChips[courseName] = true;
      });
    }
  }
};
</script>

<style lang="less" src="./schoolBlock.less"></style>
