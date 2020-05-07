<template>
   <v-slide-x-transition>
      <v-card v-if="tutorVideoTrack && isShowFloatingVideo" 
              :id="elementId" 
              height="164" width="164"
              :class="{'footerActive':isFooter}">
      </v-card>
   </v-slide-x-transition>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         elementId: 'floatingVideoContainer',
         tutorVideo: null,
      }
   },
   props:{
      isShowFloatingVideo:{
         type: Boolean
      },
      isFooter:{
         type: Boolean
      }
   },
   watch: {
      isShowFloatingVideo(newVal){
         if(!newVal){
            let localMediaContainer = document.getElementById(this.elementId);
            let videoTag = localMediaContainer.querySelector("video");
            if (videoTag) {localMediaContainer.removeChild(videoTag)} 
         }
         if(newVal && this.tutorVideo){
            let self = this;
            this.$nextTick(()=>{
               let localMediaContainer = document.getElementById(self.elementId);
               let videoTag = localMediaContainer.querySelector("video");
               if (videoTag) {localMediaContainer.removeChild(videoTag)} 
               localMediaContainer.appendChild(self.tutorVideo.attach());
            })
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
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorVideoTrack(){
         return this.getRoomTutorParticipant?.video;
      },

   },

}
</script>
<style lang="less">
   #floatingVideoContainer{
      border-radius: 50%;
      box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.26);
      position: fixed;
      bottom: 64px;
      left: 24px;
      padding: 4px;
      &.footerActive{
         bottom: 190px; // 64px + footer height 124px
      }
      video {
         width: 100%;
         height: 100%;
         object-fit: cover;
         object-position: center;
         background-repeat: no-repeat;
         pointer-events: none;
         border-radius: 50%;
      }
      video::-webkit-media-controls-enclosure {
         display: none !important;
      }
   }
</style>