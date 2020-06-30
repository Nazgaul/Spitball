<template>
   <div class="studyRoomMobile">
      <div class="studyRoomMobileContent flex-column d-flex justify-space-between align-center">
         <div :id="elementId" class="d-flex flex-grow-0 flex-shrink-0">
            <span class="tutorName">{{roomTutorName}}</span>
            <div class="videoLiner">
               <v-btn icon @click="toggleAudio" sel="audio_enabling"
               :class="['micControl','drawerControlsBtn',{'btnIgnore':!isAudioActive},'mb-2']" >
                  <v-icon v-if="isAudioActive" size="30" color="white">sbf-microphone</v-icon>
                  <v-icon v-else size="30" color="white">sbf-mic-ignore</v-icon>
               </v-btn>
            </div>
         </div>
         <div class="studyRoomMobileChatHeader mt-4">
            <div class="px-4 headerTitle mb-5 text-truncate">{{$store.getters.getRoomName}}</div>
            <div class="px-4 headerInfo d-flex justify-space-between mb-2">
               <span>
                  <v-icon class="mr-1">sbf-message-icon</v-icon>
                  {{$t('studyRoom_chat')}}
               </span>
               <span v-if="$store.getters.getRoomIsBroadcast">
               <v-icon size="16" color="#7a798c" class="pr-1">sbf-users</v-icon>
               {{$store.getters.getRoomParticipantCount}}
               </span>
            </div>
            <v-divider></v-divider>
         </div>
         <v-sheet class="chatContainer d-flex flex-grow-1">
            <chat></chat>
         </v-sheet>
      </div>
      <!-- hotfix for mobile audios -->
      <div v-show="false" v-if="roomParticipants">
         <div v-for="participant in roomParticipants" :key="Object.values(participant)[0].id">
            <userPreview :participant="Object.values(participant)[0]" class="classRoomCards mx-1"/>
         </div>
      </div>
   </div>
</template>

<script>

import chat from '../chat/components/messages.vue';
import { mapGetters } from 'vuex';
import userPreview from './layouts/userPreview/userPreview.vue';
export default {
   components:{
      chat,
      userPreview
   },
   data() {
      return {
         tutorAudio:null,
         tutorVideo:null,
         elementId:'studyRoomMobileVideo'
      }
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorAudioTrack(){
         return this.getRoomTutorParticipant?.audio;
      },
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.screen || this.getRoomTutorParticipant?.video
      },
      roomTutorName(){
         return this.$store.getters.getRoomTutor.tutorName;
      },
      roomParticipants(){
         if(this.$store.getters.getRoomParticipants){
            let participants = Object.entries(this.$store.getters.getRoomParticipants).map((e) => ( { [e[0]]: e[1] } ));
           return participants
         }else{
            return null
         }
      },
      isAudioActive() {
         return this.$store.getters.getIsAudioActive;
      },
   },
   watch: {
      tutorAudioTrack:{
         immediate:true,
         deep:true,
         handler(track){
            if(track){
               let self = this;
               this.$nextTick(()=>{
                  self.tutorAudio = track;
                  const localMediaContainer = document.getElementById(self.elementId);
                  if(localMediaContainer){
                     let audioTag = localMediaContainer.querySelector("audio");
                     if (audioTag) {localMediaContainer.removeChild(audioTag)}
                     localMediaContainer.appendChild(track.attach());
                     return
                  }
               })
            }
            if(this.tutorAudio && !track){
               this.tutorAudio = null;
            }
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
   .studyRoomMobile {
      width: 100%;
      .studyRoomMobileContent{
            ::-webkit-scrollbar-track {
                  background: #f5f5f5; 
            }
            ::-webkit-scrollbar {
                  width: 10px;
            }
            ::-webkit-scrollbar-thumb {
                  background: #bdc0d1 !important;
                  border-radius: 4px !important;
            }
            height: 100vh;
            height: -webkit-fill-available; //safari
            position: fixed;
            left: 0;
            right: 0;
            top: 0;
            bottom: 0;
            //height: ~"calc(100vh - 8px)";
            @media (max-width: @screen-xs) {
               //height: ~"calc(100vh - 56px)";
            }
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
         .studyRoomMobileChatHeader{
            @media (max-width: @screen-sm) and (orientation: landscape) {
               display: none !important;
            }
            width: 100%;
            // padding: 0 12px;
            .headerTitle{
               font-size: 14px;
               font-weight: 600;
               color: #43425d;
            }
            .headerInfo{
               font-size: 14px;
               font-weight: 600;
               color: #665d81;
            }
         }
         .chatContainer{
            @media (max-width: @screen-sm) and (orientation: landscape) {
               display: none !important;
            }
            width: 100%;
            margin-top: 14px;
            overflow-y: hidden;
            .messages-container{
               height: initial;
            }
            .messages-body{
               padding-bottom: 0;
               margin-bottom: 0;
               .message_wrap{
                  &:last-child{
                     margin-bottom: 14px;
                  }
               }
            }
            .messages-input{
               background: white;
               box-shadow: none;
               .chat-upload-wrap{
                  .chat-input-container{
                     .chat-attach-file{
                        color: #4c59ff;
                     }
                     .chat-camera{
                        .photo-camera_svg__chatUploadIconSvg{
                           fill: #4c59ff !important;
                        }
                     }
                     .chat-photo{
                        .outline-insert-photo_svg__chatUploadIconSvg{
                           fill: #4c59ff !important;
                        }
                     }
                  } 
               } 
               .messages-textarea{
                  .v-input__control{
                     .v-input__slot{
                        border: solid 1px #b8c0d1;
                     }
                  } 
               } 

            }
         }
      }
   }
</style>