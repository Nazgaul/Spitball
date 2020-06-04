<template>
   <v-navigation-drawer 
         mobile-break-point="960" app right clipped
         class="studyRoomDrawer" :width="drawerExtend ? 300 : 12">
   <button @click="drawerExtend = !drawerExtend" class="collapseBtnDrawer">
      <v-icon class="pa-1" color="#7a798c" size="16" v-text="drawerExtend? 'sbf-arrow-right-carousel': 'sbf-arrow-left-carousel'"></v-icon>
   </button>
   <div :class="[{'hiddenDrawer':!drawerExtend}]" class="drawerContent flex-column d-flex justify-space-between align-center">
      <drawerVideoContainer :isShowVideo="isShowVideo" class="mt-3 d-flex flex-grow-0 flex-shrink-0 elevation-0"/>
      <div class="drawerChatHeader mt-3">
         <div class="headerTitle mb-5 text-truncate">{{$store.getters.getRoomName}}</div>
         <div class="headerInfo d-flex justify-space-between mb-2">
            <span>
               <v-icon class="me-1">sbf-message-icon</v-icon>
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
import drawerVideoContainer from './drawerVideoContainer.vue';
export default {
   data() {
      return {
        drawerExtend:true,
      }
   },
   components:{
      chat,
      drawerVideoContainer,
   },
   computed: {
      isShowVideo(){
         return this.$store.getters.getActiveNavEditor !== 'class-screen'
      },
   },
   watch: {
      drawerExtend:{
         immediate:true,
         handler(newVal){
            this.$store.commit('setStudyRoomDrawer',newVal);
         }
      }
   },
}
</script>

<style lang="less">
   .studyRoomDrawer {
      overflow: initial; // to let the collapse btn to show
      box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.11);
      &.v-navigation-drawer{
         left: auto !important /*rtl:ignore */;
         right: 0 !important /*rtl:ignore */;
      }
      .drawerContent{
         &.hiddenDrawer{
            visibility: hidden;
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
            .messages-container{
               height: initial;
            }
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
      .collapseIcon {
         width: 32px;

         border-radius: 0%; //vuetify override
         border-top-left-radius: 8px;
         border-bottom-left-radius: 8px;
      }
      .collapseBtnDrawer{
         position: absolute;
         left: -26px/*rtl:ignore */;
         background: #ffffff;
         width: 32px;
         height: 32px;
         top: 0;
         z-index: 1;
         border-top-left-radius: 8px /*rtl:ignore */;
         border-bottom-left-radius: 8px /*rtl:ignore */;
         outline: none;
      }
   }
</style>