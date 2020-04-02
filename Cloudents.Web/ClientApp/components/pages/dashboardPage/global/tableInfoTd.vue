<template>
   <div>
      <router-link class="tableInfo_router" :to="dynamicRouter(item)">
         <template v-if="item.type === 'Question' || item.type === 'Answer'">
            <div class="text-truncate">
               <span class="font-weight-bold" v-language:inner="'dashboardPage_question'"/>
               <span class="text-truncate">{{item.text}}</span>
            </div>
            <div class="text-truncate" v-if="item.answerText">
               <span class="font-weight-bold" v-language:inner="'dashboardPage_answer'"/>
               <span>{{item.answerText}}</span>
            </div>
         </template>
         <template v-if="item.conversationId">
            <div class="text-truncate">
               <span>{{item.name}}</span>
            </div>
         </template>
      </router-link>
   </div>
</template>
<script>
export default {
   props:{
      item:{
         type:Object,
         required:true
      }
   },
   methods: {
      dynamicRouter(item){
         if(item.type === 'Question' || item.type === 'Answer'){
            return {path:'/question/'+item.id}
         }
         if(item.conversationId){
            return {name: 'profile',params: {id: item.userId, name: item.name}}
         }
         if(item.userId && !item.conversationId && !item.type){
            return {name: 'profile',params: {id: item.userId, name: item.name}}
         }
      },   
   },
   
}
</script>

<style lang="less">
   .tableInfo_router{
      color: #43425d !important;
      line-height: 1.6;
   }
</style>

