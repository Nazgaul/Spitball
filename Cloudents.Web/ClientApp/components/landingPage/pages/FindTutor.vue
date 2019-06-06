<template>
  <v-container class="landing-page-container">
    <FindTutorHeadlineSection/>
    <FindTutorHeadlineStatus/>
    <component class="component-wrapper" v-for="index in 6" :is="`FindTutor-section${index++}`" :key="index"></component>
    <FindTutorCarouselSection class="component-wrapper"/>
    <FindTutorSubfooterSection/>
    <!-- <div class="light-thin-grid">
            <div class="container">
                <div class="space-around-box">
                    <ExtraLinks :links="extraLinks"></ExtraLinks>
                </div>
            </div>
    </div>-->
    <!-- <v-layout>
        <v-flex class="centered-vertical-grid">
          <h3 v-language:inner="'landingPage_subfooter_title'"></h3>
          <a href="#" v-language:inner="'landingPage_subfooter_btn'"></a>
        </v-flex>
    </v-layout> -->
  </v-container>
</template>

<script>
import FindTutorHeadlineSection from "../components/findTutorComponents/FindTutor-headlineSection.vue"
import FindTutorHeadlineStatus from "../components/findTutorComponents/FindTutor-headlineStatus.vue"
import FindTutorSection1 from "../components/findTutorComponents/FindTutor-section1.vue"
import FindTutorSection2 from "../components/findTutorComponents/FindTutor-section2.vue"
import FindTutorSection3 from "../components/findTutorComponents/FindTutor-section3.vue"
import FindTutorSection4 from "../components/findTutorComponents/FindTutor-section4.vue"
import FindTutorSection5 from "../components/findTutorComponents/FindTutor-section5.vue"
import FindTutorSection6 from "../components/findTutorComponents/FindTutor-section6.vue"
import FindTutorCarouselSection from "../components/findTutorComponents/FindTutor-carouselSection.vue"
import FindTutorSubfooterSection from '../components/findTutorComponents/FindTutor-subfooterSection.vue'

export default {
  components: {
    FindTutorHeadlineSection,
    FindTutorHeadlineStatus,
    FindTutorSection1,
    FindTutorSection2,
    FindTutorSection3,
    FindTutorSection4,
    FindTutorSection5,
    FindTutorSection6,
    FindTutorCarouselSection,
    FindTutorSubfooterSection,
  },
  name: "home",
};
</script>

<style lang="less">
.landing-page-container{
  max-width:100%;
  padding: 0;
  .component-wrapper{
    padding: 0 370px;
  }
}
</style>