<template>
  <v-app>
    <template v-if="isMobile">
      <studyRoomMobile/>
    </template>
    <template v-else>
      <studyRoomDrawer/>
      <studyRoomHeader/>
      <v-content>
        <studyRoomWrapper style="height:100%"/>
      </v-content>
      <studyRoomFooter v-if="isShowFooter"/>
    </template>

    <studyRoomAudio/>
    <studyRoomSettingsDialog v-if="!isRoomActive"/>
    <studyRoomDialogs/>
    <slot name="appInjections"></slot>
  </v-app>
</template>

<script>
const studyRoomDrawer = () => import('./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import('./layouts/studyRoomFooter/studyRoomFooter.vue');
const studyRoomHeader = () => import('./layouts/studyRoomHeader/studyRoomHeader.vue');
import * as dialogNames from '../pages/global/dialogInjection/dialogNames.js';
import chatService from "../../services/chatService";
import { mapGetters } from 'vuex';
const studyRoomMobile = () => import('./studyRoomMobile.vue');
const studyRoomWrapper = () => import('./windows/studyRoomWrapper.vue');
const studyRoomSettingsDialog = () => import("./tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue");
const studyRoomDialogs = () => import('./studyRoomDialogs.vue');
const studyRoomAudio = () => import('./layouts/studyRoomAudio/studyRoomAudio.vue');

export default {
  data() {
    return {
      id: this.$route.params.id,
    }
  },
  components: {
    studyRoomDrawer,
    studyRoomFooter,
    studyRoomHeader,

    studyRoomMobile,
    studyRoomWrapper,
    studyRoomSettingsDialog,
    studyRoomDialogs,

    studyRoomAudio,

  },
  computed: {
    ...mapGetters(['getRoomIsNeedPayment']),
    isMobile(){
      return this.$vuetify.breakpoint.smAndDown;
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
    }
  },
  beforeDestroy() {
    this.$store.dispatch('updateResetRoom');
    global.onbeforeunload = function() { };
  },
};
</script>