<template>
   <div class="studyRoomWrapper">
      <component :is="activeWindow" style="height:100%"></component>
      <floatingVideoContainer v-if="!isRoomTutor"/>
      <studyRoomDialogs/>
      <studyRoomSettingsDialog v-if="!isRoomActive"/>
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
const studyRoomDialogs = () => import('../studyRoomDialogs.vue');
const studyRoomSettingsDialog = () => import("../tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue");
export default {
   components:{
      canvasWrap,
      codeEditor,
      sharedDocument,
      classMode,
      classScreen,
      screenMode,
      floatingVideoContainer,
      studyRoomDialogs,
      studyRoomSettingsDialog,
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
      },
      isRoomActive(){
         return this.$store.getters.getRoomIsActive;
      },
   },
}
</script>
<style lang="less">
.studyRoomWrapper{
   position: relative;
}
</style>
