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
      <v-list-item class="group_list_sideMenu_dash" v-for="(item, index) in settingList" :key="index"
         :to="{name: item.route}"
         event
         @click.native.prevent="settingProps.showSchoolBlock ? settingProps.goTo(item.route) : settingProps.openSideMenu()" :sel="item.sel" v-show="settingProps.isShowItem(item.route)">
         <v-list-item-content> 
            <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':settingProps.currentPageChecker(item.route)}]">
            <v-icon class="group_list_icon_dash" v-html="item.icon"/>
            <span class="group_list_title_dash ml-3">{{item.name}}</span>
            </v-list-item-title>
         </v-list-item-content>
      </v-list-item>
   </v-list-group>
</template>

<script>
export default {
   data() {
      return {
         settingList:[
            {name: this.$t('schoolBlock_courses'), key:'courses', route: 'editCourse', icon:'sbf-classes-icon'},
            {name: this.$t('schoolBlock_calendar'), key:'my-calendar',route: 'myCalendar', icon:'sbf-calendar',sel:'sd_calendar'},
            {name: this.$t('menuList_my_study_rooms'), key:'tutoring', route: 'tutoring', icon:'sbf-pc',sel:'menu_row'},
            {name: this.$t('menuList_changeUniversity'), key:'university', route: 'addUniversity', icon:'sbf-university',sel:'sd_studyroom'},
         ]
      }
   },
   props:{
      settingProps:{
         required: true,
      }
   }

}
</script>

<style lang="less">

</style>