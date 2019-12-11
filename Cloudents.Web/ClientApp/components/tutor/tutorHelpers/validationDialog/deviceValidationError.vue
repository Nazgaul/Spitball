<template>
  <div class="device-validation-container">
    <div class="header-text-wrap px-6 py-2">
      <div class="header-text 1">
        <span v-html="$Ph('studyRoom_error_with',[devicesTypeWithError])"/>
      </div>
    </div>
    <v-card class="elevation-0 pa-6" color="white" tile>
      <v-container class="pa-0">
        <not-allowed :deviceValidationObj="deviceValidationObj"></not-allowed>
        <v-layout align-center justify-end>
          <v-flex class="d-flex align-center justify-center" style="max-width: 250px;">
            <button class="blue-btn" @click="closeDialog()">
              <span v-language:inner>tutor_quality_btn_done</span>
            </button>
          </v-flex>
        </v-layout>
      </v-container>
    </v-card>
  </div>
</template>

<script>
import { mapActions } from "vuex";
import notAllowed from "./errorTypes/notAllowed.vue";
import microphoneImage from "../../images/microphone.svg";
import videoImage from "../../images/video-camera.svg";
import insightService from "../../../../services/insightService";
import { LanguageService } from "../../../../services/language/languageService";

export default {
  components: {
    notAllowed,
    microphoneImage,
    videoImage
  },
  props: {
    deviceValidationObj: {
      required: true,
      type: Object
    }
  },
  computed: {
    devicesTypeWithError() {
      let result = "";
      let videoString = LanguageService.getValueByKey("studyRoom_video");
      let audioString = LanguageService.getValueByKey("studyRoom_audio");
      let videoAndAudio = LanguageService.getValueByKey("studyRoom_video_&_audio");
      if (
        !this.deviceValidationObj.hasVideo &&
        !this.deviceValidationObj.hasAudio
      ) {
        result = videoAndAudio;
      } else if (!this.deviceValidationObj.hasVideo) {
        result = videoString;
      } else if (!this.deviceValidationObj.hasAudio) {
        result = audioString;
      }
      return result;
    }
  },
  methods: {
    ...mapActions(["setDeviceValidationError"]),
    closeDialog() {
      this.setDeviceValidationError(false);
    }
  }
};
</script>

<style lang="less">
.device-validation-container {
  border-radius: 4px;
  .header-text-wrap {
    display: flex;
    background-color: #f2f2f2;
    width: 100%;
    border-radius: 4px 4px 0 0;
    .header-text {
      font-size: 12px;
      font-weight: 600;
      color: #000000;
    }
  }
  .blue-btn {
    width: 100%;
    padding: 5px 10px;
    border-radius: 4px;
    box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
    border: solid 1px #43425d;
    color: rgba(67, 66, 93, 0.87);
    font-size: 14px;
    font-weight: 600;
  }
}
</style>