<template functional>
   <td class="tablePreview">
      <router-link :to="props.globalFunctions.router(props.item)" class="tablePreview_img">
         <img width="80" height="80" :src="props.globalFunctions.formatImg(props.item)">
      </router-link>
   </td>  
</template>

<style lang="less">
.tablePreview{
   line-height: 0;
   padding-right: 0 !important;
   width: 104px;
   .tablePreview_img{
      img{
         margin: 10px 0;
         border: 1px solid #d8d8d8;
      }
   }
}
</style>