<template>
    <v-container class="videos-wrapper py-0">
        <v-layout>
            <v-flex>
                <div class="roomTitle">
                    <span v-if="loading"> Loading...</span>
                </div>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex>
                <v-btn v-if="!isSharing" @click="showScreen">Share Screen</v-btn>
                <v-btn v-else @click="stopSharing">Stop Sharing</v-btn>
                <v-btn class="create-session" color="primary" @click="createRoomFunc()" v-if="!id">Initiate tutoring
                    session
                </v-btn>
            </v-flex>
        </v-layout>

        <v-layout column align-end v-show="loaded">
            <div class="video-holder">
                <v-flex class="px-3 video-con-controls" @click="minimize('remote_player')">
                    <div style="display: flex; align-items: center;">
                        <span :class="[remoteOffline  ? 'remote-offline' : 'remote-online']"></span>
                        <span class="user-badge ml-2">Guest</span>
                    </div>
                    <div style="display: flex; align-items: center;">
                    <span class="video-size-ctrl mr-2" @click.stop="biggerRemoteVideo">
                        <v-icon class="video-size-icon">sbf-expand-icon</v-icon>
                    </span>
                        <span class="video-size-ctrl" @click.stop="minimize('remote_player')">
                         <v-icon v-if="visible.remote_player" class="video-size-icon">sbf-minimize</v-icon>
                         <v-icon v-else class="video-size-icon">sbf-toggle-enlarge</v-icon>
                    </span>
                    </div>
                </v-flex>
                <v-flex v-show="visible.remote_player">
                    <div class="row remote_video_container">
                        <div id="remoteTrack"></div>
                        <div class="local-video-holder">
                            <div id="localTrack"></div>
                        </div>

                        </div>
                </v-flex>
            </div>
        </v-layout>
    </v-container>
</template>

<script src="./videoStream.js"></script>
<style lang="less" src="./videoStream.less"></style>