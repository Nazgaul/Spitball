<template>
   <v-list-item class="group_list_sideMenu_dash" :to="{name: item.route}" event :sel="item.sel"
      @click.native.prevent="dashboardProps.showSchoolBlock ? dashboardProps.goTo(item.route) : dashboardProps.openSideMenu()">
      <v-list-item-content> 
         <v-list-item-title :class="['group_list_titles_dash',{'active_list_dash':dashboardProps.currentPageChecker(item.route)}]">
            <v-icon class="group_list_icon_dash">{{item.icon}}</v-icon>
            <span class="group_list_title_dash ml-3">{{item.name}}</span>
         </v-list-item-title>
      </v-list-item-content>
   </v-list-item>
</template>

<script>
export default {
   props:{
      dashboardProps:{
         required:true,
      },
      item:{
         required:true,
      }
   }
}
</script>