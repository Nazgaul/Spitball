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
              <AppLogo></AppLogo>
            </span>
            <div
              class="tutor-nav-item cursor-pointer"
              @click="updateActiveNav(singleNav.value)"
              v-for="(singleNav, index) in navs"
              :class="{'active-nav': singleNav.value === activeItem}"
              :key="index">
              <v-icon class="mr-2 nav-icon">{{singleNav.icon}}</v-icon>
              <a class="tutor-nav-item-link">{{singleNav.name}}</a>
            </div>
          </div>
          <div
            style="display: flex; align-items: center; max-height: 48px; width: 320px; justify-content: space-between;"
          >
            <startEndSessionBtn :id="id"></startEndSessionBtn>
            <v-menu bottom origin="center center" transition="scale-transition">
              <template v-slot:activator="{ on }">
                <v-btn flat icon v-on="on">
                  <settingIcon class="white-btn"></settingIcon>
                </v-btn>
                <v-divider color="#000000" inset style="opacity: 0.12; height: 30px;" vertical></v-divider>
              </template>
              <v-list>
                <v-list-tile @click="changeQualityDialogState(true)">
                  <v-list-tile-action>
                    <testIcon class="test-icon mr-1"></testIcon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>
                      <span v-language:inner>tutor_btn_system_check</span>
                    </v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list>
            </v-menu>

            <div class="d-flex pr-4">
              <networkLevel class="network-icon ml-3" :signalLevel="localNetworkQuality"></networkLevel>
            </div>
          </div>
        </nav>
        <v-flex xs12   class="study-tools-wrapper">
          <v-layout class="pl-2" align-center shrink>
            <v-flex shrink class="canvas-tools-wrapper" v-if="isWhiteBoardActive">
              <whiteBoardTools></whiteBoardTools>
            </v-flex>
            <v-spacer></v-spacer>
            <v-flex xs1  >
              <share-screen-btn class="nav-share-btn"></share-screen-btn>
            </v-flex>
            <v-flex shrink class="controls-holder">
              <v-btn
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                @click.stop="selectViewOption(enumViewOptions.videoChat)"
                :input-value="activeViewOption == enumViewOptions.videoChat"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_videoChat</span>
              </v-btn>
              <v-btn
                @click="selectViewOption(enumViewOptions.fullScreenVideo)"
                class="control-btn text-capitalize elevation-0 cursor-pointer"
                :input-value="activeViewOption == enumViewOptions.fullScreenVideo"
                active-class="v-btn--active control-btn-active"
              >
                <span v-language:inner>tutor_option_videoFull</span>
              </v-btn>
              <v-btn
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

      <v-dialog
        v-model="qualityDialog"
        content-class="quality-dialog"
        :fullscreen="$vuetify.breakpoint.xsOnly"
        persistent
      >
        <quality-validation></quality-validation>
      </v-dialog>

      <sb-dialog
        :showDialog="getReviewDialogState "
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
        :showDialog="getTutorStartDialog && !qualityDialog"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'startSessionTutor'"
        :maxWidth="'356'"
        :onclosefn="closeStartSessionTutor"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
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
        :showDialog="getStudentStartDialog && !qualityDialog"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'startSessionStudent'"
        :maxWidth="'356'"
        :onclosefn="closeStartSessionStudent"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'session-start-student-dialog'"
      >
        <startSessionStudent :id="id"></startSessionStudent>
      </sb-dialog>
    </div>
  </v-layout>
</template>
<script>
import initSignalRService from "../../services/signalR/signalrEventService";
import { mapActions, mapGetters } from "vuex";
import videoStream from "./videoStream/videoStream.vue";
import whiteBoard from "./whiteboard/WhiteBoard.vue";
import codeEditor from "./codeEditor/codeEditor.vue";
import qualityValidation from "./tutorHelpers/qualityValidation/qualityValidation.vue";
import sharedDocument from "./sharedDocument/sharedDocument.vue";
import shareScreenBtn from "./tutorHelpers/shareScreenBtn.vue";
import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
import testIcon from "./images/eq-system.svg";
import chatIcon from "../../font-icon/message-icon.svg";
import settingIcon from "../../font-icon/settings.svg";
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

export default {
  components: {
    videoStream,
    whiteBoard,
    codeEditor,
    sharedDocument,
    shareScreenBtn,
    AppLogo,
    qualityValidation,
    testIcon,
    chatIcon,
    settingIcon,
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
    browserSupport
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
      activeViewOption: "videoChat"
    };
  },

  props: {
    id: ""
  },
  computed: {
    ...mapGetters([
      "qualityDialog",
      "localNetworkQuality",
      "isRoomCreated",
      "getZoom",
      "getPanX",
      "getPanY",
      "getReviewDialogState",
      "getStudentStartDialog",
      "getTutorStartDialog",
      "getEndDialog",
      "getBrowserSupportDialog"
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
    
  },
  methods: {
    ...mapActions([
      "updateTestDialogState",
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
      "setBrowserSupportDialog"
    ]),
    closeReviewDialog() {
      this.updateReviewDialog(false);
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
    updateActiveNav(value) {
      this.activeNavItem = value;
      console.log(this.activeItem);
    },
    changeQualityDialogState(val) {
      this.updateTestDialogState(val);
    },
    selectViewOption(param) {
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
    setStudyRoom(id) {
      let self = this;
      tutorService.getRoomInformation(id).then(({ data }) => {
        initSignalRService(`studyRoomHub?studyRoomId=${id}`);
        let roomData = { ...data, roomId: id };
        this.updateStudyRoomProps(roomData);
        self.getChatById(data.conversationId).then(({ data }) => {
          let currentConversationObj = chatService.createActiveConversationObj(
            data
          );
          self.setActiveConversationObj(currentConversationObj);
          self.lockChat();
        });
      });
    },
    closeWin() {
      global.close();
    },
    closeBrowserSupportDialog(){ 
      this.setBrowserSupportDialog(false);
    },
    isBrowserSupport(){
      let agent = navigator.userAgent;
      return agent.match(/Firefox|Chrome|Safari/);
    }
  },
  created() {
    if (!this.isBrowserSupport()) {
      this.$nextTick(()=>{
        this.setBrowserSupportDialog(true)
      })
    }
    this.setStudyRoom(this.id);
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
      return "Are you sure you want to close the window?";
    };
  }
};
</script>

<style lang="less" src="./tutor.less"></style>
