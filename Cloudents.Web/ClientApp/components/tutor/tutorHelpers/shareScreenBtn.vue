<template>
    <v-flex>
        <v-btn v-if="!isSharing" @click="showScreen">Share Screen</v-btn>
        <v-btn v-else @click="stopSharing">Stop Sharing</v-btn>
    </v-flex>
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
            }
        },
        computed: {
            ...mapGetters(['activeRoom'])
        },

        methods: {
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
                        // self.screenShareTrack = null;
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