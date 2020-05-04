<template>
  <div class="studyRoom">
    <studyRoomDrawer/>
    <studyRoomHeader/>
    <v-content>
      <component style="height:100%" :is="activeWindow"></component>
    </v-content>
    <studyRoomFooter v-if="isShowFooter"/>
  </div>
</template>

<script>
const studyRoomDrawer = () => import('./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import('./layouts/studyRoomFooter/studyRoomFooter.vue');
import studyRoomHeader from './layouts/studyRoomHeader/studyRoomHeader.vue';

const canvasWrap = () => import("./windows/canvas/canvasWrap.vue");
const codeEditor = () => import("./codeEditor/codeEditor.vue");
const sharedDocument = () => import("./sharedDocument/sharedDocument.vue");
const classMode = () => import('./windows/class/classRoom.vue');
const classScreen = () => import('./windows/class/classFullScreen.vue');
export default {
  components: {
    studyRoomDrawer,
    studyRoomFooter,
    studyRoomHeader,

    canvasWrap,
    codeEditor,
    sharedDocument,
    classMode,
    classScreen
  },
  computed: {
    activeWindow(){
      if(this.$store.getters.getActiveNavEditor === 'white-board'){
        return 'canvasWrap'
      }
      return this.$store.getters.getActiveNavEditor
    },
    isShowFooter(){
      let currentEditor = this.$store.getters.getActiveNavEditor 
      return currentEditor !== 'class-mode' && currentEditor !== 'class-screen'
    }
  },
};
</script>