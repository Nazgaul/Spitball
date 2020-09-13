<template>
   <v-card id="tutorFullScreen" color="black" class="tutorFullScreen OT_big">
         <div class="videoTools" v-if="isRoomTutor">
            <v-row class="videoBtns" justify="center">
               <v-tooltip top>
                  <template v-slot:activator="{ on }">
                     <v-btn v-on="on" :class="['controlsBtn',{'btnIgnoreClass':!isVideoActive}]" icon @click="toggleVideo" sel="video_class_enabling">
                        <v-icon v-if="isVideoActive" size="20" class="ms-1" color="white">sbf-video-camera</v-icon>
                        <v-icon v-else size="30" color="white">sbf-camera-ignore</v-icon>
                     </v-btn>
                  </template>
                  <span v-text="$t(isVideoActive?'tutor_tooltip_video_pause':'tutor_tooltip_video_resume')"/>
               </v-tooltip>
               <v-tooltip top>
                  <template v-slot:activator="{ on }">
                     <v-btn v-on="on" :class="['controlsBtn',{'btnIgnoreClass':!isAudioActive},'ms-3']" icon @click="toggleAudio" sel="audio_class_enabling">
                        <v-icon v-if="isAudioActive" size="22" color="white">sbf-microphone</v-icon>
                        <v-icon v-else size="28" color="white">sbf-mic-ignore</v-icon>
                     </v-btn>
                  </template>
                  <span v-text="$t(isAudioActive?'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute')"/>
               </v-tooltip>
            </v-row>
            <div class="videoMinimizeBtn" @click="closeFullScreen">
               <v-icon size="30" color="white">sbf-minis</v-icon>
            </div>
         </div>
      <div v-if="!isRoomTutor && isShowLowNetwork" class="lowNetworkMsg">
         <span class="lowNetworkMsg_span">
            <span class="me-1">💡</span>{{$t('lownetwork',[tutorName])}}
         </span>
      </div>
   </v-card>  
</template>

<script>
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         isShowLowNetwork:false,
      }
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.video;
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      tutorName(){
         return this.$store.getters.getRoomTutor?.tutorName;
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
      closeFullScreen(){
         this.$store.dispatch('updateToggleTutorFullScreen',false);
      },
      onVideoResolutionChange(e){
         let videoWidth = getNextResByCurrent(e.target.videoWidth);
         let isMobileVideo = e.target.videoWidth < e.target.videoHeight
         let box = document.getElementById('tutorFullScreen');
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
      tutorVideoTrack:{
         immediate:true,
         deep:true,
         handler(track){
            if(track){
               let self = this;
               this.$nextTick(()=>{
                  const localMediaContainer = document.getElementById('tutorFullScreen');
                  let videoTag = localMediaContainer.querySelector("video");
                  if (videoTag) {localMediaContainer.removeChild(videoTag)}
                  localMediaContainer.appendChild(track.attach());
                  let videoTagForEvent = localMediaContainer.querySelector("video");
                  videoTagForEvent.addEventListener('resize', self.onVideoResolutionChange); 
               })
            }
         }
      },
   },
}
</script>

<style lang="less">
.tutorFullScreen{     
   max-width: 100%;
   max-height: 100%;
   border-radius: 0 !important;
   position: relative;
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
      object-fit: cover;
      object-position: center;
      background-repeat: no-repeat;
      max-width: 100%;
      max-height: 100%;
      min-width: auto;
      transition: all .8s linear;
   }
   video::-webkit-media-controls-enclosure {
      display: none !important;
   }
   video::-webkit-media-controls {
      display:none !important;
   }
   .videoTools{
      position: absolute;
      background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 32%, rgba(0, 0, 0, 0.13) 61%, rgba(0, 0, 0, 0.49)) !important;
      width: 100%;
      height: 118px;
      bottom: 0;
      .videoMinimizeBtn{
         position: absolute;
         bottom: 14px;
         right: 12px;
      }
      .videoBtns{
         height: 100%;
         align-items: center;
         padding-top: 30px;
         .controlsBtn{
            width: 60px;
            height: 60px;
            background-color: rgba(0, 0, 0, 0.25);
            border-radius: 50%;
            &.btnIgnoreClass{
               background-color: rgba(255, 0, 0, 0.589);
            }
         }
      }
   }
}
</style>