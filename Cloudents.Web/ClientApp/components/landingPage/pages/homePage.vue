<template>
  <div class="landingPageHP">
      <!-- <homePageVideo></homePageVideo> -->
      <!-- <headlineSection></headlineSection>
      <categoriesSection></categoriesSection>
      <tutorsSection></tutorsSection>
      <itemsSection></itemsSection>
      <testimonialsSection></testimonialsSection>
      <earnSection></earnSection> -->


      <div class="homeVideo">
        <video ref="video" class="video" height="636" data-object-position="50% 10%" autoplay loop data-object-fit="cover">
          <source src="https://az32006.vo.msecnd.net/spitball-user/Spitball-HP-video.mp4" type="video/mp4">
        </video>
      </div>





  </div>
</template>

<script>
// import headlineSection from '../components/headlineSection.vue';
// import categoriesSection from '../components/categoriesSection.vue';
// import tutorsSection from '../components/tutorsSection.vue';
// import itemsSection from '../components/itemsSection.vue';
// import testimonialsSection from '../components/testimonialsSection.vue';
// import earnSection from '../components/earnSection.vue'

// import homePageVideo from 

export default {
      components:{
        // headlineSection,
        // categoriesSection,
        // tutorsSection,
        // itemsSection,
        // testimonialsSection,
        // earnSection
    },
    mounted() {
      let self = this  
      this.$nextTick(() => {
        self.$refs.video.play().then(res => {
        debugger
          console.log(res);
          
        }).catch(err => { console.log(err) })
      })
    }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.landingPageHP{
    background-color: rgb(59, 59, 59);
    .homeVideo {
      height: 636px;
      background-image: url('../images/Spitball_original_homepage_poster.jpg');
      .video {
        width: 100%;
        object-fit: cover;
        flex: 1;
        min-height: 100%;
        min-width: 100%;

      }
    }
}
</style>