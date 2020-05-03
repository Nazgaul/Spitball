<template>
   <v-navigation-drawer app right clipped class="studyRoomDrawer" :width="drawerExtend ? 300 : 0">
   <v-btn icon class="collapseIcon" @click="drawerExtend = !drawerExtend" color="#fff">
      <v-icon v-text="drawerExtend? 'sbf-arrow-right-carousel': 'sbf-arrow-left-carousel'"></v-icon>
   </v-btn>

<!--Video element-->
   <v-card :color="'grey lighten-1'" class="ma-2" height="210" width="276"></v-card>
<!--chat element-->
   <v-sheet height="100%" width="100%" color="green">
      <div>Chat location</div>
   </v-sheet>
   </v-navigation-drawer>
</template>

<script>
export default {
   data() {
      return {
        drawerExtend:true, 
      }
   },
}
</script>

<style lang="less">
   .studyRoomDrawer {
      overflow: initial; // to let the collapse btn to show
      // position: relative;
      .collapseIcon {
         position: absolute;
         top: 20px;
         left: -35px;
         background: #212123;
         border-radius: 0%; //vuetify override
      }
   }
</style>