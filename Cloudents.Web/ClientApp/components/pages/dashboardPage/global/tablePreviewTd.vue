<template functional>
   <td class="tablePreview">
      <router-link :to="props.globalFunctions.router(props.item)">
         <span v-if="props.item.online" class="tablePreview_online"></span>
         <img width="80" height="80" :src="props.globalFunctions.formatImg(props.item)" class="tablePreview_img">
      </router-link>
   </td>  
</template>

<style lang="less">
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
   }
}
</style>