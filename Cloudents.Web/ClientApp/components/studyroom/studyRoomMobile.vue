<template>
   <div class="studyRoomMobile">
      <div class="studyRoomMobileContent flex-column d-flex justify-space-between align-center">
         <div :id="elementId" class="d-flex flex-grow-0 flex-shrink-0">
            <span class="tutorName">{{roomTutorName}}</span>
            <div class="videoLiner"></div>
            <div class="videoPlaceHolderContainer" v-if="!tutorVideo">
               <div class="cameraCircle">
                  <v-icon size="26" color="#A9A9A9">sbf-camera-ignore</v-icon>
               </div>
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
   </div>
</template>

<script>

import chat from '../chat/components/messages.vue';
import { mapGetters } from 'vuex';
export default {
   components:{
      chat,
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
            height: ~"calc(100vh - 8px)";
            @media (max-width: @screen-xs) {
               height: ~"calc(100vh - 56px)";
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
               position: absolute;
               width: 100%;
               height: 100%;
               background-image: linear-gradient(to top, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
            }
            .videoPlaceHolderContainer{
               position: absolute;
               width: 100%;
               height: 100%; 
               display: flex;
               justify-content: center;
               align-items: center;
               .cameraCircle{
                  display: flex;
                  justify-content: center;
                  align-items: center;
                  border-radius: 50%;
                  width: 100px;
                  height: 100px;
                  background-color: #353537;
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