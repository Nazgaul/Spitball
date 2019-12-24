<template functional>
   <span class="tableFooter">
   {{props.item.pageStop}} <span v-language:inner="'dashboardPage_of'"/> {{props.item.itemsLength}}
   </span>
</template>

<style lang="less">
.tableFooter{
   font-size: 14px;
   color: #43425d !important;
}
</style>