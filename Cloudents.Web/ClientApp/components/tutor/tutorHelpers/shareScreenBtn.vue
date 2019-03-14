<template>
    <div>
        <v-flex>
            <v-btn v-if="!isSharing" @click="showScreen">Share Screen</v-btn>
            <v-btn v-else @click="stopSharing">Stop Sharing</v-btn>
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

    export default {
        name: "shareScreenBtn",
        data() {
            return {
                isSharing: false,
                extensionDialog: false,
                extensionLink: `https://chrome.google.com/webstore/detail/hicolpoppnllddloocbcjfeoijgjfdeg`
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

<style scoped>

</style>