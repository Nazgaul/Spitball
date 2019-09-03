<template>
  <div class="study-room-settings-wrapper">
    <button @click="closeDialog" class="close-button"><v-icon>sbf-close</v-icon></button>
    <div class="study-room-settings-top">
      <v-navigation-drawer permanent class="study-room-settings-nav">
        <v-toolbar flat>
          <v-list>
            <v-list-tile>
              <v-list-tile-title class="study-room-settings-nav-title">
                <settingIcon></settingIcon>&nbsp;&nbsp;Study-Room Settings
              </v-list-tile-title>
            </v-list-tile>
          </v-list>
        </v-toolbar>

        <v-divider></v-divider>

        <v-list dense class="pt-0">
          <v-list-tile
            v-for="item in items"
            :key="item.title"
            @click="currenctComponent = item.componentName"
            :class="{'tileActive': currenctComponent === item.componentName}"
          >
            <v-list-tile-action>
              <component :is="item.icon"></component>
            </v-list-tile-action>

            <v-list-tile-content>
              <v-list-tile-title>{{ item.title }}</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
      </v-navigation-drawer>
      <v-container class="study-room-settings-body">
        <component :is="currenctComponent"></component>
      </v-container>
    </div>
  </div>
</template>

<script>
import settingIcon from "../../../../font-icon/settings.svg";
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";
import studyRoomAudioSetting from "./audio/studyRoomAudioSetting.vue";
import {mapActions} from "vuex";
import videoCameraImage from '../../images/video-camera.svg';
import microphoneImage from '../../images/microphone.svg';
export default {
  components: {
    settingIcon,
    studyRoomVideoSetting,
    studyRoomAudioSetting,
    videoCameraImage,
    microphoneImage
  },
  data() {
    return {
      items: [
        {
          title: "Video",
          icon: "videoCameraImage",
          componentName: "studyRoomVideoSetting"
        },
        {
          title: "Audio",
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
        }
        .v-list__tile__title {
          transition: none;
        }
        .tileActive {
          background-color: #3dc2ba;
          color: white;
          i {
            color: white;
          }
        }
      }
      .study-room-settings-body {
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
