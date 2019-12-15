<template>
  <div class="settingsUnable">
      <h1 class="unable-title" v-language:inner="'studyroomSets_unable_title'"/>
      <div class="unable-middle">
      <img src="../image/device.png" alt="">
      <p class="unable-middle-txt" v-language:inner="'studyroomSets_unable_middle'"/>
      </div>
      <div class="unable-how">
      <img class="how-img" src="../image/defined.png" alt="">
      <div class="how-cont">
          <div class="how-title" v-language:inner="'studyroomSets_unable_how_title'"/>
          <div class="how-txt" v-language:inner="'studyroomSets_unable_how_txt'"/>
      </div>
      
      </div>
      <div class="unable-actions">
        <v-btn class="actions-try" depressed rounded color="#4452fc" @click="reloadPage"><span v-language:inner="'studyroomSets_unable_action_try'"/></v-btn>
        <v-btn class="actions-continue" depressed rounded @click="nextPage"><span v-language:inner="'studyroomSets_unable_action_continue'"/></v-btn>
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
methods:{
  reloadPage(){
    global.location.reload();
  },
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
.settingsUnable{
  max-width: 564px;
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
    .unable-middle-txt{
      margin: 0;
      padding: 0;
      padding-left: 30px;
      padding-bottom: 18px;
      font-size: 16px;
      font-weight: 600;
      font-stretch: normal;
      font-style: normal;
      line-height: 1.63;
      letter-spacing: normal;
      color: #595475;
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