<template>
  <div class="study-room-settings-wrapper">
    <button @click="closeDialog" class="close-button"><v-icon size="14">sbf-close</v-icon></button>
    <div class="study-room-settings-top">
      <!-- <v-navigation-drawer width="300" permanent class="study-room-settings-nav">
        <v-toolbar class="study_room_toolBar" flat>
          <v-list>
            <v-list-item class="pa-0">
              <v-list-item-title class="study-room-settings-nav-title">
                <span class="study-room-settings-nav-title_title" v-language:inner='"studyRoomSettings_title"'></span>
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
      </v-navigation-drawer> -->
      <!-- <v-container class="study-room-settings-body d-flex align-center">
        <component :is="currenctComponent"></component>
      </v-container> -->

        <v-container class="study-room-settings-body d-flex align-center justify-space-between">
                <studyRoomVideoSetting />
                <!-- <studyRoomAudioSetting /> -->
                <div class="settingDetailsWrap">
                    <div class="mb-12 settingDetails">
                        <div v-t="'studyRoomSettings_room_name'"></div>
                        <div v-t="'studyRoomSettings_tutor_name'">a</div>
                        <div v-t="'studyRoomSettings_price'">a</div>
                        <div v-t="'studyRoomSettings_schedule_date'">a</div>
                        <div v-t="'studyRoomSettings_room_link'">a</div>
                    </div>

                  <div class="text-center">
                      <div class="mb-8" v-t="'studyRoomSettings_clock_counter'"></div>

                      <v-btn class="joinNow white--text px-8" height="50" color="green" rounded depressed>{{$t('studyRoomSettings_join_now')}}</v-btn>
                  </div>
                </div>
        </v-container>
    </div>
  </div>
</template>

<script>
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";
import studyRoomAudioSetting from "./audio/studyRoomAudioSetting.vue";
import videoCameraImage from '../../images/video-camera.svg';
import microphoneImage from '../../images/microphone.svg';
// import { LanguageService } from '../../../../services/language/languageService';

export default {
  components: {
    studyRoomVideoSetting,
    studyRoomAudioSetting,
    videoCameraImage,
    microphoneImage
  },
  data() {
    return {
      // items: [
      //   {
      //     title: LanguageService.getValueByKey("studyRoomSettings_video_title"),
      //     icon: "videoCameraImage",
      //     componentName: "studyRoomVideoSetting"
      //   },
      //   {
      //     title: LanguageService.getValueByKey("studyRoomSettings_audio_title"),
      //     icon: "microphoneImage",
      //     componentName: "studyRoomAudioSetting"
      //   }
      // ],
      // currenctComponent: "studyRoomVideoSetting"
    };
  }, 
  methods:{
      closeDialog(){
        this.$store.dispatch('updateDialogRoomSettings',false)
      }
  }
};
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.tutor-settings-dialog {
    height: 100%;
    max-height: 100% !important;
    max-width: 100% !important;
    width: 100%;
    margin: 0 !important;
    background: #fff;
    border-radius: 0;
  .study-room-settings-wrapper {
    width: 100%;
    display: flex;
    flex-direction: column;
    position:relative;
    .close-button {
        position: absolute;
        top: 10px;
        right: 10px;
        outline:none;
    }
    .study-room-settings-top {
      display: flex;
      height: 100%;
      // .study-room-settings-nav {
      //   .study_room_toolBar{
      //     background: rgb(245, 245, 245)

      //   }
      //   .study-room-settings-nav-title{
      //       display: flex;
      //       align-items: center;
      //       font-weight: bold;
      //   }
      //   .study-room-settings-nav-title_title{
      //     font-size: 16px;
      //   }
      //   .v-list-item__title {
      //     transition: none;
      //     font-size: 13px;
      //   }
      //   .v-list-item__action{
      //     margin: 0 32px 0 0;
      //   }
      //   .tileActive {
      //     background-color: #5158af;
      //     color: white !important;
      //     i {
      //       color: white !important;
      //     }
      //     .v-list-item__action{
      //       svg{
      //         fill: #FFF !important;
      //       }
      //     }
      //   }
      // }
      .study-room-settings-body {
        padding: 30px;
        height: 100%;

        .settingDetailsWrap {

          font-weight: 600;
          color: @global-purple;
          .settingDetails {
            font-size: 16px;

          }
          .joinNow {
            font-weight: 600;
          }
        }
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
