<template>
    <v-dialog :value="true" class="study-room-settings-wrapper" :fullscreen="true">
        <button @click="closeDialog" class="close-button">
            <v-icon size="14">sbf-close</v-icon>
        </button>
        <appLogo class="pa-4" />
        <!-- <div class="study-room-settings-top"> -->

            <v-container class="study-room-settings-body d-flex align-center justify-center">
                <studyRoomVideoSetting />
                
                <studyRoomSettingDetails :isRoomActive="isRoomActive" :id="id" />
            </v-container>

        <!-- </div> -->
    </v-dialog>
</template>

<script>
import appLogo from '../../../app/logo/logo.vue'
import studyRoomSettingDetails from "./studyRoomSettingsDetails.vue";
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";
// import studyRoomAudioSetting from "./audio/studyRoomAudioSetting.vue";

export default {
  components: {
    appLogo,
    studyRoomSettingDetails,
    studyRoomVideoSetting,
    // studyRoomAudioSetting,
  },
  props: {
    id: {
      type: String,
    }
  },
  data() {
    return {
      isRoomActive: false
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
  computed: {
    roomName() {
      return this.$store.getters?.getRoomName
    },
    roomTutor() {
      return this.$store.getters?.getRoomTutor
    },
    roomLink() {
      return `${window.origin}/studyroom/${this.id}`
    }
  },
  methods:{
    startSession(){
      this.isLoading = true;
      this.$store.dispatch('updateEnterRoom',this.id)
    },
    closeDialog(){
      this.$store.dispatch('updateDialogRoomSettings',false)
    }
  }
};
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.tutor-settings-dialog {
    background: #fff;
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
      .study-room-settings-body {
        height: 100%;

        .settingDetailsWrap {
          max-width: 400px;
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
