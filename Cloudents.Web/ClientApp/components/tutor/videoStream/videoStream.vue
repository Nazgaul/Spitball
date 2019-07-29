<template>
    <v-container class="videos-wrapper py-0 px-0">
        <v-layout column align-end>
            <div class="video-holder">
                <v-flex class="px-3 video-con-controls">
                </v-flex>
                <v-flex v-show="visible.remote_player">
                    <div class="row remote_video_container">
                        <div id="remoteTrack"></div>

                        <div class="local-video-holder">
                            <div v-show="!isActive" class="localTrack-placeholder">
                                <div class="placeholder-back">
                                    <videoCameraImageIgnore2 class="placeholder-svg" />
                                </div>
                            </div>
                            <div v-show="isActive" id="localTrack"></div>
                        </div>
                        <div class="control-panel">
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <button v-on="on" :class="['mic-image-btn',localAudioTrack? 'dynamicBackground-light': 'dynamicBackground-dark']" @click="toggleAudio">   
                                        <microphoneImage v-if="localAudioTrack" class="mic-image-svg" />
                                        <microphoneImageIgnore v-if="!localAudioTrack" class="mic-ignore" />           
                                    </button>
                                </template>
                                <span v-language:inner="localAudioTrack ? 'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute'"/>
                            </v-tooltip>
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    
                                    <button v-on="on" :class="['video-image-btn',localVideoTrack? 'dynamicBackground-light': 'dynamicBackground-dark']" @click="toggleVideo">              
                                        <videoCameraImage v-if="localVideoTrack" class="video-image-svg"/>
                                        <videoCameraImageIgnore v-else class="cam-ignore"/>
                                    </button>
                                </template>
                                <span v-language:inner="localVideoTrack ? 'tutor_tooltip_video_pause':'tutor_tooltip_video_resume'"/>
                            </v-tooltip>
                        </div>
                    </div>
                </v-flex>
            </div>
        </v-layout>
    </v-container>
</template>

<script src="./videoStream.js"></script>
<style lang="less" src="./videoStream.less"></style>