<template>
  <div class="error-with-audio-recording-container">
      <v-container class="ewar-wrapper" justify-center align-center>
          <h1 v-language:inner="'tutor_ewar-continue-without-recording'"></h1>
          <v-layout>
              <img :src="voiceErrorImg" alt="">
          </v-layout>
          <v-layout class="ewar-btns-container">
              <v-btn @click="tryAgain" depressed rounded class="actions-try"><span v-language:inner="'tutor_ewar-try-again'"></span></v-btn>
              <v-btn @click="ignoreError" depressed rounded class="actions-continue"><span v-language:inner="'tutor_ewar-continue-without-audio'"></span></v-btn>
          </v-layout>
      </v-container>
  </div>
</template>

<script>
import studyRoomRecordingService from '../../studyRoomRecordingService';
import {mapActions} from 'vuex';
export default {
computed:{
    voiceErrorImg(){
        let isRtl = global.isRtl;
        if(isRtl){
            return require('./img/voice-recording-rtl.png')
        }else{
            return require('./img/voice-recording.png')
        }
    }
},
methods:{
    ...mapActions(['setShowAudioRecordingError']),
    tryAgain(){
        let cancelled = true;
        studyRoomRecordingService.stopRecord(cancelled);
        setTimeout(()=>{
            studyRoomRecordingService.toggleRecord();
        })
        this.setShowAudioRecordingError(false);
    },
    ignoreError(){
        this.setShowAudioRecordingError(false);
    }
}
}
</script>

<style lang="less">
.error-with-audio-recording-container{
    width:100%;
    background:white;
    .ewar-wrapper{
        display: flex;
        flex-direction: column;
        h1{
            font-size: 20px;
            font-weight: 600;
            line-height: 1.5;
            text-align: center;
            color: #4d4b69;
            margin-bottom: 40px;
            max-width: 415px;
        }
    }
    .ewar-btns-container{
    text-align: center;
    margin-top: 60px;
      height: 40px !important;
      font-size: 14px;
      font-weight: 600;
      
    .actions-try{
        text-transform: none;
    min-width: 230px;
      color: white;
      background-color: #4452fc !important;
    }
    .actions-continue{
        text-transform: none;
        min-width: 230px;
        color: #4452fc;
        background-color: white !important;
        border: 1px solid #4452fc !important;
      
    }
  }
}
</style>