<template>
    <v-dialog :value="true" max-width="570" content-class="studyRoomAudioVideoDialog" :fullscreen="$vuetify.breakpoint.xsOnly" persistent>
        <div class="studyRoomSettingsWrapper">
            <div class="audioVideoTabs d-flex align-center text-center">
                <div class="tabBtn video" :class="{'active': toggleVideoActiveClass}" @click="switchToVideo">
                    <videoCameraImage class="image vidSvg" width="22" height="40" />
                    <div class="tabText ml-1" v-t="'studyRoomSettings_video_title'"></div>
                </div>

                <div class="tabBtn audio" :class="{'active': toggleAudioActiveClass}" @click="switchToAudio">
                    <microphoneImage class="image" width="24" />
                    <div class="tabText ml-1" v-t="'studyRoomSettings_audio_title'"></div>
                </div>
            </div>

            <div class="mainWrap pa-5 pt-0 ">
                <component :is="currenctComponent"></component>

                <div class="text-sm-right text-center btnWrap">
                    <v-btn
                      class="white--text text-truncate"
                      @click="$emit('closeAudioVideoSettingDialog', false)"
                      width="120"
                      color="#4c59ff"
                      rounded
                      depressed
                    >
                      {{$t('studyRoomSettings_done_btn')}}
                    </v-btn>
                </div>
            </div>
        </div>
    </v-dialog>
</template>

<script>
import studyRoomVideoSetting from "./studyRoomVideoSetting/studyRoomVideoSetting.vue";
import studyRoomAudioSetting from "./studyRoomAudioSetting/studyRoomAudioSetting.vue";

import videoCameraImage from '../images/videocam.svg';
import microphoneImage from '../../../images/microphone.svg';

export default {
  components: {
    studyRoomVideoSetting,
    studyRoomAudioSetting,
    videoCameraImage,
    microphoneImage
  },
  data() {
    return {
      toggleVideoActiveClass: true,
      toggleAudioActiveClass: false,
      currenctComponent: "studyRoomVideoSetting"
    };
  },
  methods: {
    switchToAudio() {
      this.currenctComponent = 'studyRoomAudioSetting'
      this.toggleAudioActiveClass = true
      this.toggleVideoActiveClass = false
    },
    switchToVideo() {
      this.currenctComponent = 'studyRoomVideoSetting'
      this.toggleVideoActiveClass = true
      this.toggleAudioActiveClass = false
    }
  }
};
</script>

<style lang="less">
@import '../../../../../styles/colors.less';
@import '../../../../../styles/mixin.less';

@tabsHeight: 46px;

.studyRoomAudioVideoDialog {
  height: 544px;
  background-color: #fff;
  border-radius: 6px;

  @media (max-width: @screen-xs) {
    height: 100%;
  }
  .studyRoomSettingsWrapper {
    height: 100%;

    .audioVideoTabs {
      height: @tabsHeight;

      .tabBtn {
        cursor: pointer;
        flex: 1;
        border-bottom: 1px solid #ddd;
        color: #a5a8ba;
        font-weight: 600;

        &:first-child {
          border-right: 1px solid #ddd;
        }
        .image {
          fill: #a5a8ba;
        }
        * {
          display: inline-block;
          vertical-align: middle;
        }
        &.active {
          border-bottom: 2px solid @global-auth-text;

          .image {
            fill: @global-auth-text;
          }
          .tabText {
            color: @global-auth-text;
          }
        }
      }
    }
    .mainWrap {
      height: calc(100% - @tabsHeight); // calculating the tabs height for justify-space-between wont make scroll
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      align-items: center;

      .btnWrap {
        width: 100%;
      }
    }
  }
}
</style>