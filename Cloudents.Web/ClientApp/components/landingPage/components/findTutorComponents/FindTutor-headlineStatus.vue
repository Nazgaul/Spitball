<template>
  <v-layout
    class="landing-headelien-headlineStatus-container"
    align-center
    justify-space-between
    row
    wrap
  >
    <span v-language:inner="'landingPage_main_stats_sources'"></span>
    <v-flex class="landing-headelien-headlineStatus-stars">
      <span v-language:inner="'landingPage_main_stats_reviews_presents'"></span>
      <FindTutorStarsRating :stars="5"/>
      <span v-language:inner="'landingPage_main_stats_reviews'"></span>
    </v-flex>
    <span v-language:inner="'landingPage_main_stats_courses'"></span>
    <span v-language:inner="'landingPage_main_stats_tutors'"></span>
  </v-layout>
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

.landing-headelien-headlineStatus-container {
  height: 120px;
  padding: 0 258px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
  background-color: #ffffff;
  span {
    font-size: 22px;
    font-weight: 600;
    color: rgba(0, 0, 0, 0.87);
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
</style>
