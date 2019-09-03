<template>
    <v-layout
      v-if="isNotAllowedErrorType"
      column
      align-center
      justify-center
      class="error-permisson-wrap"
    >
      <v-flex class="mb-2" xs12>
        <p>
          <span v-language:inner>tutor_quality_allowed_title</span>
          <span class="font-weight-bold" v-language:inner>tutor_quality_allowed_press</span>
        </p>
      </v-flex>
      <v-flex xs6 sm6 class="mb-5">
        <img class="enable-explanation" src="../../../images/chrome-camera-reset.gif" alt />
      </v-flex>
    </v-layout>

    <v-layout v-else column align-center justify-center class="error-permisson-wrap">
      <v-flex class="mb-4">
        <p style="font-size: 16px;" v-language:inner>tutor_quality_allowed_looks_like</p>
        <div v-language:inner>tutor_quality_allowed_tip_one</div>
        <p
          v-language:inner
          style="font-size: 12px;margin-top:10px;max-width: 80%;"
        >tutor_quality_allowed_tip_two</p>
      </v-flex>
    </v-layout>
</template>

<script>

export default {
  props: {
    deviceValidationObj: {
      type: Object,
      required: true
    }
  },
  computed: {
    isNotAllowedErrorType() {
      let videoNotAllowed = false;
      let audioNotAllowed = false;
      if (!this.deviceValidationObj.hasVideo) {
        videoNotAllowed =
          this.deviceValidationObj.errors.video.indexOf("NotAllowedError") > -1;
      }
      if (!this.deviceValidationObj.hasAudio) {
        audioNotAllowed =
          this.deviceValidationObj.errors.audio.indexOf("NotAllowedError") > -1;
      }

      return videoNotAllowed || audioNotAllowed;
    }
  }
};
</script>

<style  lang="less">
.error-permisson-wrap {
  .enable-explanation {
    width: 100%;
    max-width: 368px;
    border-radius: 4px;
    box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
    border: solid 6px #ffffff;
  }
  .input-icon {
    height: 40px;
    width: 40px;
    fill: #a8a8a8;
  }
  img {
    width: 100%;
    height: auto;
  }
}
</style>