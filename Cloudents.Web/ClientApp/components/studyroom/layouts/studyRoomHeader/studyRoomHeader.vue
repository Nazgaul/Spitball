<template>
   <v-app-bar height="62" app clipped-right color="#4c59ff" class="studyRoomHeader elevation-0">
      <a @click="resetItems()">
         <logoComponent/>
      </a>
      <div class="roundShape mr-2"></div>
      <v-toolbar-title class="white--text mr-7">{{$t('studyRoom_live')}}</v-toolbar-title>
      <!-- <v-divider class="ml-3 divider" vertical inset color="white"></v-divider> -->

      <!-- <v-btn-toggle v-if="isRoomTutor" mandatory :value="getIsCurrentMode(currentEditorMode)" :ripple="false" active-class="editorActive"  borderless group class="editors">
         <v-btn v-for="(navTab, objectKey) in navTabs" :key="objectKey" 
                :value="objectKey" text @click="actionHandler(objectKey)">
            <span><v-icon class="mr-2">{{navTab.icon}}</v-icon>{{navTab.text}}</span>
         </v-btn>
      </v-btn-toggle> -->
      <template v-if="isRoomTutor" >
         <template v-for="(navTab, objectKey) in navTabs">
            <v-divider height="33px" class="divider" vertical inset color="white"></v-divider>
            <button :key="objectKey" @click="actionHandler(objectKey)"
               :class="['tutorNavTab', 
                  {'tutorNavTab-active': navTab.icon == navTabs[getIsCurrentMode(currentEditorMode)].icon}]" >
               <span><v-icon size="16" :color="navTab.icon == navTabs[getIsCurrentMode(currentEditorMode)].icon?'#4c59ff':'white'" class="mr-2">{{navTab.icon}}</v-icon>{{navTab.text}}</span>
            </button>
            
         </template>
      </template>
      <div v-else class="studentNavTab">
         <v-icon size="16" class="mr-2" color="#4c59ff">
            {{navTabs[getIsCurrentMode(currentEditorMode)].icon}}
         </v-icon>
         {{navTabs[getIsCurrentMode(currentEditorMode)].text}}
      </div>

      <v-spacer></v-spacer>
      <template v-if="isRoomTutor">
         <v-btn text @click="muteAll()">
            <div class="muteAllBtn">
               <v-icon v-if="$store.getters.getIsAudioParticipants" size="16">sbf-microphone</v-icon>
               <v-icon v-else size="16">sbf-mic-ignore</v-icon>
               <span>{{$t($store.getters.getIsAudioParticipants?'tutor_mute_room':'tutor_unmute_room')}}</span>
            </div>
         </v-btn>
         <v-btn class="endBtn" rounded depressed  @click="endSession()">
            <div class="btnIcon"></div>
            <span>{{$t('studyRoom_end')}}</span>
         </v-btn>
      </template>

      <v-menu offset-y min-width="158" content-class="menuStudyRoom">
         <template v-slot:activator="{ on }">
            <v-btn icon>
               <v-icon v-on="on">sbf-3-dot</v-icon>
            </v-btn>
         </template>
     <v-list>
        <v-list-item class="menuStudyRoomOption" @click="toggleRecord">
           <template v-if="!getIsRecording">
               <v-icon color="7a798c" class="mr-2" size="20">sbf-record</v-icon> 
               {{$t('tutor_begain_recording')}}
           </template>
           <template v-else>
               <v-icon color="7a798c" class="mr-2" size="20">sbf-record</v-icon> 
               {{$t('tutor_stop_recording')}}
           </template>

        </v-list-item>
        <v-list-item class="menuStudyRoomOption" sel="setting_draw" @click="openSettingsDialog">
            <v-icon color="7a798c" class="mr-2" size="20">sbf-settings</v-icon> 
            {{$t('studyRoom_menu_settings')}}
        </v-list-item>
        <v-list-item class="menuStudyRoomOption" sel="help_draw" @click="showIntercom">
            <v-icon color="7a798c" class="mr-2" size="20">sbf-help-icon</v-icon> 
            {{$t('studyRoom_menu_help')}}
        </v-list-item>
      </v-list>
      </v-menu>
      <v-dialog v-model="getDialogRoomEnd" max-width="356px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
         <endSessionConfirm/>
      </v-dialog>    
         <studyRoomAudioVideoDialog v-if="settingDialogState"
        @closeAudioVideoSettingDialog="val => settingDialogState = val"/>

      <v-dialog v-model="getDialogUserConsent" max-width="356px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
          <studentConsentDialog></studentConsentDialog>
      </v-dialog>

      <v-dialog v-model="getShowAudioRecordingError" max-width="675px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
        <errorWithAudioRecording></errorWithAudioRecording>
      </v-dialog>


   </v-app-bar>
</template>

<script>
import endSessionConfirm from "../../tutorHelpers/endSessionConfirm/endSessionConfirm.vue";
import studyRoomRecordingService from '../../studyRoomRecordingService.js';
import intercomSettings from '../../../../services/intercomService';
import studyRoomAudioVideoDialog from '../../tutorHelpers/studyRoomSettingsDialog/studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue';
import studentConsentDialog from '../../tutorHelpers/studentConsentDialog/studentConsentDialog.vue';
import errorWithAudioRecording from '../../tutorHelpers/errorWithAudioRecording/errorWithAudioRecording.vue';

import logoComponent from "../../../app/logo/logo.vue";
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         settingDialogState:false,
      }
   },
   components:{
      logoComponent,
      endSessionConfirm,
      studyRoomAudioVideoDialog,
      studentConsentDialog,
      errorWithAudioRecording
   },
   computed: {
      ...mapGetters(['getDialogRoomEnd','getIsRecording','getDialogUserConsent','getShowAudioRecordingError']),
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
         this.settingDialogState = true;
      },
      resetItems(){
         let isExit = confirm(this.$t("login_are_you_sure_you_want_to_exit"),)
         if(isExit){
         this.$ga.event("tutoringRoom", 'resetItems');
         this.$router.push('/');
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
            this.$store.dispatch('updateEndDialog',true)
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
.menuStudyRoom{
   .menuStudyRoomOption{
      font-size: 14px;
      color: #43425d;
   }
}
   .studyRoomHeader {
      .v-toolbar__content{
         padding-bottom: 0;
      }
      .logo {
         fill: #fff;
      }
      .tutorNavTab{
         outline: none;
         margin-top: 8px;
         font-size: 12px;
         font-weight: 600;
         color: white;
         height: 50px;
         width: 130px;
         &.tutorNavTab-active{
            color: #4c59ff;
            // margin-left: 26px;
            background: white;
            border-top-right-radius: 6px;
            border-top-left-radius: 6px;
            display: flex;
            justify-content: center;
            align-items: center;
         }
      }
      .studentNavTab{
         // margin-left: 26px;
         background: white;
         width: 130px;
         height: 50px;
         margin-top: 8px;
         border-top-right-radius: 6px;
         border-top-left-radius: 6px;
         display: flex;
         justify-content: center;
         align-items: center;
         color: #4c59ff;
         font-size: 12px;
         font-weight: 600;
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
         height: 36px;
         border-radius: 18.5px;
         padding: 0 18px;
         color: #4c59ff;
         font-size: 14px;
         font-weight: 600;
         .btnIcon{
            width: 14px;
            height: 14px;
            border-radius: 3px;
            margin-right: 12px;
            margin-top: 2px;
            background-color: #4c59ff;
         }
      }
      .editors{
         button{
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 0 !important;
         }
         .editorActive{
            background: white !important;
            color: #4c59ff !important;
         }
      }
            
      .divider{
         opacity: 0.28;
         margin-top: 14px;
         min-height: 0;
         max-height: calc(100% - 22px);
      }
      .roundShape {
         width: 8px;
         height: 8px;
         background-color: #fff;
         border-radius: 50%;
      }
   }
</style>