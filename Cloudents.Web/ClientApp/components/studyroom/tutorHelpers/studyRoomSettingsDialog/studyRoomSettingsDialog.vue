<template>
  <div class="study-room-settings-wrapper">
    <button @click="closeDialog" class="close-button"><v-icon>sbf-close</v-icon></button>
    <div class="study-room-settings-top">
      <v-navigation-drawer permanent class="study-room-settings-nav">
        <v-toolbar flat>
          <v-list>
            <v-list-item>
              <v-list-item-title class="study-room-settings-nav-title">
                <span v-language:inner='"studyRoomSettings_title"'></span>
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </v-toolbar>

        <v-divider></v-divider>

        <v-list dense class="study-room-settings-nav-sideMenu pt-0">
          <v-list-item
            v-for="item in items"
            :key="item.title"
            @click="currenctComponent = item.componentName"
            :class="{'tileActive': currenctComponent === item.componentName}"
          >
            <v-list-item-action>
              <component :is="item.icon"></component>
            </v-list-item-action>

            <v-list-item-content>
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-navigation-drawer>
      <v-container class="study-room-settings-body">
        <component :is="currenctComponent"></component>
      </v-container>
    </div>
  </div>
</template>

<script>
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";
import studyRoomAudioSetting from "./audio/studyRoomAudioSetting.vue";
import {mapActions} from "vuex";
import videoCameraImage from '../../images/video-camera.svg';
import microphoneImage from '../../images/microphone.svg';
import { LanguageService } from '../../../../services/language/languageService';

export default {
  components: {
    studyRoomVideoSetting,
    studyRoomAudioSetting,
    videoCameraImage,
    microphoneImage
  },
  data() {
    return {
      items: [
        {
          title: LanguageService.getValueByKey("studyRoomSettings_video_title"),
          icon: "videoCameraImage",
          componentName: "studyRoomVideoSetting"
        },
        {
          title: LanguageService.getValueByKey("studyRoomSettings_audio_title"),
          icon: "microphoneImage",
          componentName: "studyRoomAudioSetting"
        }
      ],
      currenctComponent: "studyRoomVideoSetting"
    };
  }, 
  methods:{
      ...mapActions(['setStudyRoomSettingsDialog']),
      closeDialog(){
          this.setStudyRoomSettingsDialog(false);
      }
  }
};
</script>

<style lang="less">
.tutor-settings-dialog {
  min-height: 560px;
  background-color: #fff;
  .study-room-settings-wrapper {
    width: 100%;
    display: flex;
    flex-direction: column;
    position:relative;
    .close-button{
        position: absolute;
        top: 10px;
        right: 10px;
        outline:none;
        i{
            font-size: 14px;
        }
    }
    .study-room-settings-top {
      display: flex;
      height: 100%;
      .study-room-settings-nav {
        .study-room-settings-nav-title{
            display: flex;
            align-items: center;
            font-weight: bold;
        }
        .v-list-item__title {
          transition: none;
        }
        .v-list-item__action{
          margin: 0 32px 0 0;
        }
        .tileActive {
          background-color: #5158af;
          color: white !important;
          i {
            color: white !important;
          }
          .v-list-item__action{
            svg{
              fill: #FFF !important;
            }
          }
        }
      }
      .study-room-settings-body {
        padding: 30px;
        height: 100%;
      }
    }
    .study-room-settings-bottom {
      border-top: 1px solid #e0e0e0;
      .study-room-settings-footer {
          button{
            margin: 0 10px;
            background: #c2443d;
            padding: 5px 15px;
            border-radius: 4px;
            color: #FFF;
            font-weight: 400;
            font-size: 14px;
            outline: none;
          }
      }
    }
  }
}
</style>
