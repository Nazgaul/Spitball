<template>
    <v-container class="videos-wrapper py-0">

        <v-layout align-center justify-end>
            <v-flex xs8 class="d-inline-flex">
                <button v-show="!roomIsActive && !waitingStudent" class="create-session"  color="primary" :class="{'disabled': roomIsPending}" @click="enterRoom()">
                    <timerIcon class="timer-icon mr-2"></timerIcon>
                    <span v-show="isTutor" v-language:inner>tutor_stream_btn_start_session</span>
                    <span v-show="!isTutor" v-language:inner>tutor_stream_btn_join_session</span>
                </button>
                <button class="create-session" v-show="waitingStudent && isTutor">
                    <span v-language:inner>tutor_stream_btn_waiting</span>
                </button>

                <button v-show="roomIsActive && !waitingStudent" class="end-session"  @click="endSession()">
                    <stopIcon class="stop-icon mr-2"></stopIcon>
                    <span v-language:inner>tutor_stream_btn_end_session</span>

                </button>
            </v-flex>
        </v-layout>

        <v-layout column align-end>
            <div class="video-holder">
                <v-flex class="px-3 video-con-controls" @click="minimize('remote_player')">
                    <div style="display: flex; align-items: center;">
                        <span :class="[remoteOffline  ? 'remote-offline' : 'remote-online']"></span>
                        <span class="user-badge" v-language:inner>tutor_stream_guest</span>
                        <div id="micVolume_indicator" >

                        </div>
                    </div>
                    <div style="display: flex; align-items: center;">
                    <span class="video-size-ctrl mr-2" @click.stop="biggerRemoteVideo">
                        <span class="video-size-icon">
                            <fullScreenIcon class="full-screen-icon"></fullScreenIcon>
                        </span>
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