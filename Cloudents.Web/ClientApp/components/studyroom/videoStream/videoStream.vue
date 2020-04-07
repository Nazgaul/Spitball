<template>
    <v-container class="videos-wrapper py-0 px-0">
        <v-layout column align-end>
            <div class="video-holder">
                <v-flex class="px-3 video-con-controls">
                </v-flex>
                <v-flex>
                    <div class="remote_video_container">
                        <localVideoTrack/>
                        <remoteVideoTrack v-for="(track, index) in $store.getters.getVideoTrackList" :key="index" :track="track"/>
                        <div id="remoteTrack"></div>
                    </div>
                </v-flex>
            </div>
        </v-layout>
    </v-container>
</template>
<script>
import remoteVideoTrack from './components/remoteVideoTrack.vue';
import localVideoTrack from './components/localVideoTrack.vue';

export default {
    name: "videoStream",
    components: { 
        remoteVideoTrack,
        localVideoTrack
    },
};
</script>
<style lang="less">
.videos-wrapper {
  .video-holder{
    border-radius: 0 0 4px 4px;
    background-color: #ffffff;
    padding: 0px 10px 10px 10px;
    display: flex;
    flex-direction: column;
    width: 320px;

    .remote_video_container{
      position: relative;
      min-height: 229px;
    }
  }
  .video-con-controls{
    display: flex;
    justify-content: space-between;
    padding: 6px 0;
  }
}
</style>