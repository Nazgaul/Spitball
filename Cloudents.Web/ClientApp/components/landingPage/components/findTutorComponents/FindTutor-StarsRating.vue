<template>
  <v-layout class="landing-headelien-headlineStatus-startsrating" align-center row wrap>
    <v-flex class="star" v-for="(star, index) in stars" :key="index"></v-flex>
  </v-layout>
</template>

<script>
export default {
  name: "StarsRating",
  props: {
    stars:{
      type: Number,
      required: true
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-headelien-headlineStatus-startsrating {
    padding: 0 20px 0 9px;
  .star {
    width: 21px;
    height: 21px;
    margin: 0 2px;
    background-image: url("./images/FindTutor_star.png");
    background-size: 21px;
  }
  @media (max-width: @screen-xss) {
    padding: 0 9px;
  }
}
</style>


