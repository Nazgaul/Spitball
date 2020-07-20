<template>
   <v-app-bar height="62" app clipped-right color="#4c59ff" class="studyRoomHeader elevation-0">
      <a @click="resetItems()">
         <logoComponent class="studyRoomMainLogo"/>
      </a>
      <div class="roundShape"></div>
      <v-toolbar-title class="white--text me-7">
         <span class="liveText">{{$t('studyRoom_live')}}</span>
         </v-toolbar-title>
      <template v-if="isRoomTutor" >
         <template v-for="(navTab, objectKey) in navTabs">
            <v-divider :key="`${objectKey}_1`" height="33px" class="divider" vertical inset color="white"></v-divider>
            <button :key="objectKey" @click="actionHandler(objectKey)"
               :class="['tutorNavTab', 'd-flex','flex-md-column','px-lg-5','px-md-4','align-md-center','flex-lg-row','justify-md-center',
                  {'tutorNavTab-active': navTab.icon == navTabs[getIsCurrentMode(currentEditorMode)].icon}]" >
                  <v-icon class="me-md-0 me-lg-2" style="vertical-align: sub;" size="15" :color="navTab.icon == navTabs[getIsCurrentMode(currentEditorMode)].icon?'#4c59ff':'white'">
                     {{navTab.icon}}
                  </v-icon>
                  <span>
                     {{navTab.text}}
                  </span>
            </button>
         </template>
      </template>
      <template v-else>
         <button :class="['tutorNavTab','tutorNavTab-active','px-5']" style="cursor:initial" >
               <v-icon class="me-2" style="vertical-align: sub;" size="15" color="#4c59ff">
                  {{navTabs[getIsCurrentMode(currentEditorMode)].icon}}
               </v-icon>
               <span>
                  {{navTabs[getIsCurrentMode(currentEditorMode)].text}}
               </span>
         </button>
      </template>
      <v-spacer></v-spacer>
      <v-btn v-if="isRoomTutor" class="mb-2" :ripple="false" text @click="muteAll()">
         <div class="muteAllBtn">
            <v-icon size="16">sbf-microphone</v-icon>
            <span>{{$t('tutor_mute_room')}}</span>
         </div>
      </v-btn>
      <button :class="['endBtn','mb-2',{'ms-2':!isRoomTutor}]" @click="endSession()">
         <div class="btnIcon"></div>
         <span>{{isRoomTutor? $t('studyRoom_end') : $t('studyRoom_end_student')}}</span>
      </button>
      <template v-if="roomNetworkQualityLevel">
         <v-tooltip bottom>
            <template v-slot:activator="{ on }">
               <div v-on="on" class="net ms-3 me-3 mt-3" >
                  <div v-for="(item, index) in 5" :key="index" :class="['bar',{'barFull':roomNetworkQualityLevel <= index}]"></div>
               </div>
            </template>
            <div dir="auto" v-if="roomNetworkQualityStats.localAudioSend" v-text="$t('localAudioSend',{0:roomNetworkQualityStats.localAudioSend})"/>
            <div dir="auto" v-if="roomNetworkQualityStats.localVideoSend" v-text="$t('localVideoSend',{0:roomNetworkQualityStats.localVideoSend})"/>
            <div dir="auto" v-if="roomNetworkQualityStats.remoteAudioReceive" v-text="$t('remoteAudioReceive',{0:roomNetworkQualityStats.remoteAudioReceive})"/>
            <div dir="auto" v-if="roomNetworkQualityStats.remoteVideoReceive" v-text="$t('remoteVideoReceive',{0:roomNetworkQualityStats.remoteVideoReceive})"/>
         </v-tooltip>
      </template>
      <v-menu offset-y min-width="158" content-class="menuStudyRoom">
         <template v-slot:activator="{ on }">
            <v-btn icon class="mb-2">
               <v-icon size="16" color="#CCCCCC" v-on="on">sbf-3-dot</v-icon>
            </v-btn>
         </template>
         <v-list>
            <v-list-item class="menuStudyRoomOption" @click="toggleRecord">
               <template v-if="!getIsRecording">
                     <v-icon color="7a798c" class="me-2" size="20">sbf-record</v-icon> 
                     {{$t('tutor_begain_recording')}}
               </template>
               <template v-else>
                     <v-icon color="7a798c" class="me-2" size="20">sbf-record</v-icon> 
                     {{$t('tutor_stop_recording')}}
               </template>
            </v-list-item>
            <v-list-item class="menuStudyRoomOption" sel="setting_draw" @click="openSettingsDialog">
                  <v-icon color="7a798c" class="me-2" size="20">sbf-settings</v-icon> 
                  {{$t('studyRoom_menu_settings')}}
            </v-list-item>
            <v-list-item class="menuStudyRoomOption" sel="help_draw" @click="showIntercom">
                  <v-icon color="7a798c" class="me-2" size="20">sbf-help-icon</v-icon> 
                  {{$t('studyRoom_menu_help')}}
            </v-list-item>
            </v-list>
         </v-menu>
   </v-app-bar>
</template>

<script>
import studyRoomRecordingService from '../../studyRoomRecordingService.js';
import intercomSettings from '../../../../services/intercomService';

import logoComponent from "../../../app/logo/logo.vue";
import { mapGetters } from 'vuex';
export default {
   components:{
      logoComponent,
   },
   computed: {
      ...mapGetters(['getIsRecording']),
      roomNetworkQualityLevel(){
         return this.$store.getters.getRoomNetworkQuality?.level;
      },
      roomNetworkQualityStats(){
         return this.$store.getters.getRoomNetworkQuality?.stats;
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      },
      roomModes(){
         return this.$store.getters.getRoomModeConsts;
      },
      currentEditorMode(){
         return this.$store.getters.getActiveNavEditor;
      },
      navTabs(){
         return {
            [this.roomModes.CLASS_MODE]:{
               icon:'sbf-class',
               text: this.$t('studyRoom_nav_class')
            },
            [this.roomModes.WHITE_BOARD]:{
               icon:'sbf-whiteboard',
               text: this.$t('studyRoom_nav_whiteboard')
            },
            [this.roomModes.SCREEN_MODE]:{
               icon:'sbf-shareScreen',
               text: this.$t('studyRoom_nav_screen')
            },
            [this.roomModes.TEXT_EDITOR]:{
               icon:'sbf-text',
               text: this.$t('studyRoom_nav_text')
            },
            [this.roomModes.CODE_EDITOR]:{
               icon:'sbf-code',
               text: this.$t('studyRoom_nav_code')
            },
         }
      }
   },
   methods: {
      toggleRecord(){
         this.$ga.event("tutoringRoom", 'toggleRecord');
         studyRoomRecordingService.toggleRecord(this.isRoomTutor);
      },
      showIntercom(){
         this.$ga.event("tutoringRoom", 'showIntercom');
         intercomSettings.showDialog();
      },
      openSettingsDialog(){
         this.$ga.event("tutoringRoom", "openSettingsDialog");
         this.$store.commit('toggleAudioVideoDialog',true)
      },
      resetItems(){
         let isExit = confirm(this.$t("login_are_you_sure_you_want_to_exit"),)
         if(isExit){
         this.$store.dispatch('updateResetRoom');
         this.$ga.event("tutoringRoom", 'resetItems');
         global.onbeforeunload = function() { };
         window.location = '/'
         }
      },
      setWhiteboard() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.WHITE_BOARD)
      },
      setTextEditor() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.TEXT_EDITOR)
      },
      setCodeEditor() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.CODE_EDITOR)
      },
      setClass() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.CLASS_MODE)
      },
      setShareScreen() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.SCREEN_MODE)
      },
      endSession() {
         this.$ga.event("tutoringRoom", "endSession");
         if(this.isRoomTutor){
            this.$store.dispatch('updateEndDialog',true)
         }else{
            this.$store.dispatch('updateEndSession')
         }
      },
      actionHandler(editorType){
         let actionsOptions = {
            [this.roomModes.WHITE_BOARD]:this.setWhiteboard,
            [this.roomModes.TEXT_EDITOR]:this.setTextEditor,
            [this.roomModes.CODE_EDITOR]:this.setCodeEditor,
            [this.roomModes.CLASS_MODE]:this.setClass,
            [this.roomModes.SCREEN_MODE]:this.setShareScreen,
         }
         actionsOptions[editorType]()
      },
      muteAll(){
         this.$store.dispatch('updateToggleAudioParticipants')
         this.$emit('roomMuted')
      },
      getIsCurrentMode(modeName){
         if(modeName == this.roomModes.CLASS_SCREEN){
            return this.roomModes.CLASS_MODE
         }else{
            return modeName
         }
      }
   },
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
.menuStudyRoom{
   .menuStudyRoomOption{
      font-size: 14px;
      color: #43425d;
   }
}
   .studyRoomHeader {
      .v-toolbar__content{
         padding-bottom: 0;
         padding-top: 10px;
         padding-right: 6px;
      }
      .studyRoomMainLogo {
         .logo {
            fill: #fff !important;
         }
      }
      .tutorNavTab{
         outline: none;
         margin-top: 2px;
         font-size: 13px;
         font-weight: 600;
         color: white;
         height: 50px;
         min-width: 130px;
         @media(max-width: @screen-md){
            min-width: initial;
         }

         &.tutorNavTab-active{
            border-radius: initial;
            color: #4c59ff;
            background: white;
            border-top-right-radius: 6px;
            border-top-left-radius: 6px;
            display: flex;
            justify-content: center;
            align-items: center;
         }
      }
      .muteAllBtn{
         display: flex;
         flex-direction: column;
         color: white;
         font-size: 12px;
         margin-top: 2px;
         font-weight: 600;
         justify-content: space-between;
         height: 36px;
      }
      .endBtn{
         background: white;
         height: 36px;
         border-radius: 18.5px;
         padding: 0 18px;
         color: #4c59ff;
         font-size: 14px;
         font-weight: 600;
         display: flex;
         align-items: center;
         outline: none;
         .btnIcon{
            width: 14px;
            height: 14px;
            border-radius: 3px;
            margin-right: 12px;
            margin-top: 2px;
            background-color: #4c59ff;
         }
      }
      .net{
         @barWidth : 3px;
         width: @barWidth * 5;
         position: relative;
         height: 12px;
         .bar{
            position: absolute;
            bottom: 0;
            width: @barWidth;
            &:nth-child(1){
               background: white;
               height: 3px;
               left: @barWidth * 0 + 3px;
            }
            &:nth-child(2){
               background: white;
               height: 8px;
               left: @barWidth * 1 + 6px
            }
            &:nth-child(3){
               background: white;
               height: 13px;
               left: @barWidth * 2 + 9px
            }
            &:nth-child(4){
               background: white;
               height: 18px;
               left: @barWidth * 3 + 12px
            }

            &:nth-child(5){
               background: white;
               height: 23px;
               left: @barWidth * 4 + 15px
            } 
            &.barFull{
               background: rgb(108 117 253);
               z-index: 2;
            }
         }
      }
            
      .divider{
         opacity: 0.48;
         margin-top: 9px;
         height: 30px;
      }
      .roundShape {
         width: 8px;
         height: 8px;
         background-color: #fff;
         border-radius: 50%;
         margin: 0 5px 2px 2px;
      }
      .v-toolbar__title{
         flex-shrink: 0;
         flex-grow: 0;
      }
      .liveText{
         font-size: 22px;
         font-weight: 600;
         vertical-align: super;
      }
   }
</style>