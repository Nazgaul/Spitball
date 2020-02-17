<template>
   <v-list-group v-model="dashboardProps.model" class="sideMenu_group" active-class="''" :prepend-icon="'sbf-dashboard-sideMenu'" :append-icon="''" no-action>
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
         @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo(item.route) : dashboardProps.openSideMenu()" :sel="item.sel" v-show="dashboardProps.isShowItem(item.route)">
         <v-list-item-content> 
            <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker(item.route)}]">
            <v-icon class="group_list_icon_dash" v-html="item.icon"/>
            <span class="group_list_title_dash ml-3">{{item.name}}</span>
            </v-list-item-title>
         </v-list-item-content>
      </v-list-item>
   </v-list-group> 
</template>

<script>
import { mapGetters } from 'vuex'
export default {
   props:{
      dashboardProps:{
         required: true,
      }
   },
   data() {
      return {
         dashboardList:[
            {name: this.$t('schoolBlock_overview'), key:'dashboard', route: 'dashboardTeacher', icon:'sbf-eye', sel:'sd_dashboard', premission: 'tutor'},
            {name: this.$t('schoolBlock_profile'), key:'profile', route: 'profile', icon:'sbf-user', sel:'sd_profile', },
            {name: this.$t('schoolBlock_my_sales'), key:'my-sales', route: 'mySales', icon:'sbf-cart',sel:'sd_sales'},
            {name: this.$t('schoolBlock_my_followers'), key:'my-followers', route: 'myFollowers', icon:'sbf-follow',sel:'sd_followers'},
            {name: this.$t('schoolBlock_purchases'), key:'my-purchases', route: 'myPurchases', icon:'sbf-cart',sel:'sd_purchases'},
            {name: this.$t('schoolBlock_my_content'), key:'my-content', route: 'myContent', icon:'sbf-my-content',sel:'sd_content'},
            {name: this.$t('schoolBlock_study'), key:'studyRooms', route: 'roomSettings', icon:'sbf-studyroom-icon',sel:'sd_studyroom'},
         ],
      }
   },
   computed: {
      ...mapGetters(['accountUser']),
      dashboardListFiltered() {
         return this.dashboardList.filter(list => {return list.premission !== 'tutor' || this.isTutor})
      },
      isTutor() {
         return this.accountUser?.isTutor
      }
   },
}
</script>

<style>

</style>