<template>
   <v-list-item class="group_list_sideMenu_dash" @click="$emit('goTo', item.route)" :sel="item.sel">
      <v-list-item-content> 
         <v-list-item-title class="group_list_titles_dash" :class="{'active_list_dash': currentPageChecker(item.route)}">
            <v-icon size="18" class="group_list_icon_dash">{{item.icon}}</v-icon>
            <span class="group_list_title_dash ml-3" v-t="item.name"></span>
         </v-list-item-title>
      </v-list-item-content>
   </v-list-item>
</template>

<script>
//TODO: DELETE FILE

// import * as routeNames from '../../../../routes/routeNames.js';
// import * as feedFilters from '../../../../routes/consts/feedFilters.js';
// import { mapGetters } from 'vuex';
export default {
   props:{
      item:{
         required:true,
      },
      currentPageChecker: {
         type: Function,
         required: true
      }
   },
   // computed: {
      // ...mapGetters(['getShowSchoolBlock']),
   // },
   // methods: {
   //    currentPageChecker(pathName){
   //       let isMyQuestions = (this.$route.name === routeNames.Feed && this.$route.query?.filter === feedFilters.Question)
   //       if(pathName === 'myQuestions' && isMyQuestions){
   //          return true;
   //       }
   //       if(pathName.toLowerCase().includes('course') && this.$route.path.includes('courses')){
   //          return true;
   //       }
   //       if(this.$route.name === pathName){
   //          return true;
   //       }
   //    }
   // },
}
</script>