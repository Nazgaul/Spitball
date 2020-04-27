<template>
    <v-row class="uploadContent mb-2 mb-sm-4">
        <v-col class="text-center uploadCol pa-0">
            <img src="./images/upload.png" class="mb-3 uploadIcon" alt="">
            <div class="text1">{{$t('dashboardTeacher_upload_content')}}</div>
            <div class="text2 mb-3">{{$t('dashboardTeacher_earn_money')}}</div>
            <v-btn sel="upload_btn" class="uploadBtn" color="#4c59ff" v-openDialog="uploadDialog" outlined rounded>{{$t('dashboardTeacher_upload')}}</v-btn>
        </v-col>
    </v-row>
</template>
<script>

import * as dialogNames from '../../global/dialogInjection/dialogNames.js'
export default {
  name: "uploadContent",
  data() {
    return {
      uploadDialog: dialogNames.Upload
    }
  },
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.uploadContent {
  padding: 16px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  background: white;
  width: 100%;
  margin: 0 auto;
  @media (max-width: @screen-xs) {
    box-shadow: none;
  }

  .uploadCol {
    color: @global-purple;
    .text1 {
      font-size: 16px;
      font-weight: 600;
    }
    .text2 {
      font-size: 12px;
    }
    .uploadBtn {
      text-transform: initial;
      font-weight: 600;
      min-width: 120px;
      letter-spacing: normal;
    }
  }
}
</style>
