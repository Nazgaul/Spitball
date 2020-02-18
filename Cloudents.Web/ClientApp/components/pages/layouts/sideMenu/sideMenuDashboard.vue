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

      <template v-if="showOverview">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'dashboardTeacher'}" event sel="sd_dashboard"
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('dashboardTeacher') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('dashboardTeacher')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-eye'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_overview')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

<!-- PROFILE -->
      <v-list-item class="group_list_sideMenu_dash" :to="{name: 'profile'}" event sel="sd_profile"
         @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('profile') : dashboardProps.openSideMenu()">
         <v-list-item-content> 
            <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('profile')}]">
            <v-icon class="group_list_icon_dash" v-html="'sbf-user'"/>
            <span class="group_list_title_dash ml-3">{{$t('schoolBlock_profile')}}</span>
            </v-list-item-title>
         </v-list-item-content>
      </v-list-item>
<!-- PROFILE -->
      <template v-if="showMySales">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'mySales'}" event sel="sd_sales"
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('mySales') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('mySales')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-cart'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_my_sales')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <template v-if="showMyFollowers">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'myFollowers'}" event sel="sd_followers"
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('myFollowers') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('myFollowers')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-follow'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_my_followers')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <template v-if="showMyPurchases">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'myPurchases'}" event sel="sd_purchases"
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('myPurchases') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('myPurchases')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-cart'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_purchases')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <template v-if="showMyContent">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'myContent'}" event sel="sd_content" 
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('myContent') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('myContent')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-my-content'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_my_content')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <template v-if="showMyStudyRooms">
         <v-list-item class="group_list_sideMenu_dash" :to="{name: 'roomSettings'}" event sel="sd_studyroom"
            @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo('roomSettings') : dashboardProps.openSideMenu()">
            <v-list-item-content> 
               <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker('roomSettings')}]">
               <v-icon class="group_list_icon_dash" v-html="'sbf-studyroom-icon'"/>
               <span class="group_list_title_dash ml-3">{{$t('schoolBlock_study')}}</span>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <!-- <v-list-item class="group_list_sideMenu_dash" v-for="(item, index) in dashboardListFiltered" :key="index"
         :to="{name: item.route}"
         event
         @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo(item.route) : dashboardProps.openSideMenu()" :sel="item.sel" v-show="dashboardProps.isShowItem(item.route)">
         <v-list-item-content> 
            <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker(item.route)}]">
            <v-icon class="group_list_icon_dash" v-html="item.icon"/>
            <span class="group_list_title_dash ml-3">{{item.name}}</span>
            </v-list-item-title>
         </v-list-item-content>
      </v-list-item> -->
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
         // dashboardList:[
         //    {name: this.$t('schoolBlock_overview'), key:'dashboard', route: 'dashboardTeacher', icon:'sbf-eye', sel:'sd_dashboard', premission: 'tutor'},
         //    {name: this.$t('schoolBlock_profile'), key:'profile', route: 'profile', icon:'sbf-user', sel:'sd_profile', },
         //    {name: this.$t('schoolBlock_my_sales'), key:'my-sales', route: 'mySales', icon:'sbf-cart',sel:'sd_sales'},
         //    {name: this.$t('schoolBlock_my_followers'), key:'my-followers', route: 'myFollowers', icon:'sbf-follow',sel:'sd_followers'},
         //    {name: this.$t('schoolBlock_purchases'), key:'my-purchases', route: 'myPurchases', icon:'sbf-cart',sel:'sd_purchases'},
         //    {name: this.$t('schoolBlock_my_content'), key:'my-content', route: 'myContent', icon:'sbf-my-content',sel:'sd_content'},
         //    {name: this.$t('schoolBlock_study'), key:'studyRooms', route: 'roomSettings', icon:'sbf-studyroom-icon',sel:'sd_studyroom'},
         // ],
      }
   },
   computed: {
      ...mapGetters(['accountUser']),
      dashboardListFiltered() {
         return this.dashboardList.filter(list => {return list.premission !== 'tutor' || this.isTutor})
      },
      isTutor() {
         return this.accountUser?.isTutor
      },
      showOverview(){
         return this.accountUser?.userType === 'Teacher';
      },
      showMySales(){
         return this.accountUser?.isSold;
      },
      showMyFollowers(){
         return this.accountUser?.haveFollowers;
      },
      showMyPurchases(){
         return this.accountUser?.isPurchased;
      },
      showMyContent(){
         return this.accountUser?.haveDocs;
      },
      showMyStudyRooms(){
         return this.accountUser?.haveStudyRoom;
      },
   },
}
</script>

<style>

</style>