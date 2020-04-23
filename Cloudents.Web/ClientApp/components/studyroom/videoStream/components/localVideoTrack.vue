<template>
  <div class="localVideoStream">
    <div class="local-video-holder">
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
      <div v-show="isVideoActive && !getIsShareScreen" id="localTrack">
        <template v-if="$store.getters.getRoomIsTutor">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-btn v-on="on" id="openFullTutor" class="fullscreen-btn" icon @click="openFullScreen" color="white">
                <v-icon>sbf-fullscreen</v-icon>
              </v-btn>
            </template>
            <span>{{$t('tutor_tooltip_fullscreen')}}</span>
          </v-tooltip>
        </template>
      </div>
    </div>
    <div class="control-panel">
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <button
            sel="audio_enabling"
            v-on="on"
            :class="['mic-image-btn', localAudioTrack && activeRoom ? 'dynamicBackground-dark': 'dynamicBackground-light', !isAudioActive ? 'micIgnore':'']"
            @click="toggleAudio"
          >
            <microphoneImage v-if="isAudioActive" class="mic-image-svg" />
            <microphoneImageIgnore v-if="!isAudioActive" class="mic-ignore" />
          </button>
        </template>
        <span
          v-language:inner="isAudioActive ? 'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute'"
        />
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <button
            sel="video_enabling"
            v-on="on"
            :class="[
                        'video-image-btn', 
                        localVideoTrack && activeRoom ? 'dynamicBackground-dark': 'dynamicBackground-light', 
                        !isVideoActive ? 'camIgnore':''
                  ]"
            @click="toggleVideo"
          >
            <videoCameraImage v-if="isVideoActive" class="video-image-svg" />
            <videoCameraImageIgnore v-else class="cam-ignore" />
          </button>
        </template>
        <span
          v-language:inner="isVideoActive ? 'tutor_tooltip_video_pause':'tutor_tooltip_video_resume'"
        />
      </v-tooltip>
    </div>
  </div>
</template>

<script>
import castIcon from "../../images/cast.svg";
import microphoneImage from "../../images/outline-mic-none-24-px-copy-2.svg";
import microphoneImageIgnore from "../../images/mic-ignore.svg";
import videoCameraImageIgnore from "../../images/camera-ignore.svg";
import videoCameraImageIgnore2 from "../../images/camera-ignore-big.svg";
import videoCameraImage from "../../images/video-camera.svg";

export default {
  name: "localVideo",
  components: {
    videoCameraImageIgnore2,
    castIcon,
    microphoneImage,
    microphoneImageIgnore,
    videoCameraImageIgnore,
    videoCameraImage
  },
  computed: {
    isVideoActive() {
      return this.$store.getters.getIsVideoActive;
    },
    getIsShareScreen() {
      return this.$store.getters.getIsShareScreen;
    },
    isAudioActive() {
      return this.$store.getters.getIsAudioActive;
    },
    localAudioTrack() {
      return false;
    },
    actvieRoom() {
      return false;
    },
    localVideoTrack() {
      return false;
    }
  },
  methods: {
    toggleAudio() {
      this.$ga.event("tutoringRoom", "toggleAudio");
      this.$store.dispatch("updateAudioToggle");
    },
    toggleVideo() {
      this.$ga.event("tutoringRoom", "toggleVideo");
      this.$store.dispatch("updateVideoToggle");
    },
    exitFull(ev) {
      if (ev.keyCode === 27){
        let video = document.querySelector(`#localTrack video`);
        video.classList.remove('fullscreenMode')
        this.$store.dispatch('updateToggleTutorFullScreen',false);
        document.querySelector('body').removeEventListener('keyup',this.exitFull)
      }
    },
    openFullScreen() {
      let video = document.querySelector(`#localTrack video`);
      if(video){
        video.classList.add('fullscreenMode');
        this.$store.dispatch('updateToggleTutorFullScreen',true);
        document.querySelector('body').addEventListener('keyup',this.exitFull)
      }
    }
  },
  beforeDestroy() {
    document.querySelector('body').removeEventListener('keyup',this.exitFull)
  },
};
</script>

<style lang="less">

.localVideoStream {
  position: relative;
  video {
    width: 100%;
    background-repeat: no-repeat;
    pointer-events: none;
  }
  video::-webkit-media-controls-enclosure {
    display: none !important;
  }

  .local-video-holder {
    .localTrack-placeholder {
      min-height: 229px;
      background: black;
      display: flex;
      justify-content: center;
      align-items: center;
      &.share-screen {
        background: #21cb4c;
        border-radius: 4px 0 00;
      }
      .placeholder-back {
        width: 100px;
        height: 100px;

        background-color: #353537;
        border-radius: 50%;
        display: flex;
        justify-content: center;
        align-items: center;
        &.share-screen {
          background-color: #21cb4c;
          svg {
            fill: #fff;
          }
        }
      }
    }
  }

  .control-panel {
    position: absolute;
    width: 100%;
    height: 30px;
    bottom: 8px;
    z-index: 11;
    .mic-image-btn {
      height: 30px;
      width: 30px;
      position: relative;
      outline: none;
      margin-left: 5px;
      transition: 0.3s;
      padding: 2px;
      border-radius: 50%;
      &.micIgnore {
        background-color: rgba(255, 24, 24, 0.63);
      }
    }
    .dynamicBackground-light {
      background-color: rgba(0, 0, 0, 0.15);
    }
    .dynamicBackground-dark {
      background-color: rgba(0, 0, 0, 0.3);
    }
    .mic-image-svg {
      position: absolute;
      height: 16px;
      width: 16px;
      top: 8px;
      left: 7px;
    }
    .mic-ignore {
      position: absolute;
      height: 16px;
      width: 16px;
      top: 8px;
      left: 7px;
    }
    .video-image-btn {
      height: 30px;
      width: 30px;
      position: relative;
      outline: none;
      margin-left: 4px;
      transition: 0.3s;
      padding: 2px;
      border-radius: 50%;
      &.camIgnore {
        background-color: rgba(255, 24, 24, 0.63);
      }
    }
    .video-image-svg {
      position: absolute;
      height: 20px;
      width: 20px;
      top: 6px;
      left: 6px;
      fill: white;
    }
    .cam-ignore {
      position: absolute;
      height: 16px;
      width: 16px;
      top: 8px;
      left: 7px;
    }
  }
  #localTrack {
          .fullscreenMode{
         position: fixed;
         top: 0;
         left: 0;
         right: 0;
         bottom: 0;
         width: 100vw;
         //object-fit: fill;
         height: 100vh;
         z-index: 20;
         background: #000;
      }
    video {
      width: 100%;
      background-repeat: no-repeat;
    }
    .fullscreen-btn {
      position: absolute;
      margin: 10px;
      right: 0;
      background: rgba(0, 0, 0, 0.7);
    }
  }

}
</style>