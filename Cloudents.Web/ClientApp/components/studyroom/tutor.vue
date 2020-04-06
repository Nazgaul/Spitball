<template>
  <v-layout
    column
    class="tutoring-page"
    :style="{'background-size': zoom, 'background-position-x': panX, 'background-position-y': panY}"
    :class="{'gridBackground': $route.name === 'tutoring', 'mobile-no-support': isMobile}"
  >
    <div v-show="isMobile" class="mobile-no-support-container">
      <noSupportTop></noSupportTop>
      <div class="no-support-text" v-language:inner="'tutor_not_supported'"></div>
      <div class="no-support-button">
        <router-link to="/" tag="button" v-language:inner="'tutor_close'"></router-link>
      </div>
      <noSupportBottom></noSupportBottom>
    </div>
    <div v-show="!isMobile">
      <v-flex>
        <nav class="tutoring-navigation">
          <div class="logo-nav-wrap">
            <span class="logo-container">
              <a @click="resetItems()" class="logo-link">
                <logoComponent></logoComponent>
              </a> 
            </span>
            <div
              class="tutor-nav-item cursor-pointer"
              @click="updateActiveNav(singleNav.value)"
              v-for="(singleNav, index) in navs"
              :class="{'active-nav': singleNav.value === activeItem, 'tutor-nav-disabled': singleNav.value !== 'white-board' && singleNav.value !== 'code-editor' && !id}"
              :key="index" :sel="`${singleNav.name.toLowerCase().replace(' ','_')}_tab`">
              <span class="dot-nav" v-if="isRoomActive && !isRoomTutor && singleNav.value === getActiveNavIndicator">●</span>
              <v-icon class="mr-2 nav-icon">{{singleNav.icon}}</v-icon>
              <a class="tutor-nav-item-link">{{singleNav.name}}</a>
            </div>
          </div>
          <div
            style="display: flex; align-items: center; max-height: 48px; justify-content: space-between;"
          >

          <template v-if="isRoomActive && isRoomTutor">
            <startEndSessionBtn :id="id"></startEndSessionBtn>
          </template>
          
            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px; margin-left:30px;" vertical></v-divider>

            <v-btn text icon @click="toggleRecord" class="recording_btn tutoringNavigationBtn" :ripple="false">
              <span v-if="!getIsRecording" class="mt-1 d-flex">
                <beginRecording class="white-btn mr-1"></beginRecording>
                <span class="recording_btn_text" v-language:inner="'tutor_begain_recording'"></span>
              </span>
              <span v-else class="mt-1 d-flex">
                <stopRecording class="mr-2"></stopRecording>
                <span class="recording_btn_text" v-language:inner="'tutor_stop_recording'"></span>
              </span>
            </v-btn>
           
            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px;" vertical></v-divider>
            
            <div class="d-flex tutoringNavigationBtn">
              <v-btn text icon @click="showIntercom" sel="help_draw">
                <intercomSVG class="network-icon"/>
              </v-btn>
            </div> 
            
            

            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px;" vertical></v-divider>
            
            <v-btn class="tutoringNavigationBtn" text icon @click="openSettingsDialog" sel="setting_draw">
              <v-icon class="white-btn">sbf-settings</v-icon>
            </v-btn>
            
          </div>
        </nav>
        <v-flex xs12   class="study-tools-wrapper">
          <v-layout class="pl-2" align-center shrink>
            <v-flex shrink class="canvas-tools-wrapper" v-if="isWhiteBoardActive">
              <whiteBoardTools></whiteBoardTools>
            </v-flex>
            <v-flex shrink class="canvas-tools-wrapper" v-if="isCodeEditorActive">
              <codeEditorTools/>
            </v-flex>
            <v-spacer></v-spacer>
            <v-flex class="share-screen">
              <shareScreenBtn class="nav-share-btn" />
            </v-flex>
            <v-flex shrink class="controls-holder">
              <v-btn sel="video_chat"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                @click.stop="selectViewOption(enumViewOptions.videoChat)"
                :input-value="activeViewOption == enumViewOptions.videoChat"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_videoChat</span>
              </v-btn>
              <v-btn
                :disabled="!getIsFullScreenAvailable"
                @click="selectViewOption(enumViewOptions.fullScreenVideo)"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                :input-value="activeViewOption == enumViewOptions.fullScreenVideo"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_videoFull</span>
              </v-btn>
              <v-btn sel="full_board"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                @click.stop="selectViewOption(enumViewOptions.fullBoard)"
                :input-value="activeViewOption == enumViewOptions.fullBoard"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_fullBoard</span>
              </v-btn>
              <v-layout
                column
                align-start
                class="video-stream-wraper"
                v-show="activeViewOption !== enumViewOptions.fullBoard"
              >
                <v-flex xs6 >
                  <videoStream></videoStream>
                </v-flex>
              </v-layout>
            </v-flex>
          </v-layout>
        </v-flex>

        <transition name="slide-x-transition">
          <keep-alive>
            <component :is="activeItem" :roomId="id"></component>
          </keep-alive>
        </transition>
      </v-flex>
    <template>
      <sb-dialog
        :showDialog="getDialogTutorStart"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'startSessionTutor'"
        :maxWidth="'356'"
        :onclosefn="closeStartSessionTutor"
        :activateOverlay="false"
        :isPersistent="true"
        :content-class="'session-start-tutor-dialog'"
      >
        <startSessionTutor :id="id"></startSessionTutor>
      </sb-dialog>

      <sb-dialog
        :showDialog="getReviewDialogState"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'reviewDilaog'"
        :maxWidth="'596'"
        :onclosefn="closeReviewDialog"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'review-dialog'"
      >
        <leave-review></leave-review>
      </sb-dialog>






















      <sb-dialog
        :showDialog="isBrowserSupportDialog"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'browserDialog'"
        :maxWidth="'612.5'"
        :onclosefn="closeBrowserSupportDialog"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'browser-dialog-unsupport'"
      >
          <browserSupport></browserSupport>
      </sb-dialog>
      <sb-dialog
        :showDialog="getDialogRoomSettings"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'tutor-settings'"
        :maxWidth="'800'"
        :onclosefn="closeStudyRoomSettingsDialog"
        :activateOverlay="false"
        :content-class="'tutor-settings-dialog'"
      >
        <studyRoomSettingsDialog></studyRoomSettingsDialog>
      </sb-dialog>
      <!--show only if not avaliable devices dialog is closed by user-->
      <!-- <sb-dialog
        :showDialog=" && !getDialogRoomSettings"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'startSessionTutor'"
        :maxWidth="'356'"
        :onclosefn="closeStartSessionTutor"
        :activateOverlay="false"
        :isPersistent="true"
        :content-class="'session-start-tutor-dialog'"
      >
        <startSessionTutor :id="id"></startSessionTutor>
      </sb-dialog> -->
      <!--end session confirmation-->
      <sb-dialog
        :showDialog="getDialogRoomEnd"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'endSessionConfirm'"
        :maxWidth="'356'"
        :onclosefn="closeEndDialog"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'session-end-confirm'"
      >
        <endSessionConfirm :id="id"></endSessionConfirm>
      </sb-dialog>
      <!--show only if not avaliable devices dialog is closed by user-->
      <sb-dialog
        :showDialog="getStudentStartDialog && !getDialogRoomSettings"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'startSessionStudent'"
        :maxWidth="'356'"
        :onclosefn="closeStartSessionStudent"
        :activateOverlay="false"
        :isPersistent="true"
        :content-class="'session-start-student-dialog'"
      >
        <startSessionStudent :id="id"></startSessionStudent>
      </sb-dialog>

            <sb-dialog
        :showDialog="getShowAudioRecordingError"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'errorWithAudioRecording'"
        :maxWidth="'675'"
        :onclosefn="closeShowAudioRecordingError"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
      >
        <errorWithAudioRecording></errorWithAudioRecording>
      </sb-dialog>
      <sb-dialog
        :showDialog="getDialogUserConsent"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'userConsentDialog'"
        :maxWidth="'356'"
        :onclosefn="closeUserConsentDialog"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'user-consent-dialog'"
      >
          <studentConsentDialog></studentConsentDialog>
      </sb-dialog>

      <sb-dialog
        :showDialog="getDialogSnapshot"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'studyroomSnapshotDialog'"
        :maxWidth="'800'"
        :onclosefn="closeSnapshotDialog"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'studyroom-snapshot-dialog'"
      >
          <snapshotDialog></snapshotDialog>
      </sb-dialog>
  </template>

    </div>
  </v-layout>
</template>
<script>

import initSignalRService from "../../services/signalR/signalrEventService";
import {CloseConnection} from "../../services/signalR/signalrEventService";
import { mapActions, mapGetters } from "vuex";
import videoStream from "./videoStream/videoStream.vue";
import whiteBoard from "./whiteboard/WhiteBoard.vue";
import codeEditorTools from './codeEditor/codeEditorTools.vue';
const codeEditor = () => import("./codeEditor/codeEditor.vue");

import sharedDocument from "./sharedDocument/sharedDocument.vue";
import shareScreenBtn from "./tutorHelpers/shareScreenBtn.vue";
import logoComponent from '../app/logo/logo.vue';
import testIcon from "./images/eq-system.svg";
import chatIcon from "../../font-icon/message-icon.svg";
import noSupportTop from "./images/not_supported_top.svg";
import noSupportBottom from "./images/not_supported_bottom.svg";
import chatService from "../../services/chatService";
import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
import leaveReview from "./tutorHelpers/leaveReview/leaveReview.vue";
import startSessionTutor from "./tutorHelpers/startSession-popUp-tutor/startSession-popUp-Tutor.vue";
import startSessionStudent from "./tutorHelpers/startSession-popUp-student/startSession-popUp-student.vue";
import whiteBoardTools from "./whiteboard/whiteboardTools.vue";
import startEndSessionBtn from "./tutorHelpers/startEndSessionBtn/startEndSessionBtn.vue";
import endSessionConfirm from "./tutorHelpers/endSessionConfirm/endSessionConfirm.vue";
import browserSupport from "./tutorHelpers/browserSupport/browserSupport.vue";
import insightService from '../../services/insightService.js';
import studyRoomSettingsDialog from "./tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue";
import intercomSVG from './images/icon-1-2.svg'
import studyRoomRecordingService from './studyRoomRecordingService.js';
import errorWithAudioRecording from './tutorHelpers/errorWithAudioRecording/errorWithAudioRecording.vue';
import studentConsentDialog from './tutorHelpers/studentConsentDialog/studentConsentDialog.vue';
import snapshotDialog from './tutorHelpers/snapshotDialog/snapshotDialog.vue';
import stopRecording from './images/stop-recording.svg';
import beginRecording from './images/begain-recording.svg';

import intercomSettings from '../../services/intercomService';

//store
import storeService from "../../services/store/storeService";
import tutoringCanvas from '../../store/studyRoomStore/tutoringCanvas.js';
import tutoringMain from '../../store/studyRoomStore/tutoringMain.js';
import studyRoomTracks_store from '../../store/studyRoomStore/studyRoomTracks_store.js';
import codeEditor_store from '../../store/studyRoomStore/codeEditor_store.js';
import roomRecording_store from '../../store/studyRoomStore/roomRecording_store.js';
import studyroomSettings_store from '../../store/studyRoomStore/studyroomSettings_store';

import studyroomSettingsUtils from '../studyroomSettings/studyroomSettingsUtils';
import * as dialogNames from '../pages/global/dialogInjection/dialogNames.js';

export default {
  components: {
    videoStream,
    whiteBoard,
    codeEditor,
    sharedDocument,
    shareScreenBtn,
    logoComponent,
    testIcon,
    chatIcon,
    sbDialog,
    leaveReview,
    noSupportTop,
    noSupportBottom,
    startSessionTutor,
    startSessionStudent,
    whiteBoardTools,
    startEndSessionBtn,
    endSessionConfirm,
    browserSupport,
    studyRoomSettingsDialog,
    codeEditorTools,
    intercomSVG,
    errorWithAudioRecording,
    studentConsentDialog,
    snapshotDialog,
    stopRecording,
    beginRecording
  },
  name: "tutor",
  data() {
    return {
      isBrowserSupportDialog:false,




      activeNavItem: "white-board",
      navs: [
        {
          name: this.$t("tutor_nav_canvas"),
          value: "white-board",
          icon: "sbf-canvas"
        },
        {
          name: this.$t("tutor_nav_code"),
          value: "code-editor",
          icon: "sbf-code-editor"
        },
        {
          name: this.$t("tutor_nav_text"),
          value: "shared-document",
          icon: "sbf-text-icon"
        }
      ],
      videoChat: true,
      enumViewOptions: {
        videoChat: "videoChat",
        fullBoard: "fullBoard",
        fullScreenVideo: "fullScreenVideo"
      },
      activeViewOption: "videoChat",
      userId: null,
    };
  },

  props: {
    id: {
      type: String,
      default: ''
    }
  },

  computed: {
    ...mapGetters([
      "getIsFullScreenAvailable",
      "getDialogTutorStart",
      "getRoomIsNeedPayment",
      "getDialogUserConsent",

      
      "getDialogRoomSettings",
      "getZoom",
      "getPanX",
      "getPanY",
      "getReviewDialogState",
      "getStudentStartDialog",
      "getDialogRoomEnd",
      "accountUser",
      "getActiveNavIndicator",
      "getIsRecording",
      "getShowAudioRecordingError",
      "getVisitedSettingPage",
      "getDialogSnapshot",
    ]),
    isRoomTutor(){
      return this.$store.getters.getRoomIsTutor;
    },
    isRoomActive(){
      return this.$store.getters.getRoomIsActive;
    },











    activeItem() {
      return this.activeNavItem;
    },
    isWhiteBoardActive() {
      return this.activeItem === "white-board" ? true : false;
    },
    zoom() {
      let gridSize = (20 * Number(this.getZoom.toFixed())) / 100;
      return `${gridSize}px ${gridSize}px`;
    },
    panX() {
      return `${this.getPanX}px`;
    },
    panY() {
      return `${this.getPanY}px`;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    isTutor() {
      return this.$store.getters.getRoomIsTutor;
    },
    isCodeEditorActive(){
      return this.activeItem === "code-editor"
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
    ...mapActions([
      "setActiveConversationObj",
      "getChatById",
      "lockChat",
      "updateReviewDialog",
      "updateReview",
      "updateStudentStartDialog",
      "closeChat",
      "openChatInterface",
      "updateEndDialog",
      "setShowAudioRecordingError",
      "updateDialogUserConsent",
      "updateDialogSnapshot",
      "stopTracks"
    ]),
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
    initMathjax(){
      this.$loadScript("https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-AMS_SVG")
        .then(() => {
          MathJax.Hub.Config({
            showMathMenu: false,
            SVG: {
              useGlobalCache: false,
              useFontCache: false
            }
          });
          MathJax.AuthorInit = function(texstring, callback) {
            var input = texstring;
            var wrapper = document.createElement("div");
            wrapper.innerHTML = input;
            var output = { svg: "" };
            MathJax.Hub.Queue(["Typeset", MathJax.Hub, wrapper]);
            MathJax.Hub.Queue(function() {
              var mjOut = wrapper.getElementsByTagName("svg")[0];
              if (!mjOut) {
                return null;
              }
              mjOut.setAttribute("xmlns", "http://www.w3.org/2000/svg");
              output.svg = mjOut.outerHTML;
              callback(output);
            });
          };
      });
    },
    openSettingsDialog(){
      this.$ga.event("tutoringRoom", "openSettingsDialog");
      this.$store.dispatch('updateDialogRoomSettings',true)
    },
    closeReviewDialog() {
      this.updateReviewDialog(false);
    },
    closeStudyRoomSettingsDialog(){
      this.$store.dispatch('updateDialogRoomSettings',false)
    },
    closeEndDialog() {
      this.updateEndDialog(false);
    },
    closeStartSessionTutor() {
    },
    closeStartSessionStudent() {
      this.updateStudentStartDialog(false);
    },
    closeShowAudioRecordingError(){
      this.setShowAudioRecordingError(false);
    },
    updateActiveNav(value) {
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_navigation', {'roomId': this.id, 'userId': this.userId, 'navigatedTo': value}, null)
      
      this.$ga.event("tutoringRoom", `updateActiveNav:${value}`);

      this.activeNavItem = value;
      if(this.isRoomTutor){
        let activeNavData = {
            activeNav: value,
        }
        let transferDataObj = {
            type: "updateActiveNav",
            data: activeNavData
        };
        let normalizedData = JSON.stringify(transferDataObj);
        this.$store.dispatch('sendDataTrack',normalizedData)
      }
    },
    selectViewOption(param) {
      this.$ga.event("tutoringRoom", `selectViewOption:${param}`);

      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_selectViewOption', {'roomId': this.id, 'userId': this.userId, 'viewOption': param}, null)
      this.activeViewOption = param;
      if (this.activeViewOption === this.enumViewOptions.videoChat) {
        this.videoChat = !this.videoChat;
        this.openChatInterface();
      } else if (this.activeViewOption === this.enumViewOptions.fullBoard) {
        this.videoChat = !this.videoChat;
        this.closeChat();
      } else if (
        this.activeViewOption === this.enumViewOptions.fullScreenVideo
      ) {
        this.biggerRemoteVideo();
      }
    },
    biggerRemoteVideo() {
      //check browser support
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_biggerRemoteVideo', {'roomId': this.id, 'userId': this.userId}, null)
      var video = document.querySelector("#remoteTrack video");
      if (!video) return;
      if (video.requestFullscreen) {
        video.requestFullscreen();
      } else if (video.webkitRequestFullscreen) {
        video.webkitRequestFullscreen();
      } else if (video.mozRequestFullScreen) {
        video.mozRequestFullScreen();
      } else if (video.msRequestFullscreen) {
        video.msRequestFullscreen();
      }
    },
    setStudyRoom() {
      this.initMathjax()
      debugger;
      let self = this;
      this.getChatById(this.$store.getters.getRoomConversationId).then(({ data }) => {
debugger;
        insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_ChatById', data, null)
        let currentConversationObj = chatService.createActiveConversationObj(data);
        self.setActiveConversationObj(currentConversationObj);
        self.lockChat();
      });
    },
    closeBrowserSupportDialog(){
      this.isBrowserSupportDialog = false;
    },
    resetItems(){
      let isExit = confirm(this.$t("login_are_you_sure_you_want_to_exit"),)
      if(isExit){
        this.$ga.event("tutoringRoom", 'resetItems');
        this.$router.push('/');
      }
    },
    showIntercom(){
      this.$ga.event("tutoringRoom", 'showIntercom');
      intercomSettings.showDialog();
    },
    toggleRecord(){
      this.$ga.event("tutoringRoom", 'toggleRecord');
      studyRoomRecordingService.toggleRecord(this.isTutor);
    },
    closeUserConsentDialog(){
      this.updateDialogUserConsent(false);
    },
    closeSnapshotDialog(){
      this.updateDialogSnapshot(false);
    }
  },
  destroyed(){
    if(this.isTutor) {
      this.$store.commit('setComponent', 'linkToaster') 
    }
    global.onbeforeunload = function() { };
  },
  beforeDestroy(){
    this.$store.dispatch('updateResetRoom');



    this.stopTracks();
    

    this.updateStudentStartDialog(false);
    storeService.unregisterModule(this.$store,'tutoringCanvas');
    // storeService.unregisterModule(this.$store,'tutoringMain');
    storeService.unregisterModule(this.$store,'studyRoomTracks_store');
    storeService.unregisterModule(this.$store,'roomRecording_store');
    storeService.unregisterModule(this.$store,'codeEditor_store');
    if(this.id){
      CloseConnection(`studyRoomHub?studyRoomId=${this.id}`);
    }

  },
  beforeCreate(){
    storeService.registerModule(this.$store,'studyRoomTracks_store',studyRoomTracks_store);
    storeService.registerModule(this.$store,'roomRecording_store',roomRecording_store);
    // storeService.registerModule(this.$store,'tutoringMain',tutoringMain);
    storeService.lazyRegisterModule(this.$store,'tutoringMain',tutoringMain);
    storeService.lazyRegisterModule(this.$store,'studyroomSettings_store',studyroomSettings_store);
    storeService.registerModule(this.$store,'tutoringCanvas',tutoringCanvas);
    storeService.registerModule(this.$store,'codeEditor_store',codeEditor_store);
  },
  async created() {
    this.$store.commit('clearComponent')
    this.userId = this.accountUser?.id || 'GUEST';

    if (!studyroomSettingsUtils.isBrowserSupport()) {
      this.$nextTick(()=>{
        this.isBrowserSupportDialog = true;
        let roomId = this.id ? this.id : 'No-Id';
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_BrowserNotSupported', {'roomId': roomId, 'userId': this.userId}, null)
      })
      return;
    }

    if(this.id){
      initSignalRService(`studyRoomHub?studyRoomId=${this.id}`);
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_Enter', {'roomId': this.id, 'userId': this.userId}, null) 
      this.$store.dispatch('updateStudyRoomInformation',this.id).catch((err)=>{
          if(err?.response){
            insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_RoomProps', err, null)
            this.$router.push('/')
          }
        })
    }else{
      //TODO - we need one place to invoke this.
      this.initMathjax()
    }

    
    // in case refresh was made in studyRoom page, make sure to init local media tracks. (to be able to share video/audio)

    // this code will create an error object to know what is the cause of the problem in case there is one.
    // settings page is running this code, but we should run this code in case refresh was made in the study room page.
    // run this code only if refresh was made in the study room 
    // if(!this.getVisitedSettingPage){
    //   await studyroomSettingsUtils.validateUserMedia(true, true); 
    // }
    //this line will init the tracks to show local medias
    studyroomSettingsUtils.validateMedia();

    global.onbeforeunload = function() {     
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_beforeUnloadTriggered', {'roomId': this.id, 'userId': this.userId}, null)
      return "Are you sure you want to close the window?";
    };
  }
};
</script>

<style lang="less" src="./tutor.less"></style>