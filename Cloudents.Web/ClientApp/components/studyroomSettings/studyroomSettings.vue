<script>
// TODO: clean this file @idan to @maor
import {mapActions} from 'vuex';

export default {

  data(){
    return{
      currentStep: null,
      currentPageIndex: 0,
      nextStepName: '',
    }
  },
  methods:{
    ...mapActions([ '', 'setVisitedSettingPage']),

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
  }   
}

</script>