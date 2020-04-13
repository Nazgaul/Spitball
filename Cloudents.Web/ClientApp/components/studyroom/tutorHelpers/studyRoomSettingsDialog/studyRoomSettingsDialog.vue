<template>
    <v-dialog :value="true" content-class="studyRoomSettingDialog" :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'" :fullscreen="true">
        <div class="settings">
            <div class="settingsHeader">
                <a @click="resetItems" class="logo-link">
                    <appLogo />
                </a>
                <!-- NOT SURE IF WE NEED EXIT BUTTON -->
                <v-icon @click="closeDialog" size="14">sbf-close</v-icon>
            </div>

            <div class="settingsMain d-flex align-center justify-center">
                <studyRoomVideoSetting />

                <studyRoomSettingDetails :isRoomActive="isRoomActive" @updateRoomIsActive="val => isRoomActive = val" />
            </div>
        </div>
    </v-dialog>
</template>

<script>
import appLogo from '../../../app/logo/logo.vue'
import studyRoomSettingDetails from "./studyRoomSettingsDetails.vue";
import studyRoomVideoSetting from "./video/studyRoomVideoSetting.vue";

export default {
  components: {
    appLogo,
    studyRoomSettingDetails,
    studyRoomVideoSetting,
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
    }

  }
}
</style>
