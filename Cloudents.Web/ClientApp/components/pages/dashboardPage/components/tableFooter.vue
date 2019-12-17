<template>
   <span class="tableFooter">
   {{item.pageStop}} <span v-language:inner="'dashboardPage_of'"/> {{item.itemsLength}}
   </span>
</template>
<script>
export default {
   props:['item']
}
</script>
<style lang="less">
.tableFooter{
   font-size: 14px;
   color: #43425d !important;
}
</style>