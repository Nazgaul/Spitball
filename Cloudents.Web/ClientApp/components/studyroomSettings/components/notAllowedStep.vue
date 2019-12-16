<template>
  <div class="settingsNotAllowed">
      <h1 class="unable-title" v-language:inner="'studyroomSets_error_with_video_and_audio'"/>
      <div class="unable-middle">
        <img :src="gifImg" alt="">
        <div>
            <p class="unable-middle-txt" v-language:inner="'studyroomSets_to_communicate'"/>
            <p class="unable-middle-txt settingsUnable_blue-color" v-language:inner="'studyroomSets_refresh'"/>
        </div>
        
      </div>
      <div class="unable-actions">
        <v-btn class="actions-try" depressed round color="#4452fc" @click="nextPage"><span v-language:inner="'studyroomSets_continue-anyway'"/></v-btn>
      </div>
  </div>
</template>
<script>
import tutorService from '../../studyroom/tutorService';
export default {
  props:{
    nextStep:{
      type:Function,
      required: true
    }
  },
computed:{
 gifImg(){
   let isRtl = global.isRtl;
   if(isRtl){
    return require('../image/unblock-permissions-rtl.gif');
   }else{
    return require('../image/unblock-permissions.gif');
   }
 }
},
methods:{
  nextPage(){
  if(tutorService.isRecordingSupported()){
    this.nextStep('watchRecordedStep', true);
  }else{
    this.nextStep('studyRoom', true);
  }
    
  }
}
}
</script>

<style lang="less">
.settingsNotAllowed{
  max-width: 675px;
  .unable-title{
    font-size: 38px;
    font-weight: 600;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    text-align: center;
    color: #43425d;
  }
  .unable-middle{
    margin-top: 40px;
    display: flex;
    align-items: center;
    img{
      width:368px;
    }
    .unable-middle-txt{
      margin: 0;
      padding: 0;
      padding-left: 30px;
      padding-bottom: 18px;
      font-size: 16px;
      font-weight: 400;
      font-stretch: normal;
      font-style: normal;
      line-height: 1.63;
      letter-spacing: normal;
      color: #595475;
    }
    .settingsUnable_blue-color{
        // color: #4c59ff;
        font-weight: bold;
    }
  }
  .unable-how{
    margin-top: 42px;
    width: 100%;
    background: white;
    padding: 12px;
    display: flex;
    align-items: start;
    .how-img{
      object-fit: contain;
    }
    .how-cont{
      padding-left: 14px;
      margin-top: -2px;
      .how-title{
        font-size: 16px;
        font-weight: 600;
        line-height: 1.63;
        color: #4c59ff;
      }
      .how-txt{
        font-size: 14px;
        line-height: 1.64;
        color: #43425d;
      }
    }
  }
  .unable-actions{
    text-align: center;
    margin-top: 60px;
    .v-btn{
      min-width: 230px;
      height: 40px !important;
      text-transform: capitalize  !important;
      font-size: 14px;
      font-weight: 600;
    }
    .actions-try{
      color: white !important;
    }
    .actions-continue{
      color: #4452fc;
      border: 1px solid #4452fc !important;
    }
  }
}
</style>