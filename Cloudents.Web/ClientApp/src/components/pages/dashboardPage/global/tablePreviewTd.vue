<template>
   <td class="tablePreview">
      <img v-if="item.type === 'BuyPoints'" :src="item.image" class="tablePreview_img buyPointsLayoutPreview">
 
      <router-link v-else :to="dynamicRouter(item)">
         <span v-if="item.online" class="tablePreview_online"/>
         <img v-if="item.image || item.preview || item.type == 'Question' || 
         item.type == 'Answer'" width="80" height="80" :src="formatImg(item)" class="tablePreview_img">
         
         <v-avatar v-else :tile="true" tag="v-avatar" :class="'tablePreview_img tablePreview_no_image userColor' + strToACII(item.name)" :style="{width: `80px`, height: `80px`, fontSize: `22px`}">
            <span class="white--text">{{item.name.slice(0,2).toUpperCase()}}</span>
        </v-avatar>
      </router-link>
   </td>  
</template>

<script>
export default {
   props:{
      item:{
         type:Object,
         required: true
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
      strToACII(name) {
         let sum = 0;
         for (let i in name) {
            sum += name.charCodeAt(i);
         }
         return sum % 11
      },
      formatImg(item){
         if(item.preview){
            return this.$proccessImageUrl(item.preview,80,80)
         }
         if(item.image){
            return this.$proccessImageUrl(item.image,80,80)
         }
         if(item.type === 'Question' || item.type === 'Answer'){
            return require('./images/qs.png') 
         }
      }
   },
}
</script>


<style lang="less">
@import '../../../helpers/UserAvatar/UserAvatar.less';
.tablePreview{
   line-height: 0;
   padding-right: 0 !important;
   width: 104px;
   position: relative;
   .tablePreview_online{
      position: absolute;
      border-radius: 50%;
      width: 10px;
      height: 10px;
      background-color: #00ff14;
      top: 16px;
      left: 28px;
   }
   .tablePreview_img{
      margin: 10px 0;
      border: 1px solid #d8d8d8;
      &.buyPointsLayoutPreview{
         width: 100%;
         object-fit: cover;
         height: 80px;      
      }
   }
   .tablePreview_no_image {
      position: unset;
      border-radius: 4px;
      font-size: 24px;
   }
}
</style>