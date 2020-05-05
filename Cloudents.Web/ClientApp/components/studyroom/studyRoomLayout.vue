<template>
  <div class="studyRoom">
    <studyRoomDrawer/>
    <studyRoomHeader/>
    <v-content>
      <component style="height:100%" :is="activeWindow"></component>
    </v-content>
    <studyRoomFooter v-if="isShowFooter"/>
    <studyRoomSettingsDialog v-if="id && !isRoomActive"/>
  </div>
</template>

<script>
const studyRoomDrawer = () => import('./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import('./layouts/studyRoomFooter/studyRoomFooter.vue');
import studyRoomSettingsDialog from "./tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue";
import studyRoomHeader from './layouts/studyRoomHeader/studyRoomHeader.vue';
import * as dialogNames from '../pages/global/dialogInjection/dialogNames.js';
import chatService from "../../services/chatService";
import { mapGetters } from 'vuex';
const canvasWrap = () => import("./windows/canvas/canvasWrap.vue");
const codeEditor = () => import("./codeEditor/codeEditor.vue");
const sharedDocument = () => import("./sharedDocument/sharedDocument.vue");
const classMode = () => import('./windows/class/classRoom.vue');
const classScreen = () => import('./windows/class/classFullScreen.vue');
export default {
  data() {
    return {
      id: this.$route.params.id
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

    studyRoomSettingsDialog,
  },
  computed: {
    ...mapGetters(['getRoomIsNeedPayment']),
    activeWindow(){
      if(this.$store.getters.getActiveNavEditor === 'white-board'){
        return 'canvasWrap'
      }
      return this.$store.getters.getActiveNavEditor
    },
    isShowFooter(){
      let currentEditor = this.$store.getters.getActiveNavEditor 
      return currentEditor !== 'class-mode' && currentEditor !== 'class-screen'
    },
    isRoomActive(){
      return this.$store.getters.getRoomIsActive;
    },
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
};
</script>