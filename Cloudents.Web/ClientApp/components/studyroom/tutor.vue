<template>
  <v-layout
    column
    class="tutoring-page"
    :style="{'background-size': zoom, 'background-position-x': panX, 'background-position-y': panY}"
    :class="{'gridBackground': $route.name === 'tutoring'}">
    <div>
      <v-flex>
        <nav class="tutoring-navigation d-none d-md-flex">
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
        <v-flex xs12  class="study-tools-wrapper d-none d-md-block">
          <v-layout class="pl-2" align-center shrink>
            <v-flex shrink class="canvas-tools-wrapper" v-if="isWhiteBoardActive">
              <whiteBoardTools></whiteBoardTools>
            </v-flex>
            <v-flex shrink class="canvas-tools-wrapper" v-if="isCodeEditorActive">
              <codeEditorTools/>
            </v-flex>
            <v-spacer></v-spacer>
            <v-flex v-if="isRoomTutor" class="share-screen">
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
              <v-btn :style="{'visibility': isRoomTutor? 'visible':'hidden'}"
                @click="$store.dispatch('updateToggleAudioParticipants')"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                :input-value="$store.getters.getIsAudioParticipants"
                active-class="v-btn--active control-btn-active"
              >
                <span>{{$t($store.getters.getIsAudioParticipants?'tutor_mute_room':'tutor_unmute_room')}}</span>
              </v-btn>
              <v-btn sel="full_board"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                @click.stop="selectViewOption(enumViewOptions.fullBoard)"
                :input-value="activeViewOption == enumViewOptions.fullBoard"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_fullBoard</span>
              </v-btn>
             
                <!-- <v-flex xs6 > -->
                 
                <!-- </v-flex> -->
            </v-flex>
          </v-layout>
        </v-flex>
         <v-layout
                column
                align-start
                class="video-stream-wraper"
                v-show="activeViewOption !== enumViewOptions.fullBoard"
              >
        <videoStream></videoStream>
         </v-layout>
        <transition name="slide-x-transition">
          <keep-alive>
            <component :is="activeItem" :roomId="id"></component>
          </keep-alive>
        </transition>
      </v-flex>
    <template>

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
      <!--show only if not avaliable devices dialog is closed by user-->
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

      <studyRoomAudioVideoDialog
        v-if="settingDialogState"
        @closeAudioVideoSettingDialog="val => settingDialogState = val"
      />
      <studyRoomSettingsDialog v-if="id && !isRoomActive"/>
  </template>

    </div>
  </v-layout>
</template>
<script>

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
import chatService from "../../services/chatService";
import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
import leaveReview from "./tutorHelpers/leaveReview/leaveReview.vue";
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
import studyRoomAudioVideoDialog from './tutorHelpers/studyRoomSettingsDialog/studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue'

import intercomSettings from '../../services/intercomService';

//store
import storeService from "../../services/store/storeService";
import tutoringCanvas from '../../store/studyRoomStore/tutoringCanvas.js';
import codeEditor_store from '../../store/studyRoomStore/codeEditor_store.js';
import roomRecording_store from '../../store/studyRoomStore/roomRecording_store.js';

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
    beginRecording,
    studyRoomAudioVideoDialog
  },
  name: "tutor",
  data() {
    return {
      settingDialogState: false,
      isBrowserSupportDialog:false,
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
      "getRoomIsNeedPayment",
      "getDialogUserConsent",
      "getZoom",
      "getPanX",
      "getPanY",
      "getReviewDialogState",
      "getDialogRoomEnd",
      "accountUser",
      "getIsRecording",
      "getShowAudioRecordingError",
      "getDialogSnapshot",
    ]),
    isRoomTutor(){
      return this.$store.getters.getRoomIsTutor;
    },
    isRoomActive(){
      return this.$store.getters.getRoomIsActive;
    },
    activeItem() {
      return this.$store.getters.getActiveNavEditor;
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
      "updateLockChat",
      "updateReviewDialog",
      "updateReview",
      "closeChat",
      "openChatInterface",
      "updateEndDialog",
      "setShowAudioRecordingError",
      "updateDialogUserConsent",
      "updateDialogSnapshot",
      "openChat"
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
      this.settingDialogState = true;
    },
    closeReviewDialog() {
      this.updateReviewDialog(false);
    },
    closeEndDialog() {
      this.updateEndDialog(false);
    },
    closeShowAudioRecordingError(){
      this.setShowAudioRecordingError(false);
    },
    updateActiveNav(value) {
      if(!this.$route.params.id || this.$route.params.id && this.isRoomTutor ){
        insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_navigation', {'roomId': this.id, 'userId': this.userId, 'navigatedTo': value}, null)
        this.$ga.event("tutoringRoom", `updateActiveNav:${value}`);
        this.$store.dispatch('updateActiveNavEditor',value)
        let transferDataObj = {
            type: "updateActiveNav",
            data: value
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
      let self = this;
      this.getChatById(this.$store.getters.getRoomConversationId).then(({ data }) => {
        insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_ChatById', data, null)
        let currentConversationObj = chatService.createActiveConversationObj(data);
        self.setActiveConversationObj(currentConversationObj);
        self.updateLockChat(true);
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
    },
    isBrowserSupport(){
      let agent = navigator.userAgent;
      if(agent.match(/Edge/)){
        return false;
      }
      return agent.match(/Firefox|Chrome|Safari/);
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
    this.updateLockChat(false);

    storeService.unregisterModule(this.$store,'tutoringCanvas');
    storeService.unregisterModule(this.$store,'roomRecording_store');
    storeService.unregisterModule(this.$store,'codeEditor_store');
  },
  beforeCreate(){
    storeService.registerModule(this.$store,'roomRecording_store',roomRecording_store);
    storeService.registerModule(this.$store,'tutoringCanvas',tutoringCanvas);
    storeService.registerModule(this.$store,'codeEditor_store',codeEditor_store);
  },
  async created() {
    this.$store.commit('clearComponent')
    this.userId = this.accountUser?.id || 'GUEST';

    if (!this.isBrowserSupport()) {
      this.$nextTick(()=>{
        this.isBrowserSupportDialog = true;
        let roomId = this.id ? this.id : 'No-Id';
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_BrowserNotSupported', {'roomId': roomId, 'userId': this.userId}, null)
      })
      return;
    }

    if(this.id){
      if(this.$store.getters.accountUser?.id){
        insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_Enter', {'roomId': this.id, 'userId': this.userId}, null) 
        this.$store.dispatch('updateStudyRoomInformation',this.id).catch((err)=>{
            if(err?.response){
              insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_RoomProps', err, null)
              this.$router.push('/')
            }
          })
        global.onbeforeunload = function() {     
          insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_beforeUnloadTriggered', {'roomId': this.id, 'userId': this.userId}, null)
          return "Are you sure you want to close the window?";
        };
      }else{
        this.$store.commit('setComponent', 'login');
      }
    }else{
      //TODO - we need one place to invoke this.
      this.initMathjax()
    }
  }
};
</script>

<style lang="less" src="./tutor.less"></style>