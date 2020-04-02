<template>
   <router-link class="router-doc" :to="item.url">
      <div class="text-truncate">
         <span>{{item.name}}</span>
      </div>
      <div class="text-truncate" v-if="item.course">
         <span class="font-weight-bold" v-language:inner="'dashboardPage_course'"></span>
         <span>{{item.course}}</span>
      </div>
   </router-link>
</template>

<script>
export default {
   props:{
      item:{
         type: Object,
         required: true
      }
   }
}
</script>
<style lang="less">
   .router-doc{
      color: #43425d !important;
      line-height: 1.6;
   }
</style>
