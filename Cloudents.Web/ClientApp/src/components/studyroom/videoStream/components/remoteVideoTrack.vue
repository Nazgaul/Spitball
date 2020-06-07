<template>
  <div class="remoteVideoStream" :id="track.sb_video_id"></div>
</template>

<script>
export default {
  name: "remoteVideo",
  props: {
    track: {
      type: Object,
      required: true
    }
  },
  methods: {
    openFullScreen() {
      var video = document.querySelector(`#${this.track.sb_video_id} video`);
      if (!video) return;

      if (video.requestFullscreen) {
        video.requestFullscreen();
        return;
      }
    }
  },
  mounted() {
    let previewContainer = document.getElementById(this.track.sb_video_id);
    previewContainer.appendChild(this.track.attach());
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin";
.remoteVideoStream {
  .fullscreen-btn {
    position: absolute;
    margin: 10px;
    right: 0;
    background: rgba(0, 0, 0, 0.7);
  }
  video {
    width: 100%;
    background-repeat: no-repeat;
    pointer-events: none;
    height: 100%;
  }

  video::-webkit-media-controls-enclosure {
    display: none !important;
  }
  @media (max-width: @screen-md) {
  display: none;
}  
}

&.fullscreenMode {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100vw;
  height: 100vh;
  z-index: 20;
  background: #000;
  display: block;
  @media (max-width: @screen-sm) and (orientation: portrait) {
    top: 0;
    left: 0;
    height: 50vh;
    bottom: 50%;
    right: 0;
  }
}
</style>