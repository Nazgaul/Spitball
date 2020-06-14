<template>
  <div class="landingPageHP">
      <headlineSection></headlineSection>
      <categoriesSection></categoriesSection>
      <tutorsSection></tutorsSection>
      <itemsSection></itemsSection>
      <testimonialsSection></testimonialsSection>
      <earnSection></earnSection>
  </div>
</template>

<script>
import headlineSection from '../components/learn/headlineSection.vue';
import categoriesSection from '../components/learn/categoriesSection.vue';
import tutorsSection from '../components/learn/tutorsSection.vue';
import itemsSection from '../components/learn/itemsSection.vue';
import testimonialsSection from '../components/learn/testimonialsSection.vue';
import earnSection from '../components/learn/earnSection.vue'

export default {
      components:{
        headlineSection,
        categoriesSection,
        tutorsSection,
        itemsSection,
        testimonialsSection,
        earnSection
    },
};
</script>

<style lang="less">
@import "../styles/mixin.less";
.landingPageHP{
    background-color: white;
}
</style>