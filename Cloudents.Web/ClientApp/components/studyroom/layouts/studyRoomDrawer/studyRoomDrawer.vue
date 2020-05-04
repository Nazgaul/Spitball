<template>
   <v-navigation-drawer app right clipped class="studyRoomDrawer" :width="drawerExtend ? 300 : 0">
   <v-btn icon class="collapseIcon" @click="drawerExtend = !drawerExtend" color="#fff">
      <v-icon v-text="drawerExtend? 'sbf-arrow-right-carousel': 'sbf-arrow-left-carousel'"></v-icon>
   </v-btn>

   <div class="drawerContent flex-column d-flex justify-space-between align-center">
      <v-card v-if="isShowVideo" class="mt-2 d-flex flex-grow-0 flex-shrink-0 elevation-0" :color="'grey lighten-1'" height="210" width="276"></v-card>
      <v-sheet class="chatContainer d-flex flex-grow-1">
         <chat></chat>
      </v-sheet>
   </div>
   </v-navigation-drawer>
</template>

<script>
import chat from '../../../chat/components/messages.vue';
export default {
   data() {
      return {
        drawerExtend:true, 
      }
   },
   components:{
      chat,
   },
   computed: {
      isShowVideo(){
         return this.$store.getters.getActiveNavEditor !== 'class-screen'
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
         height: ~"calc(100vh - 68px)";
         .chatContainer{
            // background: red;
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