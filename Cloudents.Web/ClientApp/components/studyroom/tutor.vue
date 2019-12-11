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
        <button @click="closeWin" v-language:inner="'tutor_close'"></button>
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
              :class="{'active-nav': singleNav.value === activeItem, 'tutor-nav-disabled': singleNav.value !== 'white-board' && !id}"
              :key="index" :sel="`${singleNav.name.toLowerCase().replace(' ','_')}_tab`">
              <span class="dot-nav" v-if="singleNav.value === getActiveNavIndicator">●</span>
              <v-icon class="mr-2 nav-icon">{{singleNav.icon}}</v-icon>
              <a class="tutor-nav-item-link">{{singleNav.name}}</a>
            </div>
          </div>
          <div
            style="display: flex; align-items: center; max-height: 48px; justify-content: space-between;"
          >
            <startEndSessionBtn :id="id"></startEndSessionBtn>
          
            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px; margin-left:30px;" vertical></v-divider>  
            
            <v-btn flat icon @click="toggleRecord" v-if="isRecordingSupported">
              <v-icon v-if="!getIsRecording" class="white-btn">sbf-begain-recording</v-icon>
              <v-icon v-else class="white-btn">sbf-stop-recording</v-icon>
            </v-btn>
           
            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px;" vertical></v-divider>
            
            <div class="d-flex">
              <v-btn sel="help_draw" flat icon @click="showIntercom">
                <intercomSVG class="network-icon"/>
              </v-btn>
            </div> 
            
            

            <v-divider color="#000000" inset style="opacity: 0.12; height: 30px;" vertical></v-divider>
            
            <v-btn sel="setting_draw" flat icon @click="changeSettingsDialogState(true)">
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
            <v-flex shrink xs1>
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
                :disabled="!releaseFullVideoButton"
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
                  <video-stream :id="id"></video-stream>
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
      
      <sb-dialog
        :showDialog="getBrowserSupportDialog"
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
        :showDialog="getStudyRoomSettingsDialog"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'tutor-settings'"
        :maxWidth="'800'"
        :onclosefn="closeStudyRoomSettingsDialog"
        :activateOverlay="false"
        :content-class="'tutor-settings-dialog'"
      >
        <studyRoomSettingsDialog></studyRoomSettingsDialog>
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
      <!--show only if not avaliable devices dialog is closed by user-->
      <sb-dialog
        :showDialog="openStartSessionDialog && !getStudyRoomSettingsDialog"
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
      <!--end session confirmation-->
      <sb-dialog
        :showDialog="getEndDialog"
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
        :showDialog="getStudentStartDialog && !getStudyRoomSettingsDialog"
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
    </div>
  </v-layout>
</template>
<script>

import initSignalRService from "../../services/signalR/signalrEventService";
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
import networkLevel from "./tutorHelpers/networkLevel.vue";
import noSupportTop from "./images/not_supported_top.svg";
import noSupportBottom from "./images/not_supported_bottom.svg";
import tutorService from "./tutorService";
import chatService from "../../services/chatService";
import { LanguageService } from "../../services/language/languageService";
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
import paymentDialog from './tutorHelpers/paymentDIalog/paymentDIalog.vue'
import intercomSVG from './images/icon-1-2.svg'
import studyRoomRecordingService from './studyRoomRecordingService.js';
import errorWithAudioRecording from './tutorHelpers/errorWithAudioRecording/errorWithAudioRecording.vue';

//store
import storeService from "../../services/store/storeService";
import tutoringCanvas from '../../store/studyRoomStore/tutoringCanvas.js';
import tutoringMain from '../../store/studyRoomStore/tutoringMain.js';
import studyRoomTracks_store from '../../store/studyRoomStore/studyRoomTracks_store.js';
import codeEditor_store from '../../store/studyRoomStore/codeEditor_store.js';
import roomRecording_store from '../../store/studyRoomStore/roomRecording_store.js';
import studyroomSettings_store from '../../store/studyRoomStore/studyroomSettings_store';

import studyroomSettingsUtils from '../studyroomSettings/studyroomSettingsUtils';

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
    networkLevel,
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
    paymentDialog,
    codeEditorTools,
    intercomSVG,
    errorWithAudioRecording
  },
  name: "tutor",
  data() {
    return {      
      activeNavItem: "white-board",
      showSupportBrowser: false,
      showQualityDialog: false,
      showContent: false,
      navs: [
        {
          name: LanguageService.getValueByKey("tutor_nav_canvas"),
          value: "white-board",
          icon: "sbf-canvas"
        },
        {
          name: LanguageService.getValueByKey("tutor_nav_code"),
          value: "code-editor",
          icon: "sbf-code-editor"
        },
        {
          name: LanguageService.getValueByKey("tutor_nav_text"),
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
    id: ""
  },

  computed: {
    ...mapGetters([
      "getStudyRoomSettingsDialog",
      // "qualityDialog",
      "localNetworkQuality",
      "isRoomCreated",
      "getZoom",
      "getPanX",
      "getPanY",
      "getReviewDialogState",
      "getStudentStartDialog",
      "getTutorStartDialog",
      "getEndDialog",
      "getBrowserSupportDialog",
      "accountUser",
      "getShowPaymentDialog",
      "getStudyRoomData",
      "releaseFullVideoButton",
      "getActiveNavIndicator",
      "getRecorder",
      "isFrymo",
      "getRecorderStream",
      "getIsRecording",
      "getShowAudioRecordingError",
      "getVisitedSettingPage"      
    ]),
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
    browserSupportDialog(){
      return this.getBrowserSupportDialog;
    },
    needPayment() {
      if(!this.isTutor){
        return this.getStudyRoomData ? this.getStudyRoomData.needPayment : true;
      }else{
        return false
      }
    },
    isTutor() {
        return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
    },
    openStartSessionDialog(){
        return this.getTutorStartDialog
    },
    showPaymentDialog(){
      return this.getShowPaymentDialog
    },
    isCodeEditorActive(){
      return this.activeItem === "code-editor"
    },
    isRecordingSupported(){
      if(this.id){
        return !this.isTutor;
      }else{
        return tutorService.isRecordingSupported();
      }
    }
  },

watch: {
  showDeviceValidationError: function(val){
      if(val) {
        setTimeout(function() {
          document.querySelector('.device-dialog-unsupport').parentNode.style.zIndex=999;
        },1000)
      }
    }
},

  methods: {
    ...mapActions([
      "setStudyRoomSettingsDialog",
      "setActiveConversationObj",
      "getChatById",
      "lockChat",
      "updateStudyRoomProps",
      "updateReviewDialog",
      "updateReview",
      "submitReview",
      "updateTutorStartDialog",
      "updateStudentStartDialog",
      "closeChat",
      "openChatInterface",
      "updateEndDialog",
      "setBrowserSupportDialog",
      "setRoomId",
      "setVideoDevice",
      "setAudioDevice",
      "initLocalMediaTracks",
      "UPDATE_SEARCH_LOADING",
      "setShowAudioRecordingError",
      "hideRoomToasterMessage",
    ]),
    // ...mapGetters(['getDevicesObj']),
    closeFullScreen(e){
      if(!document.fullscreenElement || !document.webkitFullscreenElement || document.mozFullScreenElement){
       this.selectViewOption(this.enumViewOptions.videoChat)
      }
    },
    closeReviewDialog() {
      this.updateReviewDialog(false);
    },
    closeStudyRoomSettingsDialog(){
      this.setStudyRoomSettingsDialog(false);
    },
    closeEndDialog() {
      this.updateEndDialog(false);
    },
    closeStartSessionTutor() {
      this.updateTutorStartDialog(false);
    },
    closeStartSessionStudent() {
      this.updateStudentStartDialog(false);
    },
    closeShowAudioRecordingError(){
      this.setShowAudioRecordingError(false);
    },
    updateActiveNav(value) {
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_navigation', {'roomId': this.id, 'userId': this.userId, 'navigatedTo': value}, null)
      this.activeNavItem = value;
      let activeNavData = {
          activeNav: value,
      }
      let transferDataObj = {
          type: "updateActiveNav",
          data: activeNavData
      };
      let normalizedData = JSON.stringify(transferDataObj);
      tutorService.dataTrack.send(normalizedData);
      // {{singleNav.value}}
      console.log(this.activeItem);
    },
    changeSettingsDialogState(val) {
      this.setStudyRoomSettingsDialog(val);
      // this.updateTestDialogState(val);
    },
    
    selectViewOption(param) {
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
    async setStudyRoom(id) {
      let self = this;
      let _roomProps = this.getStudyRoomData;
      if(_roomProps){
        initSignalRService(`studyRoomHub?studyRoomId=${id}`);
      }else{
        await tutorService.getRoomInformation(id).then((RoomProps) => {
          _roomProps = RoomProps;
          insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_RoomProps', _roomProps, null)
          initSignalRService(`studyRoomHub?studyRoomId=${id}`);
          this.updateStudyRoomProps(_roomProps);
        }, err => {
          insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_RoomProps', err, null)
        });
      }
      self.getChatById(_roomProps.conversationId).then(({ data }) => {
        insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_ChatById', data, null)
        let currentConversationObj = chatService.createActiveConversationObj(data);
        self.setActiveConversationObj(currentConversationObj);
        self.lockChat();
      });
    },
    closeWin() {
      global.close();
    },
    closeBrowserSupportDialog(){ 
      this.setBrowserSupportDialog(false);
    },
    async initDevicesToStore(){
        let availableDevices = [];
        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.log("enumerateDevices() not supported.");
            return;
        }

        // List cameras and microphones.
        let devices = await navigator.mediaDevices.enumerateDevices();
        devices.forEach(function (device) {
            console.log(device.kind + ": " + device.label +
                " id = " + device.deviceId);
            availableDevices.push(device.kind);
        });
        //create local track with custom names
        let audioTrackName = `audio_${this.isTutor ? 'tutor' : 'student'}_${this.accountUser.id}`;
        let videoTrackName = `video_${this.isTutor ? 'tutor' : 'student'}_${this.accountUser.id}`;
        let audioSetObj = {
            audio: availableDevices.includes('audioinput'),
            name: audioTrackName
        };
        let videoSetObj = {
            video: availableDevices.includes('videoinput'),
            name: videoTrackName
        };
        let constraint = {
            video: videoSetObj.video ? true : false, 
            audio: audioSetObj.audio ? true : false
        }
        let ready = await navigator.mediaDevices.getUserMedia(constraint).then(() => {
            let audioDevice = audioSetObj.audio ? audioSetObj : false;
            let videoDevice = videoSetObj.video ? videoSetObj : false;
            this.setVideoDevice(videoDevice);
            this.setAudioDevice(audioDevice);
            return true;
        }, err=>{
          return false;
        })

        return ready;
    },
    resetItems(){
      let isExit = confirm(LanguageService.getValueByKey("login_are_you_sure_you_want_to_exit"),)
      if(isExit){
        this.UPDATE_SEARCH_LOADING(true);
        this.$router.push('/');
      }
    },
    showIntercom(){
      if(this.isFrymo){
        window.open('mailto: support@frymo.com', '_blank');
      }else{
        global.Intercom('show')
        intercomSettings.hide_default_launcher = false;
      }
    },
    toggleRecord(){
      studyRoomRecordingService.toggleRecord();
    
    }

  },
  mounted() {
    document.addEventListener("fullscreenchange",this.closeFullScreen);
    let isNotStudyRoomTest = this.$route.params ? this.$route.params.id : null;
    if(isNotStudyRoomTest) {
      let self = this;
      this.$nextTick(function(){
        setTimeout(()=>{
          if(self.isTutor){
            self.updateTutorStartDialog(true);
          }else{
            self.updateStudentStartDialog(true);
          }
        }, 1500)
      })
    }
  },
  beforeDestroy(){
    this.hideRoomToasterMessage();
    document.removeEventListener('fullscreenchange',this.closeFullScreen);
    storeService.unregisterModule(this.$store,'tutoringCanvas');
    // storeService.unregisterModule(this.$store,'tutoringMain');
    storeService.unregisterModule(this.$store,'studyRoomTracks_store');
    storeService.unregisterModule(this.$store,'codeEditor_store');
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
    // let ready = this.initDevicesToStore();
    
    if (!studyroomSettingsUtils.isBrowserSupport()) {
      this.$nextTick(()=>{
        this.setBrowserSupportDialog(true)
        let roomId = this.id ? this.id : 'No-Id';
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_main_BrowserNotSupported', {'roomId': roomId, 'userId': this.userId}, null)
      })
      return;
    }
    
    // in case refresh was made in studyRoom page, make sure to init local media tracks. (to be able to share video/audio)

    // this code will create an error object to know what is the cause of the problem in case there is one.
    // settings page is running this code, but we should run this code in case refresh was made in the study room page.
    // run this code only if refresh was made in the study room 
    if(!this.getVisitedSettingPage){
      await tutorService.validateUserMedia(true, true); 
    }
    //this line will init the tracks to show local medias
    studyroomSettingsUtils.validateMedia();



    this.userId = !!this.accountUser ? this.accountUser.id : 'GUEST';
    if(!!this.id){
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_Enter', {'roomId': this.id, 'userId': this.userId}, null)
      this.setRoomId(this.id);
      this.setStudyRoom(this.id);
    }
    this.$loadScript(
      "https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-AMS_SVG"
    ).then(() => {
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
      //MathJax.Message.Log()
    });
    console.log("ID Tutor!!", this.id);
    global.onbeforeunload = function() {     
      insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_main_beforeUnloadTriggered', {'roomId': this.id, 'userId': this.userId}, null)
      return "Are you sure you want to close the window?";
    };
  }
};
</script>

<style lang="less" src="./tutor.less"></style>
