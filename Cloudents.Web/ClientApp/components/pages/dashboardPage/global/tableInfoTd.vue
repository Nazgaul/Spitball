<template>
   <div class="tableInfo text-xs-left text-truncate">
      <template v-if="item.type === 'BuyPoints'">
         <div class="text-truncate">
            <span>{{$t('dashboardPage_info_buy_points')}}</span>
         </div>
      </template>
      <router-link v-else class="tableInfo_router" :to="dynamicRouter(item)">
         <template v-if="item.type === 'TutoringSession'">
            <div class="text-truncate">
               <span v-text="$Ph('dashboardPage_session',item.name)"/>
               <div class="text-truncate">
                  <span class="font-weight-bold" v-language:inner="'dashboardPage_duration'"/> 
                  <span v-if="item.duration">{{item.duration}}</span>
                  <span v-else v-language:inner="'dashboardPage_session_on'"/>
               </div>
            </div>
         </template>
         <template v-if="item.type === 'Document' || item.type === 'Video'">
            <div class="text-truncate">
               <span>{{item.name}}</span>
            </div>
         </template>
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
         <div class="text-truncate" v-if="item.course">
            <span class="font-weight-bold" v-language:inner="'dashboardPage_course'"></span>
            <span>{{item.course}}</span>
         </div>
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
         if(item.url){
            return item.url;
         }
         if(item.type === 'Question' || item.type === 'Answer'){
            return {path:'/question/'+item.id}
         }
         if(item.type === 'TutoringSession'){
            return {name: 'profile',params: {id: item.id, name: item.name}}
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
.tableInfo{
   // width: 400px;
   // max-width: 400px;
   // min-width: 300px;
   .tableInfo_router{
      color: #43425d !important;
      line-height: 1.6;
   }
}
</style>

