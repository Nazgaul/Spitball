<template>
    <div class="share-screen-btn-wrap">
        <v-flex class="text-center">
            <div v-if="!isSharing" >
                <v-tooltip top >
                    <template v-slot:activator="{on}">
                        <!--keep this div, due to tooltip not appearing on disabled btn bug of vuetify-->
                        <div v-on="on" >
                            <button @click="showScreen" class="outline-btn-share" :disabled="(!roomIsActive && !isSafari) || (!roomIsActive && isSafari)">
                                <castIcon class="cast-icon"></castIcon>
                                <span v-language:inner="'tutor_btn_share_screen'"></span>
                            </button>
                        </div>
                    </template>
                    <span v-language:inner="isSafari? 'tutor_browser_not':'tutor_start_to_share'"/>
                </v-tooltip>
            </div>
            <button class="outline-btn-share" v-else @click="stopSharing" :disabled="!localVideoTrack && !activeRoom">
                <span v-language:inner="'tutor_btn_stop_sharing'"></span>
            </button>
        </v-flex>
        <v-dialog class="install-extension-dialog" v-model="extensionDialog" max-width="290">
            <v-card>
                <v-card-title class="headline">
                    <span v-language:inner="'tutor_chrome_ext_title'"></span>
                </v-card-title>

                <v-card-text>
                    <span v-language:inner="'tutor_chrome_ext_install'"></span>
                </v-card-text>
                <v-card-text>
                    <a>
                        <span @click="reloadPage()" v-language:inner="'tutor_chrome_ext_text'"></span>
                    </a>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <a
                            :href="extensionLink"
                            target="_blank"
                            class="btn px-3 py-2 mr-4"
                            @click="dialog = false"
                    >
                        <span v-language:inner="'tutor_chrome_ext_btn_install'"></span>
                    </a>
                    <v-btn color="green darken-1" text @click="extensionDialog = false">
                        <span v-language:inner="'tutor_chrome_ext_btn_cancel'"></span>
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script>
    import { mapActions, mapGetters, mapState } from "vuex";
    import videoService from "../../../services/videoStreamService";
    import castIcon from "../images/cast.svg";
    import insightService from '../../../services/insightService';
    import store from '../../../store/index.js';
    import { LanguageService } from "../../../services/language/languageService.js";
    export default {
        name: "shareScreenBtn",
        components: {castIcon},
        data() {
            return {
                isSafari: false,
                isSharing: false,
                extensionDialog: false,
                extensionLink: `https://chrome.google.com/webstore/detail/${
                    videoService.extensionId
                    }`,
                number: 5                    
            };
        },
        computed: {
            ...mapState(['tutoringMain', 'studyRoomTracks_store']),
            ...mapGetters(["activeRoom", "accountUser", "getStudyRoomData", "getCurrentRoomState", "getLocalVideoTrack","activeRoom"]),
            localVideoTrack(){
                return this.getLocalVideoTrack
            },
            accountUserID() {
                if(this.accountUser && this.accountUser.id) {
                    return this.accountUser.id;
                }
            },
            roomIsActive() {
                return this.getCurrentRoomState === this.tutoringMain.roomStateEnum.active;
            },
            isTutor() {
                return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
            }
        },
        methods: {
            ...mapActions(["updateToasterParams", 'changeVideoTrack', 'setIsVideoActive']),
            ...mapState(["Toaster"]),
            reloadPage() {
                global.reloadPage();
            },
            publishTrackToRoom(track) {
                if(this.activeRoom) {
                    this.activeRoom.localParticipant.videoTracks.forEach(LocalVideoTrackPublication =>{
                        this.unPublishTrackfromRoom(LocalVideoTrackPublication.track.mediaStreamTrack);
                    })
                    this.activeRoom.localParticipant.publishTrack(track, {
                        name: `shareScreen_${this.isTutor ? "tutor" : "student"}_${
                            this.accountUserID
                            }`
                    });
                    this.setIsVideoActive(true);
                }
            },
            unPublishTrackfromRoom(track) {
                if(!track) return;
                if(this.activeRoom) {
                    this.activeRoom.localParticipant.unpublishTrack(track);
                }
            },
            //screen share start
            showScreen() {
                let self = this;
                videoService.getUserScreen().then(
                    stream => {
                        stream.removeEventListener('ended', () => self.stopSharing());
                        stream.addEventListener('ended', () => self.stopSharing());
                        store.dispatch('setLocalVideoTrack', stream);
                        self.screenShareTrack = stream; //stream.getVideoTracks()[0];
                        self.publishTrackToRoom(self.screenShareTrack);
                        self.isSharing = true;
                    },
                    error => {
                        error = error || {};
                        if(error === "noExtension") {
                            self.extensionDialog = true;
                            return;
                        }
                        if(error === "notBrowser") {
                            self.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("studyRoom_not_browser"),
                                showToaster: true,
                                toasterType: "error-toaster" //c
                            });
                            return;
                        }
                        if(error.name === "NotAllowedError") {
                            //user press cancel.
                            return;
                        }
                        self.updateToasterParams({
                                                     toasterText: LanguageService.getValueByKey("studyRoom_not_screen"),
                                                     showToaster: true,
                                                     toasterType: "error-toaster" //c
                                                 });

                        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_ShareScreenBtn_showScreen', error, null);
                        console.error("error sharing screen", error);
                    }
                );
            },
            stopSharing() {
                if(this.screenShareTrack){
                    this.screenShareTrack.stop();
                }
                let videoDeviceId = global.localStorage.getItem(this.studyRoomTracks_store.storageENUM.video);
                this.changeVideoTrack(videoDeviceId);
                this.isSharing = false;
            }
        },
        created() {
            this.isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));
        },
    };
</script>

<style lang="less">
    .share-screen-btn-wrap {
        .outline-btn-share {
            display: inline-flex;
            align-items: center;
            justify-content: space-between;
            padding: 8px 12px;
            /*border-radius: 4px;*/
            /*border: solid 2px #a5a4bf;*/
            /*background-color: rgba(165, 164, 191, 0.1);*/
            font-size: 12px;
            line-height: 1.27;
            letter-spacing: 0.5px;
            color: #2d2d2d;
            /*color: #ffffff;*/
            .cast-icon {
                fill: #2d2d2d;
                margin-right: 4px;
            }
            &[disabled]{
                color: lighten(#2d2d2d, 40%);
                .cast-icon{
                    fill: lighten(#2d2d2d, 40%);
                }
            }
        }

    }
</style>