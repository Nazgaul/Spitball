<template>
   <div class="text-truncate">
      <span>{{$t('dashboardPage_info_buy_points')}}</span>
   </div>
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
