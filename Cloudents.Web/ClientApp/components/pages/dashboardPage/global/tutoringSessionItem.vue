<template>
   <router-link class="router-session" :to="{name: 'profile',params: {id: item.id, name: item.name}}">
      <div class="text-truncate">
         <div v-if="item.roomName" class="text-truncate">
            <span class="font-weight-bold">{{$t('dashboardPage_room_name')}}</span> 
            <span>{{item.roomName}}</span>
         </div>
         <span v-text="$t('dashboardPage_session',[item.name])"/>
         <div class="text-truncate">
            <span class="font-weight-bold" v-language:inner="'dashboardPage_duration'"/> 
            <span v-if="item.duration">{{item.duration}}</span>
            <span v-else v-language:inner="'dashboardPage_session_on'"/>
         </div>
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
   .router-session{
      color: #43425d !important;
      line-height: 1.6;
   }
</style>
