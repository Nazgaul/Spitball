<template>
  <div class="hd-cont">
  <v-layout
    class="landing-headelien-headlineStatus-container px-2"
    align-center
    justify-space-between
    row
    wrap
  >
    <span><b>233,254</b> <span>Study resources</span></span>
    <!-- <span v-language:inner="'landingPage_main_stats_sources'"></span> -->
    <v-flex class="landing-headelien-headlineStatus-stars">
      <span v-language:inner="'landingPage_main_stats_reviews_presents'"></span>
      <FindTutorStarsRating :stars="5"/>
      <span v-language:inner="'landingPage_main_stats_reviews'"></span>
    </v-flex>
    <span><b>1,450</b> <span>Teachers</span></span>
    <!-- <span v-language:inner="'landingPage_main_stats_courses'"></span> -->
    <span><b>255,345</b> <span>Students</span></span>
    <!-- <span v-language:inner="'landingPage_main_stats_tutors'"></span> -->
  </v-layout>
  </div>
</template>

<script>
import FindTutorStarsRating from "./FindTutor-StarsRating.vue";
export default {
  name: "headlineStatus",
  components: {
    FindTutorStarsRating
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.hd-cont{
  width: 100%;
  height: 62px;
  background-color: rgba(0, 0, 0, 0.6); 
  display: flex;
  .landing-headelien-headlineStatus-container {
      margin: 0 auto;
      max-width: calc(100% - 644px);
    span {
      font-size: 18px;
      font-weight: normal;
      color: white;
    }
    .landing-headelien-headlineStatus-stars {
      display: flex;
      flex-grow: 0;
      flex-shrink: 0;
      flex-basis: auto;
  
      @media (max-width: @screen-sm) {
  
        span {
          display: block;
        }
      }
    }
    @media (max-width: 1400px) {
      padding: 0 150px;
    }
    @media (max-width: @screen-md) {
      padding: 0 100px;
    }
    @media (max-width: @screen-mds) {
      padding: 0 50px;
    }
    @media (max-width: @screen-sm) {
      padding: 0 25px;
            justify-content: center;
      span {
        display: none;
  
      }
    }
  }

}
</style>
