<template>
   <div class="v-alert info tableEmptyState">
      <img src="../images/warning.png" alt="">
      <span class="tableEmptyState_text" v-language:inner="'dashboardPage_empty'"></span>
   </div>
</template>
<style lang="less">
.tableEmptyState{
   display: flex;
   justify-content: center;
   align-items: center;
   padding: 20px;
   .tableEmptyState_text{
      padding-left: 12px;
      color: white;
      font-size: 14px;
   }
}
</style>