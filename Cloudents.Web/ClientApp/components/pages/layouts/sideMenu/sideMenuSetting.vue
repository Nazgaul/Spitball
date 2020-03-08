<template>
   <v-list-group @click="settingProps.openSideMenu()" v-model="settingProps.model" class="sideMenu_group" active-class="''" :prepend-icon="'sbf-settings'" :append-icon="''" no-action>
      <template v-slot:activator>
         <v-list-item class="sideMenu_list">
            <v-list-item-content>
            <v-list-item-title>
               <span class="sideMenu_list_title" v-language:inner="'schoolBlock_settings'"/>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>
       <sideMenuListItem :dashboardProps="settingProps" :item="myProfileItem"/>
      <sideMenuListItem :dashboardProps="settingProps" :item="myUniversityItem"/>
      <sideMenuListItem :dashboardProps="settingProps" :item="myCoursesItem"/>
      <template v-if="isTutor">
         <sideMenuListItem :dashboardProps="settingProps" :item="myCalendarItem"/>
         <sideMenuListItem :dashboardProps="settingProps" :item="testStudyRoomItem"/>
      </template>
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
         myProfileItem:{name: this.$t('schoolBlock_profile'),route: routeNames.Profile, icon:'sbf-user',sel:'sd_profile'},
         myUniversityItem:{name: this.$t('menuList_changeUniversity'),route: routeNames.EditUniversity, icon:'sbf-university',sel:'sd_studyroom'},
         myCoursesItem:{name: this.$t('schoolBlock_courses'),route: routeNames.EditCourse, icon:'sbf-classes-icon', sel:'sd_edit_course'},
         myCalendarItem:{name: this.$t('schoolBlock_calendar'),route: routeNames.MyCalendar, icon:'sbf-calendar', sel:'sd_calendar'},
         testStudyRoomItem:{name: this.$t('menuList_my_study_rooms'),route: routeNames.StudyRoom.name, icon:'sbf-pc',sel:'menu_row'},
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
   },
   mounted() {
      this.myProfileItem.name = this.isTutor? this.$t('schoolBlock_my_site') : this.$t('schoolBlock_profile')
   },
}
</script>