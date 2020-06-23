<template>
   <div class="studyRoomMobile">
      <div class="studyRoomMobileContent flex-column d-flex justify-space-between align-center">
      <v-content style="width: 100%;">
         <component style="width:100%" :is="currentMode"></component>
         <studyRoomWrapper v-show="false" style="width:100%"/>
      </v-content>
         <!-- <component style="width:100%" :is="currentMode"></component> -->
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

import studyRoomWrapper from './windows/studyRoomWrapper.vue'
import chat from '../chat/components/messages.vue';
import studyRoomMobileVideo from './studyRoomMobileVideo.vue';
export default {
   components:{
      chat,
      studyRoomMobileVideo,
      studyRoomWrapper
   },
   computed: {
      currentEditor(){
         return this.$store.getters.getActiveNavEditor 
      },
      roomModes(){
         return this.$store.getters.getRoomModeConsts;
      },
      currentMode(){
         switch(this.currentEditor) {
            // case this.roomModes.WHITE_BOARD:
            //    return 'studyRoomMobileVideo'
            //    break;
            case this.roomModes.TEXT_EDITOR:
               return 'studyRoomWrapper'
               break;
            case this.roomModes.CODE_EDITOR:
               return 'studyRoomWrapper'
               break;
            default:
               return 'studyRoomMobileVideo'
         }
      }
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