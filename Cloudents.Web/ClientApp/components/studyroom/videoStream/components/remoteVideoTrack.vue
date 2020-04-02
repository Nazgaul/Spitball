<template>
   <div class="remoteVideoStream" :id="track.sb_video_id">
      <v-tooltip bottom>
         <template v-slot:activator="{ on }">
            <v-btn v-on="on" class="fullscreen-btn" icon @click="openFullScreen" color="white">
               <v-icon>sbf-fullscreen</v-icon>
            </v-btn>
         </template>
         <span>{{$t('tutor_tooltip_fullscreen')}}</span>
         <span v-language:inner="isAudioActive ? 'tutor_tooltip_mic_mute':'tutor_tooltip_mic_unmute'"/>
      </v-tooltip>
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
         if (video.webkitRequestFullscreen) {
            video.webkitRequestFullscreen();
            return;
         } 
         if (video.mozRequestFullScreen) {
            video.mozRequestFullScreen();
            return;
         } 
         if (video.msRequestFullscreen) {
            video.msRequestFullscreen();
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
         right: 10px;
         top: 10px;
         background:rgba(0, 0, 0, 0.7);
      }
      video {
         width: 100%;
         background-repeat: no-repeat;
         pointer-events: none;
      }
      video::-webkit-media-controls-enclosure {
         display: none !important;
      }
   }
</style>