<template>
   <v-card :id="participant.id" color="black" 
   :class="['userPreview',{'expandVideoMode':isExpandVideoMode}]" >
   <div :class="['previewControls',{'previewControlsExpanded':isExpandVideoMode}]">
      <v-tooltip top>
         <template v-slot:activator="{ on }">
            <v-icon v-on="on" :size="isExpandVideoMode? 30 :20" v-show="isCurrentParticipant"
                  @click="isShareScreen? stopShareScreen() : startShareScreen()" color="#ffffff"
                  class="me-2">{{isShareScreen? 'sbf-stop-share' : 'sbf-shareScreen'}}
            </v-icon>
         </template>
         <span> {{isShareScreen? $t('tutor_btn_stop_sharing'): $t('tutor_btn_share_screen')}}) </span>
      </v-tooltip>

      <v-tooltip top>
         <template v-slot:activator="{ on }">
            <v-icon v-on="on" :size="isExpandVideoMode? 30 :18" v-show="showExpandVideoBtn" 
                  @click="toggleExpandScreen" color="#ffffff">
                  {{isExpandVideoMode? 'sbf-minis':'sbf-exp'}}
            </v-icon>
         </template>
         <span v-text="isExpandVideoMode ? $t('tutor_tooltip_fullscreen_exit') : $t('tutor_tooltip_fullscreen')"/>
      </v-tooltip>
   </div>
      <span class="name">{{userName}}</span>
      <div v-if="!isExpandVideoMode" class="linear2"></div>
      <div v-if="!isExpandVideoMode || isExpandVideoMode && isCurrentParticipant" class="linear"></div>

      <div class="videoPreviewTools" v-if="isCurrentParticipant">
         <v-tooltip top>
            <template v-slot:activator="{ on }">
               <v-btn v-on="on" :class="['userPreviewControlsBtn',{'userPreviewbtnIgnore':!isVideoActive}]" icon @click="toggleVideo" sel="video_enabling">
                  <v-icon v-if="isVideoActive" size="10" color="white">sbf-video-camera</v-icon>
                  <v-icon v-else size="17" color="white">sbf-camera-ignore</v-icon>
               </v-btn>
            </template>
            <span v-text="isVideoActive?$t('tutor_tooltip_video_pause'):$t('tutor_tooltip_video_resume')" />
         </v-tooltip>
         <v-tooltip top>
            <template v-slot:activator="{ on }">
               <v-btn v-on="on" :class="['userPreviewControlsBtn',{'userPreviewbtnIgnore':!isAudioActive},'ms-2']" icon @click="toggleAudio" sel="audio_enabling">
                  <v-icon v-if="isAudioActive" size="16" color="white">sbf-microphone</v-icon>
                  <v-icon v-else size="16" color="white">sbf-mic-ignore</v-icon>
               </v-btn>
            </template>
            <span v-text="$t(isAudioActive?'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute')"/>
         </v-tooltip>
      </div>
      <template v-if="audioTrack && !isCurrentParticipant">
         <v-progress-linear class="audioMeterUser" rounded absolute color="#16eab1" height="6" :value="audioLevel" buffer-value="0"></v-progress-linear>
      </template>
   </v-card>  
</template>

<script>
import { mapGetters } from 'vuex';
import pollaudiolevel from './pollaudiolevel.js'
export default {
   props:{
      participant:{
         type: Object,
         required:true
      },
   },
   data() {
      return {
         videoTrack:null,
         audioTrack:null,
         isExpandVideoMode:false,
         audioLevel:0,
      }
   },
   computed: {
      ...mapGetters(['getRoomParticipants']),
      currentParticipant(){
        return this.getRoomParticipants[this.participant.id];
      },
      userName(){
         if(this.isCurrentParticipant){
            return this.$t('studyRoom_user_preview_you')
         }else{
            return this.participant.name;
         }
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      isCurrentParticipant(){
         return this.currentParticipant?.id == this.$store.getters.accountUser?.id
      },
      showExpandVideoBtn(){
         let isClassRoom = this.$store.getters.getActiveNavEditor == this.$store.getters.getRoomModeConsts.CLASS_MODE;
         return isClassRoom && this.videoTrack;
      },
      isShareScreen(){
         return this.$store.getters.getIsShareScreen;
      }
   },
   methods: {
      toggleVideo() {
         this.$ga.event("tutoringRoom", "toggleVideo");
         this.$store.dispatch("updateVideoToggle");
      },
      toggleAudio() {
         this.$ga.event("tutoringRoom", "toggleAudio");
         this.$store.dispatch("updateAudioToggle");
      },
      onAudioLevelChanged(level){
         this.audioLevel = level * 5;
      },
      handleAudioTrack(participant){
         if(this.isCurrentParticipant) return; //user dont need his audio only the remote need

         if(participant.audio){
            if(this.audioTrack){
               return;
            }else{
               this.audioTrack = participant.audio;
               let self = this;
               this.$nextTick(()=>{
                  if(!self.isCurrentParticipant){
                     pollaudiolevel(self.audioTrack,self.onAudioLevelChanged)
                  }
               })
            }
         }
         if(!participant.audio && this.audioTrack){
            this.audioTrack = null;
         }
      },
      handleVideoTrack(participant){
         
         if(participant.video){
            if(this.videoTrack && participant.video == this.videoTrack.name){
               return;
            }else{
               this.videoTrack = participant.video;
               this.$nextTick(()=>{
                  let previewContainer = document.getElementById(participant.id);
                  let videoTag = previewContainer.querySelector("video");
                  if (videoTag) {previewContainer.removeChild(videoTag)}
                  previewContainer.appendChild(participant.video.attach());
               })
            }
         }
         if(!participant.video && this.videoTrack){
            this.videoTrack = null;
            this.isExpandVideoMode = false;
         }
      },
      toggleExpandScreen(){
         if(!this.isExpandVideoMode){
            if(!this.isCurrentParticipant){
               this.videoTrack.setPriority('standard')
            }  
            this.videoTrack.dimensions.width = 1280;
            this.videoTrack.dimensions.height = 720;
         }
         this.isExpandVideoMode = !this.isExpandVideoMode
      },
      startShareScreen(){
         this.$store.dispatch('updateShareScreen',true)
      },
      stopShareScreen(){
         this.$store.dispatch('updateShareScreen',false)
      }
   },
   watch: {
      currentParticipant:{
         immediate:true,
         deep:true,
         handler(val){
            this.handleAudioTrack(val);
            this.handleVideoTrack(val);
         }
      }
   },
   destroyed() {
      if(this.videoTrack){
         this.videoTrack.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   },
}
</script>

<style lang="less">
.userPreview{
   border-radius: 6px !important;
   position: relative;
   &.expandVideoMode{
      position: absolute;
      margin: 0 !important;
      width: 100% !important;
      height: 100% !important;
      z-index: 3;
      video{
         object-fit: contain;
      }
   }
   .previewControls{
      position: absolute;
      right: 6px;
      top: 6px;
      z-index: 2;
      &.previewControlsExpanded{
         bottom: 12px;
         top: initial;
         right: 16px;
      }
   }
   .name{
      text-transform: capitalize;
      font-size: 14px;
      font-weight: 600;
      color: #ffffff;
      position: absolute;
      z-index: 1;
      top: 2px;
      left: 6px;
   }
   .audioMeterUser{
      width: 80%;
      left: 10px;
      bottom: 8px;
   }
   .linear{
      position: absolute;
      width: 100%;
      height: 100%;
      background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
      border-radius: 6px;
   }
   .linear2{
      border-radius: 6px;
      position: absolute;
      width: 100%;
      height: 100%;
      background-image: linear-gradient(to top, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
   }
   .videoPreviewTools{
      position: absolute;
      top: 0;
      height: 100%;
      width: 100%;
      border-radius: 6px;
      display: flex;
      justify-content: center;
      align-items: flex-end;
      padding-bottom: 6px;
      z-index: 1;
      .userPreviewControlsBtn{
         width: 33px;
         height: 33px;
         background-color: rgba(0, 0, 0, 0.25);
         border-radius: 50%;
         &.userPreviewbtnIgnore{
            background-color: rgba(255, 0, 0, 0.589);
         }
      }
   }
   video {
      width: 100%;
      height: 100%;
      object-fit: cover;
      object-position: center;
      background-repeat: no-repeat;
      border-radius: 6px !important;
   }
}
</style>