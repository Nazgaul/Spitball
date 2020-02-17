<template>
   <v-list-group v-model="coursesProps.coursesModel" active-class="''" :prepend-icon="'sbf-courses-icon'" :append-icon="''" no-action class="sideMenu_group">
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
      @click.native.prevent="coursesProps.getShowSchoolBlock ? selectCourse(item) : coursesProps.openSideMenu()" :sel="item.isDefault? 'all_courses' : ''">
      <v-list-item-content>
         <v-list-item-title :class="['group_list_titles_course',{'active_link_course': currentCourseChecker(item)}]">
         <arrowSVG v-if="currentCourseChecker(item)" class="arrow_course"/>
         <span :class="['group_list_title_course text-truncate',currentCourseChecker(item)? 'padding_current_course':'ml-4']" v-text="item.text ? item.text : $t('schoolBlock_allCourses')"/>
         </v-list-item-title>
      </v-list-item-content>
   </v-list-item>
   </v-list-group>
</template>

<script>
import arrowSVG from './image/left-errow.svg';
import { mapGetters } from 'vuex';

export default {
   components:{arrowSVG},
   props:{
      coursesProps:{
         required:true
      }
   },
   data() {
      return {
         selectedCourse: "",
      }
   },
   computed: {
      ...mapGetters(['getSelectedClasses','Feeds_getIsLoading']),
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
   methods: {
      currentCourseChecker(item){
         if(item.isDefault){
            return this.selectedCourse === '';
         }else{
            return item.text.toLowerCase() === this.selectedCourse.toLowerCase();
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
         this.coursesProps.toggleMiniSideMenu()
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
   },
   watch: {
      $route() {
         if (!!this.$route.query) {
         if (!this.$route.query.Course) {
            this.selectedCourse = "";
         }
         } else {
         this.selectedCourse = "";
         }
      }
   },
   created() {
   if (!!this.$route.query.Course) {
      this.selectedCourse = this.$route.query.Course;
    }
   },

}
</script>

<style>

</style>