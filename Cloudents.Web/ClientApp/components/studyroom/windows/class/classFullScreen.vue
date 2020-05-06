<template>
   <div class="classFullScreen">
      <div class="videoContainer">
         <div id="classFullScreenVideo"></div>
         <div class="videoTools">
            <v-row class="videoBtns" justify="center">
               <v-btn class="controlsBtn" icon>
                  <v-icon size="20" class="ml-1" color="white">sbf-video-camera</v-icon>
               </v-btn>
               <v-btn class="controlsBtn ml-6" icon>
                  <v-icon size="28" color="white">sbf-microphone</v-icon>
               </v-btn>
            </v-row>
            <div class="videoMinimizeBtn">
               <v-icon size="32" color="white">sbf-minis</v-icon>
            </div>
         </div>
      </div>
      <v-footer :height="124" inset app fixed color="#212123" class="classFooter">
         <v-slide-group
            class="pa-0"
            active-class="success"
            show-arrows
            color="#fff"
            prev-icon="sbf-arrow-left-carousel"
            next-icon="sbf-arrow-right-carousel">
            <v-slide-item v-for="participant in $store.getters.getRoomParticipants" :key="participant.id">
               <userPreview :participant="participant" class="ma-2"/>
            </v-slide-item>
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
      }
   },
}
</script>

<style lang="less">
.classFullScreen{
   background-color: #212123;
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
            object-fit: cover;
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
            }
         }
      }
   }
   .classFooter{
      .sbf {
         color: #fff;
      }
   }
}
</style>