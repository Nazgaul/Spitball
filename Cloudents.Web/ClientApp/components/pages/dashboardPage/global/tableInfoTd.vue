<template functional>
   <td class="tableInfo text-xs-left text-truncate">
      <router-link class="tableInfo_router" :to="props.globalFunctions.router(props.item)">
         <template v-if="props.item.type === 'TutoringSession'">
            <div class="text-truncate">
               <span v-html="props.globalFunctions.$Ph('dashboardPage_session',props.item.name)"/>
               <div class="text-truncate">
                  <span class="font-weight-bold" v-language:inner="'dashboardPage_duration'"/> 
                  <span v-if="props.item.duration">{{props.item.duration}}</span>
                  <span v-else v-language:inner="'dashboardPage_session_on'"/>
               </div>
            </div>
         </template>
         <template v-if="props.item.type === 'Document' || props.item.type === 'Video'">
            <div class="text-truncate">
               <span>{{props.item.name}}</span>
            </div>
         </template>
         <template v-if="props.item.type === 'Question' || props.item.type === 'Answer'">
            <div class="text-truncate">
               <span class="font-weight-bold" v-language:inner="'dashboardPage_question'"/>
               <span class="text-truncate">{{props.item.text}}</span>
            </div>
            <div class="text-truncate" v-if="props.item.answerText">
               <span class="font-weight-bold" v-language:inner="'dashboardPage_answer'"/>
               <span>{{props.item.answerText}}</span>
            </div>
         </template>
         <template v-if="props.item.conversationId">
            <div class="text-truncate">
               <span>{{props.item.name}}</span>
            </div>
         </template>
         <div class="text-truncate" v-if="props.item.course">
            <span class="font-weight-bold" v-language:inner="'dashboardPage_course'"></span>
            <span>{{props.item.course}}</span>
         </div>
      </router-link>
   </td>
</template>

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

