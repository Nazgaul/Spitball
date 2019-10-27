<template>
    <div>
        <top-nav></top-nav>
        <!-- <router-view/> -->
        <div class="landingPageHP">
            <headlineSection></headlineSection>
            <div class="sectionWrapper">
                <categoriesSection></categoriesSection>
                <tutorsSection></tutorsSection>
                <itemsSection></itemsSection>
            </div>
            <testimonialsSection></testimonialsSection>
            <earnSection></earnSection>
        </div>
        <Footer></Footer>
    </div>
</template>

<script>   
import topNav from '../landingPageTools/TopNav.vue';
import Footer from '../landingPageTools/Footer.vue';

import headlineSection from './components/headlineSection.vue';
import categoriesSection from './components/categoriesSection.vue'
import tutorsSection from './components/tutorsSection.vue';
import itemsSection from './components/itemsSection.vue';
import testimonialsSection from './components/testimonialsSection.vue';
import earnSection from './components/earnSection.vue'

    export default {
        components:{
            topNav,
            Footer,



            headlineSection,
            categoriesSection,
            tutorsSection,
            itemsSection,
            testimonialsSection,
            earnSection
        }, 
}
</script>
<style lang="less">
.landingPageHP{
    background-color: white;
    .sectionWrapper{
        margin: 0 auto;
        width: 100%;
        max-width: calc(~"100% - 546px");
    }
}

</style>