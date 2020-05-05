<template>
   <div class="classRoom">
      <userPreview v-for="videoTrack in $store.getters.getStudentsVideoTracks" 
                  :key="videoTrack.identity" :videoTrack="videoTrack" class="ma-2 classRoomCards"/>
   </div>
</template>

<script>
import userPreview from '../../layouts/userPreview/userPreview.vue'
export default {
   components:{
      userPreview
   },
}
</script>

<style lang="less">
.classRoom{
   background-color: #212123;
   .classRoomCards{
      float: left;
      width: 236px !important;
      height: 149px !important;
   }
}
</style>