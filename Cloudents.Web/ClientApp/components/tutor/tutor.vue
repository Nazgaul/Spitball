<template>
    <v-layout
            row
            class="tutoring-page"
            :style="{'background-size': zoom, 'background-position-x': panX, 'background-position-y': panY}"
            :class="{'gridBackground': $route.name === 'tutoring'} "
    >
        <v-flex>
            <nav class="tutoring-navigation">
                <div class="logo-nav-wrap">
          <span class="logo-container">
            <AppLogo></AppLogo>
          </span>
                    <div class="tutor-nav-item cursor-pointer" @click="updateActiveNav(singleNav.value)"
                         v-for="singleNav in navs" :class="{ 'active-nav': singleNav.value === activeItem}">
                        <v-icon class="mr-2 nav-icon">{{singleNav.icon}}</v-icon>
                        <a class="tutor-nav-item-link">{{singleNav.name}}</a>
                    </div>
                </div>
                <div style="display: flex; align-items: center; max-height: 48px;">
                    <startEndSessionBtn :id="id"></startEndSessionBtn>
                    <!--<share-screen-btn class="nav-share-btn"></share-screen-btn>-->
                    <v-menu
                            bottom
                            origin="center center"
                            transition="scale-transition"
                    >
                        <template v-slot:activator="{ on }">
                            <v-btn flat icon v-on="on" class="mx-3">
                                <settingIcon class="white-btn"></settingIcon>
                            </v-btn>
                            <v-divider color="#eee7e9"
                                    inset
                                    vertical
                            ></v-divider>
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

                            <v-list-tile @click="">
                                <v-list-tile-action>
                                   <chat-icon></chat-icon>
                                </v-list-tile-action>
                                <v-list-tile-content>
                                    <v-list-tile-title>
                                        <span v-language:inner>tutor_hide_chat</span>
                                    </v-list-tile-title>
                                </v-list-tile-content>
                            </v-list-tile>
                        </v-list>
                    </v-menu>

                    <div class="mr-4 pr-1 d-flex">
                        <component
                                class="network-icon ml-3"
                                :is="localNetworkQuality ? 'signal_level_'+localNetworkQuality : 'signal_level_0'"
                        ></component>
                    </div>
                </div >
            </nav>
            <whiteBoardTools></whiteBoardTools>
            <transition name="slide-x-transition">
                <keep-alive>
                    <component :is="activeItem" :roomId="id"></component>
                </keep-alive>
            </transition>
        </v-flex>
        <v-layout column align-start class="video-stream-wraper">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
        </v-layout>
        <v-dialog
                v-model="qualityDialog"
                content-class="quality-dialog"
                :fullscreen="$vuetify.breakpoint.xsOnly"
                persistent
        >
            <quality-validation></quality-validation>
        </v-dialog>
        <sb-dialog :showDialog="getReviewDialogState "
                   :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
                   :popUpType="'reviewDilaog'"
                   :maxWidth="'596'"
                   :onclosefn="closeReviewDialog"
                   :activateOverlay="false"
                   :isPersistent="$vuetify.breakpoint.smAndUp"
                   :content-class="'review-dialog'">
            <leave-review></leave-review>
        </sb-dialog>
        <!--show only if not avaliable devices dialog is closed by user-->
        <sb-dialog :showDialog="getTutorStartDialog && !qualityDialog"
                   :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
                   :popUpType="'startSessionTutor'"
                   :maxWidth="'356'"
                   :onclosefn="closeStartSessionTutor"
                   :activateOverlay="false"
                   :isPersistent="$vuetify.breakpoint.smAndUp"
                   :content-class="'session-start-tutor-dialog'">
            <startSessionTutor :id="id"></startSessionTutor>
        </sb-dialog>
        <!--show only if not avaliable devices dialog is closed by user-->
        <sb-dialog :showDialog="getStudentStartDialog && !qualityDialog"
                   :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
                   :popUpType="'startSessionStudent'"
                   :maxWidth="'356'"
                   :onclosefn="closeStartSessionStudent"
                   :activateOverlay="false"
                   :isPersistent="$vuetify.breakpoint.smAndUp"
                   :content-class="'session-start-student-dialog'">
            <startSessionStudent :id="id"></startSessionStudent>
        </sb-dialog>
    </v-layout>
</template>
<script>
    import initSignalRService from '../../services/signalR/signalrEventService';
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
    import signal_level_0 from "./images/wifi-0.svg";
    import signal_level_1 from "./images/wifi-1.svg";
    import signal_level_2 from "./images/wifi-2.svg";
    import signal_level_3 from "./images/wifi-3.svg";
    import signal_level_4 from "./images/wifi-4.svg";
    import signal_level_5 from "./images/wifi-4.svg";
    import tutorService from "./tutorService";
    import chatService from "../../services/chatService";
    import { LanguageService } from "../../services/language/languageService";
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
    import leaveReview from './tutorHelpers/leaveReview/leaveReview.vue';
    import startSessionTutor from './tutorHelpers/startSession-popUp-tutor/startSession-popUp-Tutor.vue';
    import startSessionStudent from './tutorHelpers/startSession-popUp-student/startSession-popUp-student.vue';
    import whiteBoardTools from './whiteboard/whiteboardTools.vue'
    import startEndSessionBtn from './tutorHelpers/startEndSessionBtn/startEndSessionBtn.vue';


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
            signal_level_0,
            signal_level_1,
            signal_level_2,
            signal_level_3,
            signal_level_4,
            signal_level_5,
            sbDialog,
            leaveReview,
            startSessionTutor,
            startSessionStudent,
            whiteBoardTools,
            startEndSessionBtn
        },
        name: "tutor",
        data() {
            return {
                activeNavItem: "white-board",
                showQualityDialog: false,
                showContent: false,
                navs: [
                    {name: LanguageService.getValueByKey("tutor_nav_canvas"), value: "white-board", icon: "sbf-canvas"},
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
                              "getTutorStartDialog"
                          ]),
            activeItem() {
                return this.activeNavItem;
            },
            showCurrentCondition() {
                return this.activeItem === "white-board" ? true : true;
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
            }
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
                              "updateStudentStartDialog"
                          ]),
            closeReviewDialog() {
                this.updateReviewDialog(false);
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

            setStudyRoom(id) {
                let self = this;
                tutorService.getRoomInformation(id).then(({data}) => {
                    initSignalRService(`studyRoomHub?studyRoomId=${id}`);
                    let roomData = {...data, roomId: id};
                    this.updateStudyRoomProps(roomData);
                    self.getChatById(data.conversationId).then(({data}) => {
                        let currentConversationObj = chatService.createActiveConversationObj(data);
                        self.setActiveConversationObj(currentConversationObj);
                        self.lockChat();
                    });
                });
            }
        },
        created() {
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
                MathJax.AuthorInit = function (texstring, callback) {
                    var input = texstring;
                    var wrapper = document.createElement("div");
                    wrapper.innerHTML = input;
                    var output = {svg: ""};
                    MathJax.Hub.Queue(["Typeset", MathJax.Hub, wrapper]);
                    MathJax.Hub.Queue(function () {
                        var mjOut = wrapper.getElementsByTagName("svg")[0];
                        if(!mjOut) {
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
            global.onbeforeunload = function () {
                return "Are you sure you want to close the window?";
            };
        }
    };
</script>

<style lang="less" src="./tutor.less"></style>