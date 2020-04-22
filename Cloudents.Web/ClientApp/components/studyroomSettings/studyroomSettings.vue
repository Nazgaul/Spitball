
<template>
  <div>
      <div class="settingsTop">
        <div class="settingsTopCont">
          <router-link @click.prevent="resetItems()" to="/" class="settingsTopLogoWrap">
            <logo class="settingsTopLogo"></logo>
          </router-link>
          
          <intercomSVG @click="showIntercom" class="settingsTopIntercom"></intercomSVG>
        </div>
      </div>

      <div class="settingsContent">
        <component :nextStep="nextStep" :is="currentStep" />
      </div>
      <sbDialog
          :activateOverlay="true"
          :isPersistent="true"
          :showDialog="showErrorDialog"
          :popUpType="'studySetting'"
          maxWidth='502'
          :maxHeight="'226'">
          <studySettingPopUp :nextStepName="nextStepName" :nextStep="nextStep" :closeDialog="closeDialog"/>
      </sbDialog>
  </div>
</template>

<script>
// TODO: clean this file @idan to @maor
import {mapGetters, mapActions} from 'vuex';

import studyroomSettingsUtils from './studyroomSettingsUtils';
// import logo from '../../components/app/logo/logo-spitball.svg';
import logo from '../../components/app/logo/logo.vue'
import intercomSVG from './image/icon-1-2.svg'

import unableToConnetStep from './components/unableToConnetStep.vue';
import watchRecordedStep from './components/watchRecordedStep.vue';
//import enableScreenStep from './components/enableScreenStep.vue';
import notAllowedStep from './components/notAllowedStep.vue';

import studySettingPopUp from './components/studySettingPopUp.vue';
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';

import intercomSettings from '../../services/intercomService';


export default {
  components:{
    logo,
    intercomSVG,
    unableToConnetStep,
    watchRecordedStep,
    notAllowedStep,
    studySettingPopUp,
    sbDialog
  },
  data(){
    return{
      currentStep: null,
      stepHistory: [],
      currentPageIndex: 0,
      showErrorDialog: false,
      nextStepName: '',
    }
  },
  props: {
    id: {
      type: String,
      default: ''
    }
  },
  computed:{
    ...mapGetters(['getStepHistory']),
  },
  methods:{
    ...mapActions(['setStepHistory', 'reOrderStepHistory', 'pushHistoryState', 'replaceHistoryState', 'setVisitedSettingPage']),
    resetItems(){
      this.$router.push('/');
    },
    orderStepHistory(){
      let newStepHistory = this.stepHistory.slice(0, this.currentPageIndex+1);
      return newStepHistory;
    },
    openDialog(stepName){
      this.nextStepName = stepName;
      this.showErrorDialog = true;
    },
    closeDialog(){
      this.showErrorDialog = false;
    },
    nextStep(stepName, openDialog){
      if(openDialog){
          this.openDialog(stepName);
      }else{
        if(stepName === "studyRoom"){
          this.$router.push({name:'tutoring', params:{id:this.id}})
        }else{
          this.reOrderStepHistory(this.currentPageIndex+1);
          this.setStepHistory(stepName);
          this.pushHistoryState();
          this.currentStep = stepName;
          this.currentPageIndex = this.getStepHistory.length -1;
        }
      }
    },
    goStep({page}){
        this.currentStep = page;
    },
    showIntercom(){
      intercomSettings.showDialog();
    },
  },
  async created(){
    this.$store.dispatch('updateStudyRoomInformationForSettings',this.id)
    if(this.$vuetify.breakpoint.xsOnly){
      this.$router.push({name:'tutoring', params:{id:this.id}})
    }
    global.onpopstate = (event)=>{
      this.goStep(event.state)
    };

    this.setVisitedSettingPage(true);
    await studyroomSettingsUtils.validateUserMedia(true, true);
    let firstPage = studyroomSettingsUtils.determinFirstPage();
    if(firstPage.type === "studyRoom"){
      this.$router.push({name:'tutoring', params:{id:this.id}})
    }else{
      this.currentStep = firstPage.type;
    }
    this.setStepHistory(this.currentStep);
    this.replaceHistoryState();
  }   
}

</script>

<style lang="less">
.settingsTop{
  height: 58px;
  width: 100%;
  background-color: #4c59ff;
  .settingsTopCont{
    max-width: calc(~"100% - 210px");
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 100%;
    .settingsTopLogoWrap{
      display: flex;
      .settingsTopLogo{
        svg {
          fill: white;
        }
      }
    }
    .settingsTopIntercom{
      cursor: pointer;
    }
  }
}
.settingsContent{
  margin: 0 auto;
  margin-top: 30px;
  display: flex;
  justify-content: center;
}
</style>