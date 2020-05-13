<template>
  <div class="studyRoom">
    <template v-if="isMobile">
      <studyRoomMobile/>
    </template>
    <template v-else>
      <studyRoomDrawer @drawerExtendChanged="(val)=> isDrawerExtendReference = val"/>
      <studyRoomHeader/>
      <v-content>
        <component :isFooterActive="isFooterExtendReference" style="height:100%" :is="activeWindow"></component>
      </v-content>
      <studyRoomFooter v-if="isShowFooter" @footerExtendChanged="(val)=> isFooterExtendReference = val"/>
      <floatingVideoContainer v-if="!isRoomTutor" :isShowFloatingVideo="isShowFloatingVideo" :isFooter="isFooterExtendReference"/>
    </template>

    <studyRoomSettingsDialog v-if="id && !isRoomActive"/>
    <studyRoomDialogs/>
  </div>
</template>

<script>
const studyRoomDrawer = () => import('./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import('./layouts/studyRoomFooter/studyRoomFooter.vue');
const studyRoomSettingsDialog = () => import("./tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue");
const studyRoomHeader = () => import('./layouts/studyRoomHeader/studyRoomHeader.vue');
import * as dialogNames from '../pages/global/dialogInjection/dialogNames.js';
import chatService from "../../services/chatService";
import { mapGetters } from 'vuex';
const canvasWrap = () => import("./windows/canvas/canvasWrap.vue");
const codeEditor = () => import("./codeEditor/codeEditor.vue");
const sharedDocument = () => import("./sharedDocument/sharedDocument.vue");
const classMode = () => import('./windows/class/classRoom.vue');
const classScreen = () => import('./windows/class/classFullScreen.vue');
const floatingVideoContainer = () => import('./layouts/studyRoomDrawer/floatingVideoContainer.vue');
const studyRoomDialogs = () => import('./studyRoomDialogs.vue');
const screenMode = () => import('./windows/shareScreen/shareScreen.vue');
const studyRoomMobile = () => import('./studyRoomMobile.vue');
export default {
  data() {
    return {
      id: this.$route.params.id,
      isDrawerExtendReference:true,
      isFooterExtendReference:true,
    }
  },
  components: {
    studyRoomDrawer,
    studyRoomFooter,
    studyRoomHeader,

    canvasWrap,
    codeEditor,
    sharedDocument,
    classMode,
    classScreen,
    screenMode,

    floatingVideoContainer,
    studyRoomSettingsDialog,
    studyRoomDialogs,
    studyRoomMobile,
  },
  computed: {
    ...mapGetters(['getRoomIsNeedPayment']),
    isMobile(){
      return this.$vuetify.breakpoint.smAndDown;
    },
    isShowFloatingVideo(){
      return !this.isDrawerExtendReference && this.currentEditor !== 'class-screen';
    },
    isFooter(){
      return this.isFooterExtendReference;
    },
    activeWindow(){
      if(this.$store.getters.getActiveNavEditor === 'white-board'){
        return 'canvasWrap'
      }
      return this.$store.getters.getActiveNavEditor
    },
    currentEditor(){
      return this.$store.getters.getActiveNavEditor 
    },
    isShowFooter(){
      return this.currentEditor !== 'class-mode' && this.currentEditor !== 'class-screen'
    },
    isRoomActive(){
      return this.$store.getters.getRoomIsActive;
    },
    isRoomTutor(){
      return this.$store.getters.getRoomIsTutor;
    }
  },
  watch: {
    getRoomIsNeedPayment:{
      immediate:true,
      handler(newVal){
        // note: we need the immediate cuz no one listen to getRoomIsNeedPayment and can 
        // getStudyRoomData empty
        if(newVal !== null){
          this.handleNeedPayment(newVal)
        }
      }
    },
  },
  methods: {
    handleNeedPayment(needPayment){
      if(needPayment){
        this.$openDialog(dialogNames.Payment)
        return;
      }
      if(this.$route.query.dialog === dialogNames.Payment){
        this.$closeDialog()
      }
      this.setStudyRoom(this.id);
    },
    setStudyRoom() {
      let self = this;
      this.$store.dispatch('getChatById',this.$store.getters.getRoomConversationId).then(({ data }) => {
        let currentConversationObj = chatService.createActiveConversationObj(data);
        self.$store.dispatch('setActiveConversationObj',currentConversationObj);
        self.$store.dispatch('updateLockChat',true);
      });
    },
  },
  created() {
    if(this.$store.getters.accountUser?.id){
      this.$store.dispatch('updateStudyRoomInformation',this.id).catch((err)=>{
          if(err?.response){
            this.$router.push('/')
          }
        })
      global.onbeforeunload = function() {     
        return "Are you sure you want to close the window?";
      };
    }else{
      this.$store.commit('setComponent', 'login');
    }
  },
  beforeDestroy() {
    this.$store.dispatch('updateResetRoom');
    global.onbeforeunload = function() { };
  },
};
</script>