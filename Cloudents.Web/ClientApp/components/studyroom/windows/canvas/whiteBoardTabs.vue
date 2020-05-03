<template>
   <v-btn-toggle :value="getCurrentSelectedTab" group dense borderless color="#4c59ff" background-color="#00FF00" mandatory>
      <template v-for="(tab,index) in getCanvasTabs">
         <v-btn @click="changeTab(tab)" :key="tab.id" :sel="`tab${index+1}`">
            {{tab.name}}
         </v-btn>
      </template>
   </v-btn-toggle>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import whiteBoardService from '../../whiteboard/whiteBoardService.js';

export default {
   computed: {
      ...mapGetters(['getCanvasTabs','getCurrentSelectedTab']),
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      }
   },
   methods: {
      ...mapActions(['changeSelectedTab']),
      changeTab(tab) {
         if(!this.$route.params.id || this.$route.params.id && this.isRoomTutor ){
            let canvas = this.$store.getters.getTempCanvasStore
            this.$ga.event("tutoringRoom", `changeTab:${tab}`);
            if (tab.id !== this.getCurrentSelectedTab.id) {
               let transferDataObj = {
                  type: "updateTabById",
                  data: {
                        tab,
                        canvas:canvas
                  }
               };
               let normalizedData = JSON.stringify(transferDataObj);
               this.$store.dispatch('sendDataTrack',normalizedData)
               this.changeSelectedTab(tab);
               whiteBoardService.hideHelper();
               whiteBoardService.redraw(canvas);
            }
         }
      },
   },

}
</script>

<style lang="less">
.whiteBoardTabs{

}
</style>