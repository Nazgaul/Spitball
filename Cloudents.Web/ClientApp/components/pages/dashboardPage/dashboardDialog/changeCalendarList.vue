<template>
   <div class="changeCalendarList">
      <calendarSelect/>
   </div>
</template>

<script>
import calendarSelect from '../../../calendar/calendarSelect.vue';
export default {
   components:{calendarSelect}

}
</script>

<style lang="less">
.changeCalendarList{
   padding: 0 20px;

}
</style>