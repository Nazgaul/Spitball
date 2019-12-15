<template>
    <div class="studySettingPopUp">
        <h1 class="settingPop-title" v-language:inner="'studyroomSets_settingPop_title'"/>
        <h2 class="settingPop-txt" v-language:inner="'studyroomSets_settingPop_txt'"/>
        <div class="settingPop-actions">
            <v-btn class="settingPop-actions-yes" depressed round color="#4452fc" @click="nextPage"><span v-language:inner="'studyroomSets_settingPop_actions_yes'"/></v-btn>
            <v-btn class="settingPop-actions-no" depressed round color="white" @click="closeDialog"><span v-language:inner="'studyroomSets_settingPop_actions_no'"/></v-btn>
        </div>
    </div>
</template>

<script>
export default {
  props:{
    nextStep:{
      type:Function,
      required: true
    },
    nextStepName:{
      type:String,
      required: true
    },
    closeDialog:{
        type:Function,
        required:true
    }
  },
methods:{
nextPage(){
    this.nextStep(this.nextStepName);
    this.closeDialog();
  }
}
}
</script>

<style lang="less">
.studySettingPopUp{
    width: 502px;
    height: 226px;
    border-radius: 6px;
    background-color: #ffffff;
    color: #43425d;
    text-align: center;
    line-height: 1.5;
    letter-spacing: normal;
    font-stretch: normal;
    font-style: normal;
    padding: 16px 0;
    
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    .settingPop-title{
        padding: 0 16px;
        font-size: 20px;
        font-weight: 600;
    }
    .settingPop-txt{
        font-size: 16px;
        font-weight: normal;
    }
    .settingPop-actions{
        text-align: center;
        .v-btn{
        min-width: 230px;
        height: 40px !important;
        text-transform: capitalize  !important;
        font-size: 14px;
        font-weight: 600;
        margin-bottom: 0;
        }
        .settingPop-actions-yes{
            color: white !important;
        }
        .settingPop-actions-no{
        color: #4452fc;
        border: 1px solid #4452fc !important;
        }
    }
}
</style>