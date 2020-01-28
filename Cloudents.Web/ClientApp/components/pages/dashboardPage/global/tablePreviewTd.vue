<template functional>
   <td class="tablePreview">
      <img v-if="props.item.type === 'BuyPoints'" 
      :src="props.item.image" 
      class="tablePreview_img buyPointsLayoutPreview">
         
      <router-link v-else :to="props.globalFunctions.router(props.item)">
         <span v-if="props.item.online" class="tablePreview_online"/>
         <img v-if="props.item.image || props.item.preview || props.item.type == 'Question' || 
         props.item.type == 'Answer'" width="80" height="80" :src="props.globalFunctions.formatImg(props.item)" class="tablePreview_img">
         
         <v-avatar v-else :tile="true" tag="v-avatar" :class="'tablePreview_img tablePreview_no_image userColor' + props.globalFunctions.strToACII(props.item.name)" :style="{width: `80px`, height: `80px`, fontSize: `22px`}">
            <span class="white--text">{{props.item.name.slice(0,2).toUpperCase()}}</span>
        </v-avatar>
      </router-link>
   </td>  
</template>


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