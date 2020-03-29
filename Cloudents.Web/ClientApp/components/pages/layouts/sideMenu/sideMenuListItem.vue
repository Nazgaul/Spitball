<template>
   <v-list-item class="group_list_sideMenu_dash" :to="{name: item.route}" event :sel="item.sel"
      @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo(item.route) : dashboardProps.openSideMenu()">
      <v-list-item-content> 
         <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':currentPageChecker(item.route)}]">
            <v-icon size="18" class="group_list_icon_dash">{{item.icon}}</v-icon>
            <span class="group_list_title_dash ml-3">{{item.name}}</span>
         </v-list-item-title>
      </v-list-item-content>
   </v-list-item>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames.js';
import * as feedFilters from '../../../../routes/consts/feedFilters.js';
export default {
   props:{
      dashboardProps:{
         required:true,
      },
      item:{
         required:true,
      }
   },
   methods: {
      currentPageChecker(pathName){
         let isMyQuestions = (this.$route.name === routeNames.Feed && this.$route.query?.filter === feedFilters.Question)
         if(pathName === 'myQuestions' && isMyQuestions){
            return true;
         }
         if(pathName.toLowerCase().includes('course') && this.$route.path.includes('courses')){
            return true;
         }
         if(this.$route.name === pathName){
            return true;
         }
      },
   },
}
</script>