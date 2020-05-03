<template>
  <div class="studyRoom">
    <studyRoomDrawer/>
    <studyRoomHeader/>
    <v-content>
      <component style="height:100%" :is="activeWindow"></component>
    </v-content>
    <studyRoomFooter/>
  </div>
</template>

<script>
const studyRoomDrawer = () => import('./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import('./layouts/studyRoomFooter/studyRoomFooter.vue');
import studyRoomHeader from './layouts/studyRoomHeader/studyRoomHeader.vue';

const canvasWrap = () => import("./windows/canvas/canvasWrap.vue");
const codeEditor = () => import("./codeEditor/codeEditor.vue");
const sharedDocument = () => import("./sharedDocument/sharedDocument.vue");
export default {
  components: {
    studyRoomDrawer,
    studyRoomFooter,
    studyRoomHeader,

    canvasWrap,
    codeEditor,
    sharedDocument,
  },
  computed: {
    activeWindow(){
      if(this.$store.getters.getActiveNavEditor === 'white-board'){
        return 'canvasWrap'
      }else{
        return this.$store.getters.getActiveNavEditor
      }
    }
  },
};
</script>

<style lang="less">
.studyRoom {
}
</style>