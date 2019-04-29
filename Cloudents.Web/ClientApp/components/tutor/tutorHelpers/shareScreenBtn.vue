<template>
    <div class="share-screen-btn-wrap">
        <v-flex>
            <button v-if="!isSharing" @click="showScreen" class="outline-btn">
                    <castIcon  class="cast-icon"></castIcon>
                Share Screen</button>
            <button class="outline-btn" v-else @click="stopSharing">Stop Sharing</button>
        </v-flex>
        <v-dialog class="install-extension-dialog"
                v-model="extensionDialog"
                max-width="290"
        >
            <v-card>
                <v-card-title class="headline">Chrome Extension Installation</v-card-title>
                <v-card-text>
                    Please install and authorize the Spitball Chrome extension to enable screen sharing on your computer.
                </v-card-text>
                <v-card-text>
                    Once the extension is installed please
                    <a @click="reloadPage()">reload</a>
                    the page for the screen sharing feature to be effective.
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <a :href="extensionLink"
                            target="_blank"
                           class="btn px-3 py-2 mr-3"
                            @click="dialog = false"
                    >Install</a>
                    <v-btn
                            color="green darken-1"
                            flat="flat"
                            @click="extensionDialog = false" >Cancel
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import { createLocalVideoTrack } from 'twilio-video';
    import videoService from "../../../services/videoStreamService";
    import castIcon from '../images/cast.svg'
    export default {
        name: "shareScreenBtn",
        components: {castIcon},
        data() {
            return {
                isSharing: false,
                extensionDialog: false,
                extensionLink: `https://chrome.google.com/webstore/detail/${videoService.extensionId}`
            }
        },
        computed: {
            ...mapGetters(['activeRoom'])
        },

        methods: {
            reloadPage(){
                global.location = global.location;
            },
            publishTrackToRoom(track) {
                this.activeRoom.localParticipant.publishTrack(track);
            },
            unPublishTrackfromRoom(track) {
                this.activeRoom.localParticipant.unpublishTrack(track);
            },
            //screen share start
            showScreen() {
                let self = this;
                videoService.getUserScreen().then((stream) => {
                        self.screenShareTrack = stream.getVideoTracks()[0];
                        self.publishTrackToRoom(self.screenShareTrack);
                        self.isSharing = true;
                    },
                    (error) => {
                        if (error === 'noExtension') {
                            self.extensionDialog = true;
                        }
                        console.log('error sharing screen')
                    }
                );
            },
            stopSharing() {
                let self = this;
                self.unPublishTrackfromRoom(self.screenShareTrack);
                //create new video track
                createLocalVideoTrack().then((videoTrack) => {
                        self.publishTrackToRoom(videoTrack);
                        self.isSharing = false;
                    },
                    (error) => {
                        console.log('error creating video track')
                    }
                );
            },
        },
    }
</script>

<style lang="less">
    .share-screen-btn-wrap{
        .outline-btn{
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 8px 12px;
            border-radius: 4px;
            border: solid 2px #a5a4bf;
            background-color: rgba(165, 164, 191, 0.1);
            font-size: 11px;
            line-height: 1.27;
            letter-spacing: 0.5px;
            color: #ffffff;
        }
        .cast-icon{
            fill: #ffffff;
            height: 16px;
            margin-right: 4px/*rtl:ignore*/;

        }
    }

</style>