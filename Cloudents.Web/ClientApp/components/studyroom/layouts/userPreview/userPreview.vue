<template>
   <v-card :id="videoTrack.sb_video_id" class="userPreview" height="100" width="154">
      <span class="name">{{userName}}</span>
      <div class="linear"></div>
      <div class="linear2"></div>
   </v-card>  
</template>

<script>
export default {
   props:{
      videoTrack:{
         type: Object,
         required:true
      }
   },
   computed: {
      userName(){
         return this.videoTrack.identity.split('_')[1];
      }
   },
   mounted() {
      let previewContainer = document.getElementById(this.videoTrack.sb_video_id);
      previewContainer.appendChild(this.videoTrack.attach());
   },
   // destroyed() {
   //    this.videoTrack.detach().forEach((detachedElement) => {
   //       detachedElement.remove();
   //    });
   // },
}
</script>

<style lang="less">
.userPreview{
   border-radius: 6px !important;
   .name{
      margin-left: 8px;
      font-size: 14px;
      font-weight: 600;
      color: #ffffff;
      position: absolute;
      z-index: 1;
   }
   .linear{
      position: absolute;
      width: 100%;
      height: 100%;
      background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 55%, rgba(0, 0, 0, 0.1) 74%, rgba(0, 0, 0, 0.64));
      border-radius: 6px;
   }
   .linear2{
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
      border-radius: 6px !important;
   }
}
</style>