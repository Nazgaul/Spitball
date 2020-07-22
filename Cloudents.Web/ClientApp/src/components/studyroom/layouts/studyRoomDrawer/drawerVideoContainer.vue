<template>
   <v-card v-if="isShowVideo" :id="elementId" :color="'black'" height="210" width="276">
      <span class="tutorName">{{roomTutorName}}</span>
      <div class="videoLiner"></div>
      <div class="drawerVideoTools" v-if="isRoomTutor">
         <div class="drawerVideoBtns">
            <template v-if="isClassMode">
               <v-tooltip top>
                  <template v-slot:activator="{ on }">
                     <v-btn v-on="on" icon class="fullScreenBtn" text @click="openFullScreen">
                        <v-icon size="22" class="ms-1" color="white">sbf-exp</v-icon>
                     </v-btn>
                  </template>
                  <span v-text="$t('tutor_tooltip_fullscreen')"/>
               </v-tooltip>
            </template>
            <v-tooltip top>
               <template v-slot:activator="{ on }">
                  <v-btn v-on="on" :class="['elevation-3','drawerControlsBtn',{'btnIgnore':!isAudioActive},'me-2']" icon @click="toggleAudio" sel="audio_enabling">
                     <v-icon v-if="isAudioActive" size="20" color="white">sbf-microphone</v-icon>
                     <v-icon v-else size="20" color="white">sbf-mic-ignore</v-icon>
                  </v-btn>
               </template>
               <span v-text="$t(isAudioActive?'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute')"/>
            </v-tooltip>

            <v-tooltip top>
               <template v-slot:activator="{ on }">
                  <v-btn v-on="on" :class="['elevation-3','drawerControlsBtn',{'btnIgnore':!isVideoActive},'ms-2']" icon @click="toggleVideo" sel="video_enabling">
                     <v-icon v-if="isVideoActive" size="14" color="white">sbf-videocam</v-icon>
                     <v-icon v-else size="20" color="white">sbf-videocam-off</v-icon>
                  </v-btn>
               </template>
               <span v-text="$t(isVideoActive?'tutor_tooltip_video_pause':'tutor_tooltip_video_resume')"/>
            </v-tooltip>
         </div>
      </div>
   </v-card>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         elementId: 'tutorVideoDrawer',
         tutorVideo: null,
      }
   },
   props:{
      isShowVideo:{
         type: Boolean
      },
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
      openFullScreen(){
         this.$store.dispatch('updateToggleTutorFullScreen',true);
      }
   },
   watch: {
      isShowVideo(newVal){
         if(!newVal){
            let localMediaContainer = document.getElementById(this.elementId);
            let videoTag = localMediaContainer.querySelector("video");
            if (videoTag) {localMediaContainer.removeChild(videoTag)} 
         }
         if(newVal && this.tutorVideo){
            let self = this;
            this.$nextTick(()=>{
               let localMediaContainer = document.getElementById(self.elementId);
               let videoTag = localMediaContainer.querySelector("video");
               if (videoTag) {localMediaContainer.removeChild(videoTag)} 
               localMediaContainer.appendChild(self.tutorVideo.attach());
            })
         }
      },
      tutorVideoTrack:{
         immediate:true,
         deep:true,
         handler(track){
            if(track){
               let self = this;
               this.$nextTick(()=>{
                  self.tutorVideo = track;
                  const localMediaContainer = document.getElementById(self.elementId);
                  if(localMediaContainer){
                     let videoTag = localMediaContainer.querySelector("video");
                     if (videoTag) {localMediaContainer.removeChild(videoTag)}
                     localMediaContainer.appendChild(track.attach());

                  }
               })
            }
            if(this.tutorVideo && !track){
               this.tutorVideo = null;
            }
         }
      },
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      roomTutorName(){
         return this.$store.getters.getRoomTutor.tutorName;
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      },
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.video;
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      isClassMode(){
         return this.$store.getters.getActiveNavEditor == this.$store.getters.getRoomModeConsts.CLASS_MODE;
      }
   },
}
</script>
<style lang="less">
   #tutorVideoDrawer{
      border-radius: initial;
      .tutorName{
         position: absolute;
         font-size: 14px;
         font-weight: 500;
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
      }
      .drawerVideoTools{
         position: absolute;
         width: 100%;
         height: 100%;
         background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
         .drawerVideoBtns{
            width: 100%;
            height: 100%;
            display: flex;
            justify-content: center;
            align-items: flex-end;
            padding-bottom: 10px;
            .fullScreenBtn{
               position: absolute;
               top: 8px;
               right: 8px;
            }
            .drawerControlsBtn{
               width: 44px;
               height: 44px;
               background-color: rgba(0, 0, 0, 0.15);
               border-radius: 50%;
               border: solid 1px #ffffff;
               &.btnIgnore{
                  background-color: #f7494a;
                  border: none;
               }
            }
         }
      }
      video {
         width: 100%;
         height: 100%;
         // object-fit: cover;
         // object-position: center;
         // background-repeat: no-repeat;
         pointer-events: none;
      }
      video::-webkit-media-controls-enclosure {
         display: none !important;
      }
   }
</style>