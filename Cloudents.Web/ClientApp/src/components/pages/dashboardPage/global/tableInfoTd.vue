<template>
   <div class="tableInfo text-xs-left text-truncate py-2">
      <template v-if="item.type === 'BuyPoints'">
         <div class="text-truncate">
            <span>{{$t('dashboardPage_info_buy_points')}}</span>
         </div>
      </template>
       <!-- :to="dynamicRouter(item)" -->
      <div v-else :class="{'cursor-pointer':item.type == 'Course'}" class="tableInfo_router" @click="item.type == 'Course' ? goToCourse(item) :''">
         <template v-if="item.type === 'TutoringSession'">
            <div class="text-truncate">
               <div v-if="item.roomName" class="text-truncate">
                  <span class="font-weight-bold">{{$t('dashboardPage_room_name')}}</span> 
                  <span>{{item.roomName}}</span>
               </div>
               <span>{{$t('dashboardPage_session',[item.name])}}</span>
               <div class="text-truncate">
                  <span class="font-weight-bold" v-t="'dashboardPage_duration'"/> 
                  <span v-if="item.duration">{{item.duration}}</span>
                  <span v-else v-t="'dashboardPage_session_on'"/>
               </div>
            </div>
         </template>
         <template v-if="item.type === 'Document' || item.type === 'Video'">
            <div class="text-truncate">
               <span>{{item.name}}</span>
            </div>
         </template>
         <template v-if="item.type === 'Course'">
            <div class="text-truncate">
               <span>{{item.name}}</span>
            </div>
         </template>
<!--         <template v-if="item.type === 'Question' || item.type === 'Answer'">-->
<!--            <div class="text-truncate">-->
<!--               <span class="font-weight-bold" v-t="'dashboardPage_question'"/>-->
<!--            </div>-->
<!--               <span class="font-weight-bold" v-t="'dashboardPage_answer'"/>-->
<!--            </div>-->
<!--         </template>-->
         <template v-if="item.conversationId">
            <div class="text-truncate">
               <span>{{item.name}}</span>
            </div>
         </template>
         <div class="text-truncate" v-if="item.course">
            <span class="font-weight-bold" v-t="'dashboardPage_course'"></span>
            <span>{{item.course}}</span>
         </div>
      </div>
   </div>
</template>
<script>
import * as routeNames from '../../../../routes/routeNames.js';
export default {
   props:{
      item:{
         type:Object,
         required:true
      }
   },
   methods: {
      goToCourse(item){
         this.$router.push({
            name: routeNames.CoursePage,
            params: {
               id:item.id,
               name:item.name
            }
         })
      }
   //    dynamicRouter(item){
   //       if(item.url){
   //          return item.url;
   //       }
   //       if(item.type === 'TutoringSession'){
   //          return {name: 'profile',params: {id: item.id, name: item.name}}
   //       }
   //       if(item.conversationId){
   //          return {name: 'profile',params: {id: item.userId, name: item.name}}
   //       }
   //       if(item.userId && !item.conversationId && !item.type){
   //          return {name: 'profile',params: {id: item.userId, name: item.name}}
   //       }
   //    },   
   },
   
}
</script>

<style lang="less">
.tableInfo{
   width: 400px;
   max-width: 400px;
   min-width: 300px;
   .tableInfo_router{
      color: #43425d !important;
      line-height: 1.6;
   }
}
</style>

