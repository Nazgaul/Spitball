<template>
   <div class="remoteVideoStream" :id="track.sb_video_id">
      <v-btn absolute color="info">full</v-btn>
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
      
   },
   mounted() {
      let previewContainer = document.getElementById(this.track.sb_video_id);
      previewContainer.appendChild(this.track.attach());
   }
}
</script>

<style lang="less">
   .remoteVideoStream{
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