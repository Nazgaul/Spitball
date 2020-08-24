<template>
   <div class="classFullScreen">
      <div class="videoContainer">
         <div id="classFullScreenVideo"></div>
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
      </div>
      <v-footer :height="124" inset app fixed color="#212123" class="classFooter pa-0 py-3 ps-2">
         <v-slide-group
            class="pa-0"
            active-class="success"
            show-arrows
            color="#fff"
            prev-icon="sbf-arrow-left-carousel"
            next-icon="sbf-arrow-right-carousel">
            <template v-if="roomParticipants" >
               <v-slide-item v-for="participant in roomParticipants" :key="Object.values(participant)[0].id">
                  <userPreview :participant="Object.values(participant)[0]" class="classRoomCards mx-1"/>
               </v-slide-item>
            </template>
         </v-slide-group>
      </v-footer>
   </div>
</template>

<script>
import userPreview from '../../layouts/userPreview/userPreview.vue'
import { mapGetters } from 'vuex';
export default {
   components:{
      userPreview
   },
   watch: {
      tutorVideoTrack:{
         immediate:true,
         deep:true,
         handler(track){
            if(track){
               this.$nextTick(()=>{
                  const localMediaContainer = document.getElementById('classFullScreenVideo');
                  let videoTag = localMediaContainer.querySelector("video");
                  if (videoTag) {localMediaContainer.removeChild(videoTag)}
                  localMediaContainer.appendChild(track.attach());
               })
            }
         }
      }
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.video;
      },
      isVideoActive() {
         return this.$store.getters.getIsVideoActive;
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      },
      roomParticipants(){
         if(this.$store.getters.getRoomParticipants){
            let participants = Object.entries(this.$store.getters.getRoomParticipants).map((e) => ( { [e[0]]: e[1] } ));
            let participantIdx;
            let currentParticipant = participants.find((participant,index)=>{
               participantIdx = index;
               return Object.values(participant)[0].id == this.$store.getters.accountUser.id
            })
            if(currentParticipant){
               participants.splice(participantIdx,1)
               participants.unshift(currentParticipant)
            }
            return participants
         }else{
            return null
         }
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
      }
   },
}
</script>

<style lang="less">
.classFullScreen{
   background-color: #212123;
   display: block; //Just to remove cache
   .v-footer{
      justify-content: center;
   }
   .videoContainer{
      width: 100%;
      height: 100%;
      max-height: ~"calc(100vh - 186px)"; // 124px footer + 62px header
      overflow: hidden;
      position: relative;
      #classFullScreenVideo{
         height: ~"calc(100vh - 186px)"; // 124px footer + 62px header

         video {
            width: 100%;
            height: 100%;
            // object-fit: cover;
            object-position: center;
         }
           video::-webkit-media-controls-enclosure {
              display: none !important;
           }
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
   .classFooter{
      .classRoomCards{
         width: 154px;
         height: 100px;
      }
      .sbf {
         color: #fff;
      }
   }
}
</style>