<template>
    <div class="study-room-settings-wrapper">
        <button @click="closeDialog" class="close-button">
            <v-icon size="14">sbf-close</v-icon>
        </button>
        <appLogo class="pa-4" />
        <div class="study-room-settings-top">

            <v-container class="study-room-settings-body d-flex align-center justify-center">

                <div class="mr-12">
                    <studyRoomVideoSetting />
                </div>
                
                <div class="settingDetailsWrap ml-12">
                    <div class="mb-12 settingDetails">
                        <div>
                          <span class="mr-2" v-t="'studyRoomSettings_room_name'"></span>
                          <span>{{roomName}}</span>
                        </div>
                        <div>
                          <span class="mr-2" v-t="'studyRoomSettings_tutor_name'"></span>
                          <span>{{roomTutor.tutorName}}</span>
                        </div>
                        <div>
                          <span class="mr-2" v-t="'studyRoomSettings_price'"></span>
                          <span>{{roomTutor.tutorPrice}}</span>
                        </div>
                        <div>
                          <span class="mr-2" v-t="'studyRoomSettings_schedule_date'"></span>
                          <span></span>
                        </div>
                        <div>
                          <span class="mr-2" v-t="'studyRoomSettings_room_link'"></span>
                          <span>{{roomLink}}</span>
                        </div>
                    </div>

                  <div class="text-center">
                      <div class="mb-8" v-show="!isRoomActive">
                        <div v-t="'studyRoomSettings_clock_counter'"></div>
                        <sessionStartCounter @updateRoomisActive="val => isRoomActive = val" />
                      </div>
                      <v-btn class="joinNow white--text px-8" @click="startSession" :disabled="!isRoomActive" height="50" color="#5360FC" rounded depressed>{{$t('studyRoomSettings_join_now')}}</v-btn>
                  </div>
                </div>
            </v-container>

        </div>
    </div>
</template>

<script>
import appLogo from '../../../app/logo/logo.vue'
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";
import studyRoomAudioSetting from "./audio/studyRoomAudioSetting.vue";
import sessionStartCounter from '../sessionStartCounter/sessionStartCounter.vue'

export default {
  components: {
    appLogo,
    studyRoomVideoSetting,
    studyRoomAudioSetting,
    sessionStartCounter,
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
      // display: flex;
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
