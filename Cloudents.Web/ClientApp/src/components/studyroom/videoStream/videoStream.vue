<template>
  <v-container class="videos-wrapper py-0 px-0">
    <v-layout column align-end>
      <div class="video-holder">
        <div :class="['remote_video_container',{'studentRemote': !$store.getters.getRoomIsTutor}]">
          <localVideoTrack />
          <remoteVideoTrack
            v-for="(track, index) in $store.getters.getVideoTrackList"
            :key="index"
            :track="track"
          />
          <div id="remoteTrack"></div>
        </div>
      </div>
    </v-layout>
  </v-container>
</template>
<script>
import remoteVideoTrack from "./components/remoteVideoTrack.vue";
import localVideoTrack from "./components/localVideoTrack.vue";

export default {
  name: "videoStream",
  components: {
    remoteVideoTrack,
    localVideoTrack
  }
};
</script>
<style lang="less">
@import "../../../styles/mixin.less";
.videos-wrapper {
  .video-holder {
    border-radius: 0 0 4px 4px;
    background-color: #ffffff;
    padding: 0px 10px 10px 10px;
    display: flex;
    flex-direction: column;
    width: 332px;

    .remote_video_container {
      position: relative;
      min-height: 229px;
      overflow: auto;
      height: 100%;
      // max-height: 100vh;
      max-height: 80vh;
      display: flex;
      flex-direction: column;
      &.studentRemote {
        :nth-child(2) {
          order: -1;
        }
      }
    }
  
  }
  // .video-con-controls{
  //   display: flex;
  //   justify-content: space-between;
  //   padding: 6px 0;
  // }
}
</style>