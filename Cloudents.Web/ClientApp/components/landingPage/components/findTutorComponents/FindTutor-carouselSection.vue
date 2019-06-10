<template>
    <div class="landing-carousel-section-container" lg12>
          <FindTutor-carousel/>
    </div>
</template>

<script>
import FindTutorCarousel from "./FindTutor-carousel.vue";

export default {
  name: 'carouselSection',
  components:{
    FindTutorCarousel
  }
};
</script>

<style lang='less'>
.landing-carousel-section-container {
  background-color: #fbfbfb;
  height: auto;
}
</style>
