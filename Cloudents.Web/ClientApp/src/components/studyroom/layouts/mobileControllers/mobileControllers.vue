<template>
  <div class="mobileControllers">
      <span class="tutorName">{{roomTutorName}}</span>
      <div class="videoLiner">
         <v-fade-transition>
            <div v-if="showMediaToaster.isOn" class="mediaToaster" >
               <template v-if="showMediaToaster.mode == 'audio'">
                  <v-icon v-text="isAudioActive?'sbf-microphone':'sbf-mic-ignore'" 
                     class="icon" size="36" color="white"></v-icon>
                  <p class="text">{{isAudioActive? $t('mic_on') : $t('mic_off')}}</p>
               </template>
               <template v-else>
                  <v-icon v-text="isVideoActive?'sbf-videocam':'sbf-videocam-off'" 
                     class="icon" size="36" color="white"></v-icon>
                  <p class="text">{{isVideoActive? $t('vid_on') : $t('vid_off')}}</p>
               </template>
            </div>
         </v-fade-transition>

         <v-btn icon @click="toggleAudio"
         :class="['btnControl','mediaControl',{'btnIgnore':!isAudioActive},'mb-2','me-4','elevation-3']" >
            <v-icon v-if="isAudioActive" size="20" color="white">sbf-microphone</v-icon>
            <v-icon v-else size="20" color="white">sbf-mic-ignore</v-icon>
         </v-btn>
         
         <v-btn icon @click="endSession" :class="['elevation-3','btnControl','mediaControl','disconnectBtn','mb-2','me-4']" >
            <v-icon size="10" color="#f7494a">sbf-callEnd</v-icon>
         </v-btn>

         <v-btn icon @click="toggleVideo"
         :class="['elevation-3','btnControl','mediaControl',{'btnIgnore':!isVideoActive},'mb-2']" >
            <v-icon v-if="isVideoActive" size="14" color="white">sbf-videocam</v-icon>
            <v-icon v-else size="20" color="white">sbf-videocam-off</v-icon>
         </v-btn>
      </div>
  </div>
</template>

<script>
export default {
   name:'mobileControllers',
   data() {
      return {
         showMediaToaster:{
            isOn:false,
            mode:'audio'
         }
      }
   },
   computed: {
      roomTutorName(){
         return this.$store.getters.getRoomTutor.tutorName;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
   },

   watch: {
      showMediaToaster:{
         deep:true,
         handler(newVal){
            if(newVal.isOn){
               setTimeout(() => {
                  this.showMediaToaster.isOn = false
               }, 1000);
            }
         }
      }
   },
   methods: {
      toggleAudio() {
         if(!this.$store.getters.getIsAudioAvailable || this.showMediaToaster.isOn) return;
         this.showMediaToaster = {
            isOn:true,
            mode:'audio'
         };
         
         this.$ga.event("tutoringRoom", "toggleAudio");
         this.$store.dispatch("updateAudioToggle");
      },
      toggleVideo() {
         if(!this.$store.getters.getIsVideoAvailable || this.showMediaToaster.isOn) return;
         this.showMediaToaster = {
            isOn:true,
            mode:'video'
         };
         this.$ga.event("tutoringRoom", "toggleVideo");
         this.$store.dispatch("updateVideoToggle");
      },
      endSession() {
         this.$ga.event("tutoringRoom", "endSession");
         this.$store.dispatch('updateEndSession')
      },
   },

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.mobileControllers{
   width: 100%;
   height: 100%;
   position: absolute;
   .tutorName{
      position: absolute;
      font-size: 14px;
      font-weight: 600;
      color: #ffffff;
      top: 6px;
      left: 8px;
      z-index: 1;
   }
   .videoLiner{
      position: absolute;
      width: 100%;
      height: 100%;
      background-image: linear-gradient(to top, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
      display: flex;
      align-items: flex-end;
      justify-content: center;
      @media (max-width: @screen-sm) and (orientation: landscape) {
         top: 0;
         left: 0;
         right: 0;
         bottom: 0;
         width: 100vw;
         height: 100vh;
      }
      .mediaToaster{
         z-index: 1;
         position: absolute;
         top: calc(~"50% - 45px");
         width: 121px;
         height: 91px;
         border-radius: 6px;
         background-color: rgba(0, 0, 0, 0.5);
         text-align: center;
         padding: 12px;
         display: flex;
         flex-direction: column;
         justify-content: space-between;
         .text{
            opacity: 0.6;
            font-size: 13px;
            font-weight: 600;
            color: #ffffff;
            margin: 0;
            padding: 0;
         }
         .icon{
            opacity: 0.6;
         }
      }
      .btnControl{
         z-index: 1;
         width: 44px;
         height: 44px;
         border-radius: 50%;
         &.mediaControl{
            border: solid 1px #ffffff;
            background-color: rgba(0, 0, 0, 0.15);
            &.btnIgnore{
               background-color: #f7494a;
               border: none;
            }
         }
         &.disconnectBtn{
            background-color: white;
         }
      }
   }
}
</style>