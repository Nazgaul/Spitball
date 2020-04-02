<template>
  <component :is="component" :item="item"></component>
</template>

<script>

const TutoringSession = () => import('./tutoringSessionItem.vue');
const Video = () => import('./docVidItem.vue');
const Document = () => import('./docVidItem.vue');

export default {
   name: 'itemInfoByType',
   components:{
      TutoringSession,
      Document,
      Video
   },
   props:{
      item:{
         type:Object,
         required:true,
      }
   },
   computed: {
      component(){
         return this.item.type
      }
   },
}
</script>