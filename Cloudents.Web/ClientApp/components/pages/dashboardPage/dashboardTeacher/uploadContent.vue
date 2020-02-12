<template>
    <v-row class="uploadContent mb-2 mb-sm-4" dense>
        <v-col class="text-center uploadCol pa-0">
            <uploadIcon class="mb-3 uploadIcon" />
            <div class="text1">{{$t('dashboardTeacher_upload_content')}}</div>
            <div class="text2 mb-3">{{$t('dashboardTeacher_earn_money')}}</div>
            <v-btn class="uploadBtn" color="#4c59ff" @click="openUpload()" outlined rounded>{{$t('dashboardTeacher_upload')}}</v-btn>
        </v-col>
    </v-row>
</template>
<script>

import uploadIcon from './images/uploadIcon.svg'
export default {
  name: "uploadContent",
  components: {
    uploadIcon
  },
  methods: {
    openUpload() {
      if (this.$store.getters.accountUser == null) {
        this.$store.dispatch('updateLoginDialogState', true);
      } else {
        this.$store.dispatch('updateDialogState', true);
        this.$store.dispatch('setReturnToUpload', false);
      }
    },
  }
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

  @media (max-width: @screen-xs) {
    box-shadow: none;
  }

  .uploadCol {
    color: @global-purple;
    .uploadIcon {
      width: 240px;
    }
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
