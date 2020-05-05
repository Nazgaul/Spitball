<template>
   <v-navigation-drawer app right clipped class="studyRoomDrawer" :width="drawerExtend ? 300 : 0">
   <v-btn icon class="collapseIcon" @click="drawerExtend = !drawerExtend" color="#fff">
      <v-icon v-text="drawerExtend? 'sbf-arrow-right-carousel': 'sbf-arrow-left-carousel'"></v-icon>
   </v-btn>

   <div class="drawerContent flex-column d-flex justify-space-between align-center">
      <v-card id="tutorVideoDrawer" v-if="isShowVideo" class="mt-2 d-flex flex-grow-0 flex-shrink-0 elevation-0" :color="'grey lighten-1'" height="210" width="276">
         <span class="tutorName">{{roomTutorName}}</span>
         <div class="videoLiner"></div>
      </v-card>
      <div class="drawerChatHeader mt-4">
         <div class="headerTitle mb-5 text-truncate">{{$store.getters.getRoomName}}</div>
         <div class="headerInfo d-flex justify-space-between mb-4">
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
   </v-navigation-drawer>
</template>

<script>
import chat from '../../../chat/components/messages.vue';
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
        drawerExtend:true, 
      }
   },
   components:{
      chat,
   },
   watch: {
      isShowVideo(newVal){
         if(!newVal){
            let localMediaContainer = document.getElementById('tutorVideoDrawer');
            let videoTag = localMediaContainer.querySelector("video");
            if (videoTag) {localMediaContainer.removeChild(videoTag)} 
         }
         if(newVal){
            let self = this;
            this.$nextTick(()=>{
               let localMediaContainer = document.getElementById('tutorVideoDrawer');
               let videoTag = localMediaContainer.querySelector("video");
               if (videoTag) {localMediaContainer.removeChild(videoTag)} 
               localMediaContainer.appendChild(self.getTutorVideoTrack.attach());
            })
         }
      },
      getTutorVideoTrack:{
         immediate:true,
         deep:true,
         handler(newVal){
            if(newVal){
               const localMediaContainer = document.getElementById('tutorVideoDrawer');
               let videoTag = localMediaContainer.querySelector("video");
               if (videoTag) {localMediaContainer.removeChild(videoTag)}
               localMediaContainer.appendChild(newVal.attach());
            }
         }
      }
   },
   computed: {
      ...mapGetters(['getTutorVideoTrack']),
      isShowVideo(){
         return this.$store.getters.getActiveNavEditor !== 'class-screen'
      },
      roomTutorName(){
         return this.$store.getters.getRoomTutor.tutorName;
      }
   },
}
</script>

<style lang="less">
   .studyRoomDrawer {
      overflow: initial; // to let the collapse btn to show
      &.v-navigation-drawer{
         left: auto !important;
         right: 0 !important;
      }
      .drawerContent{
         #tutorVideoDrawer{
            border-radius: 6px;
            .tutorName{
               position: absolute;
               font-size: 14px;
               font-weight: 600;
               color: #ffffff;
               top: 8px;
               left: 8px;
               z-index: 1;
            }
            .videoLiner{
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
              pointer-events: none;
              border-radius: 6px;
           }
           video::-webkit-media-controls-enclosure {
              display: none !important;
           }
         }
         ::-webkit-scrollbar-track {
               background: #f5f5f5; 
         }
         ::-webkit-scrollbar {
               width: 10px;
         }
         ::-webkit-scrollbar-thumb {
               background: #b5b8d9 !important;
               border-radius: 4px !important;
         }
         height: ~"calc(100vh - 68px)";
         .drawerChatHeader{
            width: 100%;
            padding: 0 12px;
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
            width: 100%;
            margin-top: 14px;
            overflow-y: hidden;
            .messages-body{
               padding-bottom: 0;
               margin-bottom: 0;
            }
            .messages-input{
               background: white;
               box-shadow: none;
               .chat-upload-wrap{
                  .chat-input-container{
                     .chat-attach-file{
                        color: #4c59ff;
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
      // position: relative;
      .collapseIcon {
         position: absolute;
         top: 20px;
         left: -35px;
         background: #212123;
         border-radius: 0%; //vuetify override
      }
   }
</style>