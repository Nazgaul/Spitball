<template>
   <div class="studyRoomWrapper" style="height:100%">
      <component :is="activeWindow" style="height:100%"></component>
      <floatingVideoContainer v-if="!isRoomTutor"/>
   </div>
</template>

<script>
const canvasWrap = () => import("./canvas/canvasWrap.vue");
const codeEditor = () => import("../codeEditor/codeEditor.vue");
const sharedDocument = () => import("../sharedDocument/sharedDocument.vue");
const classMode = () => import('./class/classRoom.vue');
const classScreen = () => import('./class/classFullScreen.vue');
const screenMode = () => import('./shareScreen/shareScreen.vue');
const floatingVideoContainer = () => import('../layouts/studyRoomDrawer/floatingVideoContainer.vue');
export default {
   components:{
      canvasWrap,
      codeEditor,
      sharedDocument,
      classMode,
      classScreen,
      screenMode,
      floatingVideoContainer,
   },
   computed: {
      activeWindow(){
         if(this.$store.getters.getActiveNavEditor === 'white-board'){
         return 'canvasWrap'
         }
         return this.$store.getters.getActiveNavEditor
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      }
   },
}
</script>
<style lang="less">
.studyRoomWrapper{
   position: relative;
}
</style>
