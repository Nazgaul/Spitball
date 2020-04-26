
<template>
  <div>
      <component :nextStep="nextStep" :is="currentStep" />
  </div>
</template>

<script>
// TODO: clean this file @idan to @maor
import {mapGetters, mapActions} from 'vuex';

import studyroomSettingsUtils from './studyroomSettingsUtils';
// import logo from '../../components/app/logo/logo-spitball.svg';
import watchRecordedStep from './components/watchRecordedStep.vue';
//import enableScreenStep from './components/enableScreenStep.vue';




export default {
  components:{
    watchRecordedStep,
  },
  data(){
    return{
      currentStep: null,
      stepHistory: [],
      currentPageIndex: 0,
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

    orderStepHistory(){
      let newStepHistory = this.stepHistory.slice(0, this.currentPageIndex+1);
      return newStepHistory;
    },
    openDialog(stepName){
      this.nextStepName = stepName;
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