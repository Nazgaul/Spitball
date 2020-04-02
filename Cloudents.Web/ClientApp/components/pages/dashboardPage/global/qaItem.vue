<template>
   <router-link class="router-qa" :to="{path:'/question/'+item.id}">
      <div class="text-truncate">
         <span class="font-weight-bold" v-language:inner="'dashboardPage_question'"/>
         <span class="text-truncate">{{item.text}}</span>
      </div>
      <div class="text-truncate" v-if="item.answerText">
         <span class="font-weight-bold" v-language:inner="'dashboardPage_answer'"/>
         <span>{{item.answerText}}</span>
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
   .router-qa{
      color: #43425d !important;
      line-height: 1.6;
   }
</style>
