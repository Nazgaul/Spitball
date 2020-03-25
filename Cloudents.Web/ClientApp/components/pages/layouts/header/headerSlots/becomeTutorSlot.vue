<template>
   <a target="_blank" :href="urlLink" class="becomeTutorSlot mr-3 mr-sm-7" v-t="'profile_become_title'"/>
</template>

<script>
export default {
   name:'becomeTutorSlot',
   data() {
      return {
         urlLink: global.isRtl? 'https://www.teach.spitball.co/?lang=he' :'https://www.teach.spitball.co',
      }
   },
}
</script>

<style lang="less">
a{
   &.becomeTutorSlot{
      font-size: 16px;
      font-weight: 600;
      color: #43425d;
   }
}
</style>