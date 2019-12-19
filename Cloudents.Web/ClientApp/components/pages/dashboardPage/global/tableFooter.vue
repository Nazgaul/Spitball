<template>
   <span class="tableFooter">
      {{$parent.pageStop}} <span v-language:inner="'dashboardPage_of'"/> {{$parent.itemsLength}}
   </span>
</template>

<style lang="less">
.tableFooter{
   font-size: 14px;
   color: #43425d !important;
}
</style>