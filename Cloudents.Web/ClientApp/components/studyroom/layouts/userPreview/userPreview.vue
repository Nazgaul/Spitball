<template>
   <v-card :id="participant.id" color="black" class="userPreview" height="100" width="154">
      <span class="name">{{userName}}</span>
      <div class="linear"></div>
      <div class="linear2"></div>
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
      }
   },
   computed: {
      ...mapGetters(['getRoomParticipants']),
      currentParticipant(){
        return this.getRoomParticipants[this.participant.id];
      },
      userName(){
         return this.participant.name;
      }
   },
   methods: {
      handleAudioTrack(participant){
         if(participant.audio){
            if(this.audioTrack){
               return;
            }else{
               this.audioTrack = participant.audio;
               this.$nextTick(()=>{
                  let previewContainer = document.getElementById(participant.id);
                  let audioTag = previewContainer.querySelector("audio");
                  if (audioTag) {previewContainer.removeChild(audioTag)}
                  previewContainer.appendChild(participant.audio.attach());
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