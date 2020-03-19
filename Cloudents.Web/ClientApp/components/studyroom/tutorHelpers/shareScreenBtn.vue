<template>
    <div class="share-screen-btn-wrap">
        <v-flex class="text-center">
            <div v-if="!getIsShareScreen" >
                <v-tooltip top >
                    <template v-slot:activator="{on}">
                        <div v-on="on">
                            <button @click="showScreen" class="outline-btn-share">
                                <castIcon class="cast-icon"></castIcon>
                                <span v-language:inner="'tutor_btn_share_screen'"></span>
                            </button>
                        </div>
                    </template>
                    <span v-language:inner="'tutor_start_to_share'"/>
                </v-tooltip>
            </div>
            <button class="outline-btn-share" v-else @click="stopSharing">
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
    export default {
        name: "shareScreenBtn",
        components: {castIcon},
        data() {
            return {
                isSafari: false,
                extensionDialog: false,
                extensionLink: `https://chrome.google.com/webstore/detail/${
                    videoService.extensionId
                    }`,
                number: 5                    
            };
        },
        computed: {
            ...mapState(['tutoringMain']),
            ...mapGetters(['getIsShareScreen',"accountUser", "getStudyRoomData", "getLocalVideoTrack","activeRoom"]),
            localVideoTrack(){
                return this.getLocalVideoTrack
            },
            accountUserID() {
                if(this.accountUser && this.accountUser.id) {
                    return this.accountUser.id;
                }else{
                    return null;
                }
            },
            roomIsActive() {
                return true
            },
            isTutor() {
                return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
            }
        },
        methods: {
            ...mapActions(["updateToasterParams"]),
            ...mapState(["Toaster"]),
            reloadPage() {
                global.reloadPage();
            },
            publishTrackToRoom(track) {
                if(this.activeRoom) {
                    this.activeRoom.localParticipant.videoTracks.forEach((LocalVideoTrackPublication) => {
                        this.unPublishTrackfromRoom(LocalVideoTrackPublication.track.mediaStreamTrack);
                    })
                    this.activeRoom.localParticipant.publishTrack(track, {
                        name: `shareScreen_${this.isTutor ? "tutor" : "student"}_${
                            this.accountUserID
                            }`
                    });
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
                this.$ga.event("tutoringRoom", 'screen share start');
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_ShareScreenBtn_Click', {id: this.$route.params.id}, null);
                this.$store.dispatch('updateShareScreen',true)

                // videoService.getUserScreen().then(
                //     stream => {
                //         store.dispatch('setLocalVideoTrack', stream);
                //         self.publishTrackToRoom(self.screenShareTrack);
                //     },
                //     error => {
                //         error = error || {};
                //         let d = {...{
                //             errorMessage:error.message,
                //             errorname:  error.name},
                //              ...{id: self.getStudyRoomData.roomId}};
                //         insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_ShareScreenBtn_showScreen', d, null);
                //         if(error === "noExtension") {
                //             self.extensionDialog = true;
                //             return;
                //         }
                //         if(error === "notBrowser") {
                //             self.updateToasterParams({
                //                 toasterText: this.$t("studyRoom_not_browser"),
                //                 showToaster: true,
                //                 toasterType: "error-toaster" //c
                //             });
                //             return;
                //         }
                //         if(error.name === "NotAllowedError") {
                //             if (error.message === "Permission denied") {
                //                 //user press cancel.
                //                 return;
                //             }
                //             if (error.message === "Permission denied by system") {
                //                  let url = 'https://support.apple.com/en-il/guide/mac-help/mchld6aa7d23/mac'
                //                  self.updateToasterParams({
                //                     toasterText: self.$t('studyRoom_premission_denied',[url]),
                //                     toasterTimeout: 30000,
                //                     showToaster: true,
                //                     toasterType: "error-toaster" //c
                //                 });
                //             }
                //             return

                           
                //         }
                //         self.updateToasterParams({
                //             toasterText: this.$t("studyRoom_not_screen"),
                //             showToaster: true,
                //             toasterType: "error-toaster" //c
                //         });
                //         console.error("error sharing screen", error);
                //     }
                // );
            },
            stopSharing() {
                this.$ga.event("tutoringRoom", 'screen stopSharing');
                this.$store.dispatch('updateShareScreen',false)
            }
        },
        created() {
            console.log('shareScreen init')
            this.isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && global.safari.pushNotification));
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