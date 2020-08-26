<template>
  <v-app>
    <template v-if="isMobile">
      <studyRoomMobile/>
    </template>
    <template v-else>
      <studyRoomDrawer/>

      <studyRoomHeader @roomMuted="showRoomMutedToaster = true"/>
      <v-main>
        <studyRoomWrapper style="height:100%"/>
        <roomMutedToaster v-if="showRoomMutedToaster"/>
      </v-main>
      <studyRoomFooter v-if="isShowFooter"/>
    </template>
    
    <studyRoomAudio/>
    <studyRoomSettingsDialog v-if="!isRoomActiveAndUpdated && !$store.getters.getIsBrowserNotSupport"/>
    <studyRoomDialogs/>
    <slot name="appInjections"></slot>
  </v-app>
</template>

<!--suppress JSUnusedGlobalSymbols -->
<script>
const studyRoomDrawer = () => import(/* webpackChunkName: "studyroomdesktop" */'./layouts/studyRoomDrawer/studyRoomDrawer.vue');
const studyRoomFooter = () => import(/* webpackChunkName: "studyroomdesktop" */'./layouts/studyRoomFooter/studyRoomFooter.vue');
const studyRoomHeader = () => import(/* webpackChunkName: "studyroomdesktop" */'./layouts/studyRoomHeader/studyRoomHeader.vue');
const studyRoomWrapper = () => import(/* webpackChunkName: "studyroomdesktop" */'./windows/studyRoomWrapper.vue');

import chatService from "../../services/chatService";
import { mapGetters } from 'vuex';
const studyRoomMobile = () => import('./studyRoomMobile.vue');

const studyRoomSettingsDialog = () => import("./tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue");
import roomMutedToaster from './layouts/roomMutedToaster.vue';

import * as componentConsts from '../pages/global/toasterInjection/componentConsts.js';
import studyRoomAudio from'./layouts/studyRoomAudio/studyRoomAudio.vue';
import studyRoomDialogs from './studyRoomDialogs.vue';

export default {
  data() {
    return {
      id: this.$route.params.id,
      showRoomMutedToaster:false,
      isReady:false,
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
    roomMutedToaster,

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
    isRoomActiveAndUpdated(){
      if(this.$store.getters.getRoomIsTutor){
        return this.isRoomActive;
      }else{
        return this.isRoomActive && this.$store.getters.getIsRoomUpdated;
      }
    },
    isRoomEnabled(){
      return this.$store.getters.getJwtToken
    }
  },
  watch: {
    isRoomActive:{
      immediate:true,
      handler(val){
        if(val ){
          window.onbeforeunload = function() {     
            return "Are you sure you want to close the window?";
          };
        }
      }
    },
    showRoomMutedToaster:{
      deep:true,
      handler(newVal){
        if(newVal){
          setTimeout(() => {
            this.showRoomMutedToaster = false
          }, 1000);
        }
      }
    }
  },
  methods: {
    handleNeedPayment(needPayment){
      if(!this.$store.getters.getUserLoggedInStatus){
        return
      }
      if(needPayment){
        this.$store.commit('addComponent',componentConsts.PAYMENT_DIALOG)
        return;
      }
      if(!this.isReady){
      this.setStudyRoom(this.id);
      }
    },
    setStudyRoom() {
      this.isReady = true;
      let self = this;
      this.$store.dispatch('getChatById',this.$store.getters.getRoomConversationId).then(({ data }) => {
        let currentConversationObj = chatService.createActiveConversationObj(data);
        self.$store.dispatch('setActiveConversationObj',currentConversationObj);
      });
    },
  },
  updated() {
    this.$watch('getRoomIsNeedPayment', (newVal) => {
        // note: we need the immediate cuz no one listen to getRoomIsNeedPayment and can 
        // getStudyRoomData empty
          let self = this;
          self.$nextTick(()=>{
            self.handleNeedPayment(newVal)
          })
      },{immediate:true}
    );
  },
  beforeDestroy() {
    this.$store.dispatch('updateResetRoom');
    global.onbeforeunload = function() { };
  },
};
</script>