<template>
   <span class="tableFooter">
      {{$parent.$parent.pagination.pageStop}} <span v-language:inner="'dashboardPage_of'"/> {{$parent.$parent.pagination.itemsLength}}
   </span>
</template>

<style lang="less">
.tableFooter{
   font-size: 14px;
   color: #43425d !important;
}
</style>