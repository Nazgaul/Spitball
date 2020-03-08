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
        <!-- <span style="position:absolute; left:25px;font-weight:bold;">{{currentStep}}</span> -->
        
        <!-- <unableToConnetStep :nextStep="nextStep"/> -->
        <!-- <enableScreenStep/> -->
        <!-- <watchRecordedStep/> -->
        <!-- <notAllowedStep/> -->
        <component :nextStep="nextStep" :is="currentStep" />
        <!-- {{currentStep}} -->
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
import {mapGetters, mapActions} from 'vuex';

import studyroomSettingsUtils from './studyroomSettingsUtils';
import tutorService from '../studyroom/tutorService';
// import logo from '../../components/app/logo/logo-spitball.svg';
import logo from '../../components/app/logo/logo.vue'
import intercomSVG from './image/icon-1-2.svg'

import unableToConnetStep from './components/unableToConnetStep.vue';
import watchRecordedStep from './components/watchRecordedStep.vue';
//import enableScreenStep from './components/enableScreenStep.vue';
import notAllowedStep from './components/notAllowedStep.vue';

import studySettingPopUp from './components/studySettingPopUp.vue';
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import storeService from '../../services/store/storeService';

import tutoringMain from '../../store/studyRoomStore/tutoringMain.js';
import studyroomSettings_store from '../../store/studyRoomStore/studyroomSettings_store.js';
import intercomSettings from '../../services/intercomService';
import studyRoomService from '../../services/studyRoomService';


export default {
  components:{
    logo,
    intercomSVG,
    unableToConnetStep,
    watchRecordedStep,
   // enableScreenStep,
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
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
  },
  methods:{
    ...mapActions(['setStepHistory', 'reOrderStepHistory', 'pushHistoryState', 'replaceHistoryState', 'setVisitedSettingPage', 'updateStudyRoomProps']),
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
    storeService.lazyRegisterModule(this.$store,'studyroomSettings_store',studyroomSettings_store);
    storeService.lazyRegisterModule(this.$store,'tutoringMain',tutoringMain);
    if(this.isMobile){
      this.$router.push({name:'tutoring', params:{id:this.id}})
    }
    global.onpopstate = (event)=>{
      this.goStep(event.state)
    }; 
    if(!!this.id){
      await studyRoomService.getRoomInformation(this.id).then((RoomProps) => {
        this.updateStudyRoomProps(RoomProps)
      })
    }
    
    this.setVisitedSettingPage(true);
    
    await tutorService.validateUserMedia(true, true);
    let firstPage = studyroomSettingsUtils.determinFirstPage();
    if(firstPage.type === "studyRoom"){
      this.$router.push({name:'tutoring', params:{id:this.id}})
    }else{
      this.currentStep = firstPage.type;
    }
    this.setStepHistory(this.currentStep);
    this.replaceHistoryState();
    console.log(firstPage);
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