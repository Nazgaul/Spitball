<template>
  <div class="settingsEnable">
      <h1 class="enable-title" v-language:inner="'studyroomSets_enable_title'"/>
      <div class="settings-top-wrap">
        <div>
          <img src="../image/record-icon.png" alt="" srcset="">
        </div>
        <div>
          <div class="enable-top">
            <span v-language:inner="'studyroomSets_start_recording_instruction'"/>
          </div>
          <div class="enable-top">
            <span class="enable-note" v-language:inner="'studyroomSets_enable_note'"/>
            <span v-language:inner="'studyroomSets_enable_note_txt'"/>
          </div>
        </div>
      </div>
      <h3 class="enable-middle-title" v-language:inner="'studyroomSets_enable_middle_title'"/>
      <img :src="shareScreenImg" alt="">
      <div class="enable-actions">
        <v-btn class="actions-record" depressed round color="#4452fc" @click="skipToStudyRoom"><span v-language:inner="'studyroomSets_enable_actions_yes'"/></v-btn>
      </div>
  </div>
</template>

<script>
import studyRoomRecordingService from '../../studyRoom/studyRoomRecordingService';
export default {
  props:{
    nextStep:{
      type:Function,
      required: true
    }
  },
  methods:{
    skipToStudyRoom(){
      studyRoomRecordingService.toggleRecord();
      this.nextStep('studyRoom')
    }
  },
  computed:{
    shareScreenImg(){
      let isRtl = global.isRtl;
      if(isRtl){
        return require('../image/group-copy-rtl.png');
      }else{
        return require('../image/group-copy.png');
      }
    }
  }
}
</script>

<style lang="less">
.settingsEnable{
  max-width: 610px;
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #43425d;
  .enable-title{
    font-size: 32px;
    font-weight: 600;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    text-align: center;
  }
  .settings-top-wrap{
    display: flex;
    align-items: center;
    .enable-top{
      margin-left: 14px;
        font-size: 14px;
        font-weight: 600;
        font-stretch: normal;
        font-style: normal;
        letter-spacing: normal;
        .enable-note{
          color: #5560ff
        }
    }
  }
  
  .enable-middle-title{
    margin-top: 26px;
    margin-bottom: 12px;
    font-size: 18px;
    font-weight: 600;
    font-stretch: normal;
    font-style: normal;
    line-height: 1.44;
    letter-spacing: normal;
    color: #4c59ff;
  }
  .enable-actions{
    text-align: center;
    margin-top: 10px;
    .v-btn{
      min-width: 178px;
      height: 40px !important;
      text-transform: capitalize  !important;
      font-size: 14px;
      font-weight: 600;
    }
    .actions-record{
      color: white !important;
    }
    .actions-no-e{
      color: #4452fc;
      border: 1px solid #4452fc !important;
    }
  }
}
</style>