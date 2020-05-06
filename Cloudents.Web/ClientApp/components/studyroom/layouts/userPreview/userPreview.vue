<template>
   <v-card :id="participant.id" color="black" class="userPreview" height="100" width="154">
      <span class="name">{{userName}}</span>
      <div class="linear"></div>
      <div class="linear2"></div>
      <div class="videoPreviewTools" v-if="isCurrentParticipant">
         <v-tooltip top>
            <template v-slot:activator="{ on }">
               <v-btn v-on="on" :class="['userPreviewControlsBtn',{'userPreviewbtnIgnore':!isVideoActive}]" icon @click="toggleVideo" sel="video_enabling">
                  <v-icon v-if="isVideoActive" size="11" class="ml-1" color="white">sbf-video-camera</v-icon>
                  <v-icon v-else size="18" color="white">sbf-camera-ignore</v-icon>
               </v-btn>
            </template>
            <span v-text="$t(isVideoActive?'tutor_tooltip_video_pause':'tutor_tooltip_video_resume')"/>
         </v-tooltip>
         <v-tooltip top>
            <template v-slot:activator="{ on }">
               <v-btn v-on="on" :class="['userPreviewControlsBtn',{'userPreviewbtnIgnore':!isAudioActive},'ml-2']" icon @click="toggleAudio" sel="audio_enabling">
                  <v-icon v-if="isAudioActive" size="16" color="white">sbf-microphone</v-icon>
                  <v-icon v-else size="16" color="white">sbf-mic-ignore</v-icon>
               </v-btn>
            </template>
            <span v-text="$t(isAudioActive?'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute')"/>
         </v-tooltip>
      </div>
      <div class="audioMeter" v-if="audioTrack && !isCurrentParticipant" :id="audioMeterId"></div>
   </v-card>  
</template>

<script>
import { mapGetters } from 'vuex';
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
         audioMeterId: `audioMeter_${this.participant.id}`,
         audioContext:null,
         input:null,
         analyser:null,
         scriptProcessor:null,
      }
   },
   computed: {
      ...mapGetters(['getRoomParticipants']),
      currentParticipant(){
        return this.getRoomParticipants[this.participant.id];
      },
      userName(){
         return this.participant.name;
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      isCurrentParticipant(){
         return this.currentParticipant?.id == this.$store.getters.accountUser?.id
      }
   },
   methods: {
      processInput(){
         let array = new Uint8Array(this.analyser.frequencyBinCount);
         this.analyser.getByteFrequencyData(array);
         let values = 0;

         let length = array.length;
         let i;
         for (i = 0; i < length; i++) {
               values += (array[i]);
         }

         let average = values / length;

         let micVolume = document.getElementById(this.audioMeterId);
         if (!micVolume) return;
         micVolume.style.backgroundColor = '#16eab1';
         micVolume.style.height = '6px';
         micVolume.style.borderRadius = '2px';
         micVolume.style.maxWidth = '40px';
         micVolume.style.width = `${Math.round(average)}px`;
      },
      createAudioMeter(audioTrack){
         // audioTrack.media somehting..check it
         this.audioContext = new (AudioContext || webkitAudioContext)();
         this.input = this.audioContext.createMediaStreamSource(audioTrack);
         this.analyser = this.audioContext.createAnalyser();
         this.scriptProcessor = this.audioContext.createScriptProcessor();

         // Some analyser setup
         this.analyser.smoothingTimeConstant = 0.3;
         this.analyser.fftSize = 1024;

         this.input.connect(this.analyser);
         this.analyser.connect(this.scriptProcessor);
         this.scriptProcessor.connect(this.audioContext.destination);
         this.scriptProcessor.onaudioprocess = this.processInput;

      },
      toggleVideo() {
         this.$ga.event("tutoringRoom", "toggleVideo");
         this.$store.dispatch("updateVideoToggle");
      },
      toggleAudio() {
         this.$ga.event("tutoringRoom", "toggleAudio");
         this.$store.dispatch("updateAudioToggle");
      },
      handleAudioTrack(participant){
         if(participant.audio){
            if(this.audioTrack){
               return;
            }else{
               this.audioTrack = participant.audio;
               let self = this;
               this.$nextTick(()=>{
                  let previewContainer = document.getElementById(participant.id);
                  let audioTag = previewContainer.querySelector("audio");
                  if (audioTag) {previewContainer.removeChild(audioTag)}
                  previewContainer.appendChild(participant.audio.attach());
                  if(!self.isCurrentParticipant){
                     let domStream = previewContainer.querySelector("audio").captureStream()
                     self.createAudioMeter(domStream)
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
            if(this.videoTrack){
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
         }
      },
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
      if(this.audioTrack){
         this.audioTrack.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   },
}
</script>

<style lang="less">
.userPreview{
   border-radius: 6px !important;
   .name{
      margin-left: 8px;
      font-size: 14px;
      font-weight: 600;
      color: #ffffff;
      position: absolute;
      z-index: 1;
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
   .audioMeter{
      position: absolute;
      width: 100%;
      height: 100%;
      bottom: 8px;
      left: 12px;
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
      padding-bottom: 10px;
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