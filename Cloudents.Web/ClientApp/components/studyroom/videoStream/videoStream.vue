<template>
    <v-container class="videos-wrapper py-0 px-0">
        <v-layout column align-end>
            <div class="video-holder">
                <v-flex class="px-3 video-con-controls">
                </v-flex>
                <v-flex v-show="visible.remote_player">
                    <div class="remote_video_container">
                        <remoteVideoTrack v-for="(track, index) in $store.getters.getVideoTrackList" :key="index" :track="track"/>
                        <div id="remoteTrack"></div>
                        <localStreams/>
<!-- 
                        <div class="local-video-holder" :class="{'extended-video-window': extandLocalVideoScreen}">
                            <div v-show="!isVideoActive && !getIsShareScreen" class="localTrack-placeholder">
                                <div class="placeholder-back">
                                    <videoCameraImageIgnore2 class="placeholder-svg" />
                                </div>
                            </div>
                            <div v-show="getIsShareScreen" class="localTrack-placeholder share-screen">
                                <div class="placeholder-back share-screen">
                                    <castIcon class="placeholder-svg"></castIcon>
                                </div>
                            </div>
                            <div v-show="isVideoActive && !getIsShareScreen" style="width:104px;height:80px" id="localTrack"></div>          
                        </div> -->
                        <!-- <div class="control-panel">
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <button sel="audio_enabling" v-on="on" :class="['mic-image-btn', localAudioTrack && activeRoom ? 'dynamicBackground-dark': 'dynamicBackground-light', !isAudioActive ? 'micIgnore':'']" @click="toggleAudio">   
                                        <microphoneImage v-if="isAudioActive" class="mic-image-svg" />
                                        <microphoneImageIgnore v-if="!isAudioActive" class="mic-ignore" />           
                                    </button>
                                </template>
                                <span v-language:inner="isAudioActive ? 'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute'"/>
                            </v-tooltip>
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <button 
                                    sel="video_enabling" v-on="on" 
                                    :class="[
                                        'video-image-btn', 
                                        localVideoTrack && activeRoom ? 'dynamicBackground-dark': 'dynamicBackground-light', 
                                        !isVideoActive ? 'camIgnore':''
                                    ]" 
                                    @click="toggleVideo">              
                                        <videoCameraImage v-if="isVideoActive" class="video-image-svg"/>
                                        <videoCameraImageIgnore v-else class="cam-ignore"/>
                                    </button>
                                </template>
                                <span v-language:inner="isVideoActive ? 'tutor_tooltip_video_pause':'tutor_tooltip_video_resume'"/>
                            </v-tooltip>
                        </div> -->
                    </div>
                </v-flex>
            </div>
        </v-layout>
    </v-container>
</template>

<script src="./videoStream.js"></script>
<style lang="less" src="./videoStream.less"></style>