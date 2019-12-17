<template>
  <div class="myCalendar">
     <h1>My Calendar</h1>
     <ul class="myCalendar_ul elevation-1">
        <li class="myCalendar_li" v-for="(item, index) in 100" :key="index">
           <v-layout row class="myCalendar_div">
              <v-flex xs4 sm2 class="myCalendar_div_day">Mon Aug 29</v-flex>
              <v-flex xs3 sm2 >8:00am - 8:15am</v-flex>
              <v-flex xs6 sm6>Plan this dayPlan this day Plan this day Plan this day Plan this day</v-flex>
           </v-layout>
        </li>
     </ul>
  </div>
</template>

<script>
export default {

}
</script>

<style lang="less">
.myCalendar{
   
   .myCalendar_ul{
      background: white;
      padding: 0;
      max-height: calc(~"70vh - 100px");
      overflow-y: scroll;
      .myCalendar_li{
         padding: 6px 10px;
         border-bottom: 1px solid #818181;
         text-transform: capitalize;
         .myCalendar_div_day{
            color: rgb(0, 0, 180);
            font-weight: 600;
         }
      }
   }
}
</style>