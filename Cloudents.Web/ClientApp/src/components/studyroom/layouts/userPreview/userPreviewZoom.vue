<template>
   <v-card :id="participant.id" color="black" 
   :class="['userPreviewZoom']" >
   <div :class="['previewControls']">
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
         <span v-text="$t(isExpandVideoMode?'tutor_tooltip_fullscreen_exit':'tutor_tooltip_fullscreen')"/>
      </v-tooltip>
   </div>
      <span class="name">{{userName}}</span>
      <div class="linear2"></div>
      <div v-if="isCurrentParticipant" class="linear"></div>

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
      <div v-if="!isCurrentParticipant && isShowLowNetwork" class="lowNetworkMsg">
         <span class="lowNetworkMsg_span">
            <span class="me-1">💡</span>{{$t('lownetwork',[userName])}}
         </span>
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
         isShowLowNetwork:false,
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
               let self = this;
               this.$nextTick(()=>{
                  let previewContainer = document.getElementById(participant.id);
                  let videoTag = previewContainer.querySelector("video");
                  if (videoTag) {previewContainer.removeChild(videoTag)}
                  previewContainer.appendChild(participant.video.attach());
                  let videoTagForEvent = previewContainer.querySelector("video");
                  videoTagForEvent.addEventListener('resize', self.onVideoResolutionChange);
               })
            }
         }
         if(!participant.video && this.videoTrack){
            this.videoTrack = null;
            this.isExpandVideoMode = false;
         }
      },
      toggleExpandScreen(){
         let el = document.getElementById(this.participant.id);
         if(!this.isExpandVideoMode){
            if(!this.isCurrentParticipant){
               this.videoTrack.setPriority('standard')
            }
         }
         let videoTag = el.querySelector('video');

         if (videoTag.requestFullscreen) {
            videoTag.requestFullscreen();
         } else if (videoTag.webkitRequestFullscreen) {
            videoTag.webkitRequestFullscreen();
         } else if (videoTag.mozRequestFullScreen) {
            videoTag.mozRequestFullScreen();
         } else if (videoTag.msRequestFullscreen) {
            videoTag.msRequestFullscreen();
         }
         videoTag.classList.toggle('noAnimation')
         this.isExpandVideoMode = !this.isExpandVideoMode;
      },
      startShareScreen(){
         this.$store.dispatch('updateShareScreen',true)
      },
      stopShareScreen(){
         this.$store.dispatch('updateShareScreen',false)
      },
      onVideoResolutionChange(e){
         let videoWidth = getNextResByCurrent(e.target.videoWidth);
         let isMobileVideo = e.target.videoWidth < e.target.videoHeight
         let box = document.getElementById(this.participant.id);
         let videoEl = box?.querySelector('video');
         videoEl.width = videoWidth;
         // console.log('videoWidth <= 640:',videoWidth <= 640)
         // console.log('box.clientWidth >= 640:',box.clientWidth >= 640)
         let isLowNetwork = (
            (videoWidth <= 640 && !isMobileVideo) || 
            videoWidth <= 350 && isMobileVideo)
          && box.clientWidth >= 640;
         if(isLowNetwork){
            this.isShowLowNetwork = true;
         }else{
            this.isShowLowNetwork = false;
         }

         function getNextResByCurrent(width){
            if(width >= 960) return 1280;// height = 720;
            if(width >= 640) return 960;// height = 540;
            if(width >= 480) return 640;// height = 480;
            if(width >= 320) return 480;// height = 240;
            if(width >= 240) return 320;
            if(width >= 176) return 240;
         }
         // console.log(videoWidth)
         // console.log([e.target.videoWidth,e.target.videoHeight].join('x'))
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
      },
   },
   destroyed() {
      if(this.videoTrack){
         this.videoTrack.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   },
   mounted() {
      document.addEventListener('fullscreenchange',(ev)=>{
         if(!document.fullscreen){
            let videoTag = document.getElementById(this.participant.id).querySelector('video');
            videoTag.classList.remove('noAnimation')
            this.isExpandVideoMode = false;
         }
      },false)
   },
}
</script>

<style lang="less">
.userPreviewZoom{
   border-radius: 0 !important;
   position: relative;
   .previewControls{
      position: absolute;
      right: 6px;
      top: 6px;
      z-index: 2;
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
      // border-radius: 6px;
   }
   .linear2{
      // border-radius: 6px;
      position: absolute;
      width: 100%;
      height: 100%;
      background-image: linear-gradient(to top, rgba(0, 0, 0, 0) 30%, rgba(0, 0, 0, 0.1) 80%, rgba(0, 0, 0, 0.50));
   }
   .videoPreviewTools{
      position: absolute;
      top: 0;
      height: 100%;
      width: 100%;
      // border-radius: 6px;
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
   .lowNetworkMsg{
      color: #fff;
      font-size: 14px;
      z-index: 1;
      position: absolute;
      width: 100%;
      height: 100%;
      display: flex;
      justify-content: center;
      align-items: flex-end;
      .lowNetworkMsg_span{
         background: #0000006e;
         padding: 5px;
      }
   }
   video {
      // width: 100%;
      // height: 100%;
      object-fit: cover;
      object-position: center;
      background-repeat: no-repeat;
      // border-radius: 6px !important;
      max-width: 100%;
      max-height: 100%;
      min-width: auto;
      transition: all .8s linear;
      &.noAnimation{
         transition: none;
      }
   }
   video::-webkit-media-controls-enclosure {
      display: none !important;
   }
   video::-webkit-media-controls {
      display:none !important;
   }
}
</style>