<template>
   <v-list-group v-if="!hideDashboardGroup" @click="dashboardProps.openSideMenu()" v-model="dashboardProps.model" class="sideMenu_group" active-class="''" :prepend-icon="'sbf-dashboard-sideMenu'" :append-icon="''" no-action>
      <template v-slot:activator>
         <v-list-item sel="sidemenu_dashboard" class="sideMenu_list">
            <v-list-item-content>
            <v-list-item-title>
               <span class="sideMenu_list_title" v-language:inner="'schoolBlock_dashboard'"/>
               </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>

      <template v-if="showOverview">
         <sideMenuListItem sel="sidemenu_dashboard_overview" :dashboardProps="dashboardProps" :item="overviewItem"/>
      </template>

      <template v-if="showOverview">
         <sideMenuListItem sel="sidemenu_dashboard_opportunities" :dashboardProps="dashboardProps" :item="myQuestionsItem"/>
      </template>

      <template v-if="showMyContent">
         <sideMenuListItem sel="sidemenu_dashboard_myContent" :dashboardProps="dashboardProps" :item="myContentItem"/>
      </template>

      <sideMenuListItem sel="sidemenu_dashboard_myStudyRooms" :dashboardProps="dashboardProps" :item="myStudyRoomsItem"/>
      
      <template v-if="showMySales">
         <sideMenuListItem sel="sidemenu_dashboard_mySales" :dashboardProps="dashboardProps" :item="mySalesItem"/>
      </template>

      <template v-if="showMyFollowers">
         <sideMenuListItem sel="sidemenu_dashboard_myFollowers" :dashboardProps="dashboardProps" :item="myFollowersItem"/>
      </template>

      <template v-if="showMyPurchases">
         <sideMenuListItem sel="sidemenu_dashboard_myPurchases" :dashboardProps="dashboardProps" :item="myPurchasesItem"/>
      </template>

   </v-list-group> 
</template>

<script>
import { mapGetters } from 'vuex';
import sideMenuListItem from './sideMenuListItem.vue';
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   props:{
      dashboardProps:{
         required: true,
      }
   },
   components:{sideMenuListItem},
   data() {
      return {
         overviewItem:{name: this.$t('schoolBlock_overview'),route: routeNames.Dashboard, icon:'sbf-eye', sel:'sd_dashboard'},
         mySalesItem:{name: this.$t('schoolBlock_my_sales'),route: routeNames.MySales, icon:'sbf-my-sales', sel:'sd_sales'},
         myFollowersItem:{name: this.$t('schoolBlock_my_followers'),route: routeNames.MyFollowers, icon:'sbf-follow', sel:'sd_followers'},
         myPurchasesItem:{name: this.$t('schoolBlock_purchases'),route: routeNames.MyPurchases, icon:'sbf-cart', sel:'sd_purchases'},
         myContentItem:{name: this.$t('schoolBlock_my_content'),route: routeNames.MyContent, icon:'sbf-my-content', sel:'sd_content'},
         myStudyRoomsItem:{name: this.$t('schoolBlock_study'),route: routeNames.MyStudyRooms, icon:'sbf-studyroom-icon', sel:'sd_studyroom'},
         myQuestionsItem:{name: this.$t('schoolBlock_my_Questions'),route:'myQuestions',icon:'sbf-my-questions',sel:'sd_questions'}
      }
   },
   computed: {
      ...mapGetters(['accountUser', 'getIsTeacher']),
      showOverview(){
         return this.getIsTeacher;
      },
      showMySales(){
         return this.accountUser.isSold;
      },
      showMyFollowers(){
         return this.accountUser.haveFollowers;
      },
      showMyContent(){
         return this.accountUser.haveContent;
      },
      showMyPurchases(){
         return this.accountUser.isPurchased;
      },
      hideDashboardGroup(){
         let itemList = [this.showOverview,this.showMySales,this.showMyFollowers,this.showMyContent,this.showMyPurchases]
         return itemList.every(item=>(!item))
      }
   },
}
</script>