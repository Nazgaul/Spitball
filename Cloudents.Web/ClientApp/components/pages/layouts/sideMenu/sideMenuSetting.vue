<template>
   <v-list-group v-model="settingProps.model" class="sideMenu_group" active-class="''" :prepend-icon="'sbf-settings'" :append-icon="''" no-action>
      <template v-slot:activator>
         <v-list-item class="sideMenu_list">
            <v-list-item-content>
            <v-list-item-title>
               <span class="sideMenu_list_title" v-language:inner="'schoolBlock_settings'"/>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>
      <template v-if="isTutor">
         <sideMenuListItem :dashboardProps="settingProps" :item="myCalendarItem"/>
         <sideMenuListItem :dashboardProps="settingProps" :item="testStudyRoomItem"/>
      </template>
      <sideMenuListItem :dashboardProps="settingProps" :item="myUniversityItem"/>
      <sideMenuListItem :dashboardProps="settingProps" :item="myCoursesItem"/>
   </v-list-group>
</template>

<script>
import { mapGetters } from 'vuex';
import sideMenuListItem from './sideMenuListItem.vue';
import * as routeNames from '../../../../routes/routeNames.js';
export default {
   components:{sideMenuListItem},
   data() {
      return {
         myCoursesItem:{name: this.$t('schoolBlock_courses'),route: routeNames.EditCourse, icon:'sbf-classes-icon', sel:'sd_edit_course'},
         myCalendarItem:{name: this.$t('schoolBlock_calendar'),route: routeNames.MyCalendar, icon:'sbf-calendar', sel:'sd_calendar'},
         testStudyRoomItem:{name: this.$t('menuList_my_study_rooms'),route: routeNames.StudyRoom, icon:'sbf-pc',sel:'menu_row'},
         myUniversityItem:{name: this.$t('menuList_changeUniversity'),route: routeNames.EditUniversity, icon:'sbf-university',sel:'sd_studyroom'},
      }
   },
   props:{
      settingProps:{
         required: true,
      }
   },
   computed: {
      ...mapGetters(['accountUser']),
      isTutor(){
         return this.accountUser?.isTutor
      }
   }
}
</script>