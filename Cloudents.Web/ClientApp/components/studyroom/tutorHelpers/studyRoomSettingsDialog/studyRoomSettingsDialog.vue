<template>
    <v-dialog :value="true" content-class="studyRoomSettingDialog" :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'" :fullscreen="true">
        <div class="settings">
            <div class="settingsHeader">
                <a @click="resetItems" class="logo-link">
                    <appLogo />
                </a>
            </div>

            <div class="settingsMain d-flex align-center justify-center">
                <div class="wrap d-md-flex d-block align-center justify-center mx-sm-5 mx-0">
                    <studyRoomVideo />
                    <studyRoomSettingDetails :isRoomActive="isRoomActive" @updateRoomIsActive="val => isRoomActive = val" />
                </div>
            </div>
        </div>
    </v-dialog>
</template>

<script>
import appLogo from '../../../app/logo/logo.vue'
import studyRoomSettingDetails from "./studyRoomSettingsDetails/studyRoomSettingsDetails.vue";
import studyRoomVideo from "./studyRoomVideo/studyRoomVideo.vue";

export default {
  components: {
    appLogo,
    studyRoomSettingDetails,
    studyRoomVideo,
  },
  data() {
    return {
      isRoomActive: false
    }
  },
  computed: {
    roomIsActive() {
      return this.$store.getters.getRoomIsActive
    },
  },
  methods:{
    closeDialog(){
      this.$store.commit('setComponent', '')
    },
    resetItems() {
      let isExit = confirm(this.$t("login_are_you_sure_you_want_to_exit"),)
      if(isExit){
        this.$ga.event("tutoringRoom", 'resetItems');
        this.closeDialog()
        this.$router.push('/');
      }
    }
  }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.studyRoomSettingDialog {
  .settings {
     height: 100%;
    .settingsHeader {
        position: absolute;
        width: 100%;
        padding: 10px 14px 0;
        display: flex;
        align-items: end;
        justify-content: space-between;
  
        button {
          outline: none;
        }
    }
  
    .settingsMain {
      height: inherit;
      padding-top: 70px;

      .wrap {
        height: 100%;
        @media (max-width: @screen-xs) {
          width: 100%;
        }
      }
    }

  }
}
</style>
