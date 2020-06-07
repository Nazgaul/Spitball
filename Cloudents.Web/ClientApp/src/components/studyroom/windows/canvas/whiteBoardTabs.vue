<template>
   <div>
      <button v-for="(tab,index) in canvasTabs" 
              @click="changeTab(tab)" :key="tab.id" :sel="`tab${index+1}`"
              :class="['tabBtns',{'activeTabBtn':getCurrentSelectedTab.id == tab.id},{'disableClick':!isRoomTutor}]">
         {{tab.name}}
      </button>
   </div>
   <!-- <v-btn-toggle group dense borderless color="#4c59ff" background-color="#00FF00" mandatory>
         {{getCurrentSelectedTab}}
      <template v-for="(tab,index) in ">
         <v-btn :input-value="getCurrentSelectedTab.id == tab.id" @click="changeTab(tab)" :key="tab.id" :sel="`tab${index+1}`">
            {{tab.name}}
         </v-btn>
      </template>
   </v-btn-toggle> -->
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import whiteBoardService from '../../whiteboard/whiteBoardService.js';

export default {
   data() {
      return {
         canvasTabs: [
            {
                  name: this.$t('tutor_tab') + ' 1',
                  id: 'tab-0'
            },
            {
                  name: this.$t('tutor_tab') + ' 2',
                  id: 'tab-1'
            },
            {
                  name: this.$t('tutor_tab') + ' 3',
                  id: 'tab-2'
            },
            {
                  name: this.$t('tutor_tab') + ' 4',
                  id: 'tab-3'
            },
            {
                  name: this.$t('tutor_tab') + ' 5',
                  id: 'tab-4'
            },
            {
                  name: this.$t('tutor_tab') + ' 6',
                  id: 'tab-5'
            },
            {
                  name: this.$t('tutor_tab') + ' 7',
                  id: 'tab-6'
            },
            {
                  name: this.$t('tutor_tab') + ' 8',
                  id: 'tab-7'
            },
         ],
      }
   },
   computed: {
      ...mapGetters(['getCurrentSelectedTab']),
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
   created() {
      if(!this.getCurrentSelectedTab.name){
         let tab = {
            name: this.$t('tutor_tab') + ' 1',
            id: 'tab-0'
         }
         this.$store.dispatch('changeSelectedTab',tab)
      }  
   },
}
</script>

<style lang="less">
.tabBtns{
   min-width: 88px;
   height: 30px;
   font-size: 14px;
   border-right: 1px solid white;
   background-color: #e0e0e1;
   font-weight: 600;
   color: #7a798c;
   outline: none;
   &.activeTabBtn{
      background-color: white ;
      font-weight: bold;
      color: #4c59ff;
   }
   &.disableClick{
      pointer-events: none;
   }
}
</style>