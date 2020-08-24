<template>
   <div :id="elementId" class="d-flex align-center flex-grow-0 flex-shrink-0">
      <!-- <span class="tutorName">{{roomTutorName}}</span> -->
      <!-- <div class="videoLiner">
         <v-btn icon @click="toggleAudio" sel="audio_enabling"
         :class="['micControl','drawerControlsBtn',{'btnIgnore':!isAudioActive},'mb-2']" >
            <v-icon v-if="isAudioActive" size="30" color="white">sbf-microphone</v-icon>
            <v-icon v-else size="30" color="white">sbf-mic-ignore</v-icon>
         </v-btn>
      </div> -->
   </div>  
</template>

<script>
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         tutorVideo:null,
         elementId:'studyRoomMobileVideo'
      }
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.screen || this.getRoomTutorParticipant?.video
      },
      roomTutorName(){
         return this.$store.getters.getRoomTutor.tutorName;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
   },
   watch: {
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
                     return
                  }
               })
            }
            if(this.tutorVideo && !track){
               this.tutorVideo = null;
            }
         }
      },
   },
   methods: {
      toggleAudio() {
         this.$ga.event("tutoringRoom", "toggleAudio");
         this.$store.dispatch("updateAudioToggle");
      },
   },
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
         #studyRoomMobileVideo{
            width: 100%;
            min-height: 280px;
            background: black;
            position: relative;
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

               @media (max-width: @screen-sm) and (orientation: landscape) {
                  top: 0;
                  left: 0;
                  right: 0;
                  bottom: 0;
                  width: 100vw;
                  height: 100vh;
               }

               position: absolute;
               width: 100%;
               height: 100%;
               background-image: linear-gradient(to top, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
               display: flex;
               align-items: flex-end;
               justify-content: center;

               .micControl{
                  z-index: 1;
                  &.drawerControlsBtn{
                  width: 60px;
                  height: 60px;
                  background-color: rgba(0, 0, 0, 0.589);
                  border-radius: 50%;
                     &.btnIgnore{
                        background-color: rgba(255, 0, 0, 0.589);
                     }
                  }
               }
            }
            video {
               width: 100%;
               height: 100%;
               pointer-events: none;
               max-height: 50vh;
               @media (max-width: @screen-sm) and (orientation: landscape) {
                  position: fixed;
                  top: 0;
                  left: 0;
                  right: 0;
                  bottom: 0;
                  width: 100vw;
                  height: 100vh;
                  max-height: initial;
                  background: #000;
               }
            }
            video::-webkit-media-controls-enclosure {
               display: none !important;
            }
         }
</style>