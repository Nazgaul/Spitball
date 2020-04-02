<template>
   <div>
      <router-link class="tableInfo_router" :to="dynamicRouter(item)">
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
         if(item.conversationId){
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

