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
import headlineSection from '../components/headlineSection.vue';
import categoriesSection from '../components/categoriesSection.vue';
import tutorsSection from '../components/tutorsSection.vue';
import itemsSection from '../components/itemsSection.vue';
import testimonialsSection from '../components/testimonialsSection.vue';
import earnSection from '../components/earnSection.vue'

export default {
      components:{
        headlineSection,
        categoriesSection,
        tutorsSection,
        itemsSection,
        testimonialsSection,
        earnSection
    },
    beforeRouteEnter (to, from, next) {
      let isLogoClicked = from.name !== null; 
        let isLoggedIn = global.isAuth;
        let query = location.search;
        if(!!isLoggedIn){
          let forceReload = '';
          if(from.path === `/feed` && from.fullPath === '/feed'){
            forceReload = '?reloaded='
          }
          let nextRout = isLogoClicked ? `/feed${forceReload}` : `/feed${query}`;
          next(nextRout);
        }else{
            next();
        }
    }, 
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.landingPageHP{
    background-color: white;
}
</style>