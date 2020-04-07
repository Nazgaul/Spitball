<template>
   <div class="remoteVideoStream" :id="track.sb_video_id">
      <!-- <template v-if="$store.getters.getRoomIsTutor">
         <v-tooltip bottom>
            <template v-slot:activator="{ on }">
               <v-btn v-on="on" class="fullscreen-btn" icon @click="openFullScreen" color="white">
                  <v-icon>sbf-fullscreen</v-icon>
               </v-btn>
            </template>
            <span>{{$t('tutor_tooltip_fullscreen')}}</span>
         </v-tooltip>
      </template> -->
   </div>
</template>

<script>
export default {
   name:'remoteVideo',
   props:{
      track:{
         type: Object,
         required:true
      }
   },
   methods: {
      openFullScreen(){
         var video = document.querySelector(`#${this.track.sb_video_id} video`);
         if (!video) return;
         
         if (video.requestFullscreen) {
            video.requestFullscreen();
            return;
         } 
      }
   },
   mounted() {
      let previewContainer = document.getElementById(this.track.sb_video_id);
      previewContainer.appendChild(this.track.attach());
   }
}
</script>

<style lang="less">
   .remoteVideoStream{
      .fullscreen-btn{
         position: absolute;
         margin: 10px;
         right: 0;
         background:rgba(0, 0, 0, 0.7);
      }
      video {
         width: 100%;
         background-repeat: no-repeat;
         pointer-events: none;
      }
      .fullscreenMode{
         position: fixed;
         top: 0;
         left: 0;
         right: 0;
         bottom: 0;
         width: 100vw;
         object-fit: fill;
         height: 100vh;
         z-index: 20;
      }
      video::-webkit-media-controls-enclosure {
         display: none !important;
      }
   }
</style>