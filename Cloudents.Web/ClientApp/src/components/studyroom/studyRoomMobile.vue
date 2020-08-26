<template>
   <div class="studyRoomMobile">
      <div class="studyRoomMobileContent flex-column d-flex justify-space-between align-center">


      <v-content style="width: 100%;" class="d-flex flex-grow-0 flex-shrink-0">
         <mobileControllers/>
         <component class="roomWrapper" :class="currentEditor" style="width:100%" :is="currentMode"></component>
         <div class="landscapeNotSupported">
            <div class="d-flex flex-column align-center justify-center mt-12">
               <div><landscape class="svgIcon"/></div>
               <div class="unSupportedText">
                  {{$t('unsupported_feature3')}}
               </div>
            </div>
         </div>
      </v-content>


         <!-- <div :id="elementId" class="d-flex flex-grow-0 flex-shrink-0">
            <mobileControllers/>
         </div> -->


         <div class="studyRoomMobileChatHeader mt-4">
            <div class="px-4 headerTitle mb-5 text-truncate">{{$store.getters.getRoomName}}</div>
            <div class="px-4 headerInfo d-flex justify-space-between mb-2">
               <span>
                  <v-icon class="me-1">sbf-message-icon</v-icon>
                  {{$t('studyRoom_chat')}}
               </span>
               <span v-if="$store.getters.getRoomIsBroadcast">
               <v-icon size="16" color="#7a798c" class="pe-1">sbf-users</v-icon>
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
import landscape from './landscape.svg';

import mobileControllers from './layouts/mobileControllers/mobileControllers.vue';
import studyRoomWrapper from './windows/studyRoomWrapper.vue'
import chat from '../chat/components/messages.vue';
import studyRoomMobileVideo from './studyRoomMobileVideo.vue';

import { mapGetters } from 'vuex';
export default {
   components:{
      chat,
      mobileControllers,
      studyRoomMobileVideo,
      studyRoomWrapper,
      landscape
   },
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
      currentMode(){
         switch(this.currentEditor) {
            case this.roomModes.WHITE_BOARD:
            return 'studyRoomWrapper'
            //    break;
            case this.roomModes.TEXT_EDITOR:
               return 'studyRoomWrapper'
               // break;
            case this.roomModes.CODE_EDITOR:
               return 'studyRoomWrapper'
               // break;
            default:
               return 'studyRoomMobileVideo'
         }
      },
      currentEditor(){
         return this.$store.getters.getActiveNavEditor 
      },
      roomModes(){
         return this.$store.getters.getRoomModeConsts;
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
         .landscapeNotSupported{
            display: none;
         }
         .white-board + .landscapeNotSupported,
         .shared-document + .landscapeNotSupported, 
         .code-editor + .landscapeNotSupported{
            @media (max-width: @screen-sm) and (orientation: portrait) {
               display: none;
            }
            @media (max-width: @screen-sm) and (orientation: landscape) {
               display: block;
               position: fixed;
               z-index: 150;
               top:0;
               left: 0;
               right: 0;
               bottom: 0;
               background: #fff;
               .unSupportedText {
                  margin-top: 20px;
                  font-size: 20px;
                  line-height: 1.6;
                  text-align: center;
                  color: #43425d;
                  max-width: 314px;
               }
               .svgIcon {
                  width: 80px;
               }
            }
         }


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
            //height: -webkit-fill-available; //safari
            //position: fixed;
            //left: 0;
            //right: 0;
            //top: 0;
            //bottom: 0;
            //height: ~"calc(100vh - 8px)";
            @media (max-width: @screen-xs) {
               //height: ~"calc(100vh - 56px)";
            }
         #studyRoomMobileVideo{
            width: 100%;
            min-height: 280px;
            background: black;
            position: relative;
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
               margin-bottom: 14px + 50px;
               .message_wrap{
                  &:last-child{
                     margin-bottom: 0; //override
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


               @media (max-width: @screen-sm) and (orientation: portrait) {
                     position: fixed;
                     bottom: 0;
                     left: 0;
                     right: 0;
               }

            }
         }
      }

   }
</style>