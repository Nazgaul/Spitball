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
import {mapGetters} from 'vuex';
import FindTutorHeadlineSection from "../components/findTutorComponents/FindTutor-headlineSection.vue";
import FindTutorHeadlineStatus from "../components/findTutorComponents/FindTutor-headlineStatus.vue";
import FindTutorSection1 from "../components/findTutorComponents/FindTutor-section1.vue";
import FindTutorSection2 from "../components/findTutorComponents/FindTutor-section2.vue";
import FindTutorSection3 from "../components/findTutorComponents/FindTutor-section3.vue";
import FindTutorSection4 from "../components/findTutorComponents/FindTutor-section4.vue";
import FindTutorSection5 from "../components/findTutorComponents/FindTutor-section5.vue";
import FindTutorSection6 from "../components/findTutorComponents/FindTutor-section6.vue";
import FindTutorCarouselSection from "../components/findTutorComponents/FindTutor-carouselSection.vue";
import FindTutorSubfooterSection from '../components/findTutorComponents/FindTutor-subfooterSection.vue';
import store from '../../../store/index';

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
    beforeRouteEnter (to, from, next) {
      let isLogoClicked = from.name !== null; 
      //makes sure an auth user won't see this page!
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
.landing-page-container{
  transition: 0.3s;
  max-width:100%;
  padding: 0;
  .component-wrapper{
    padding: 0 370px;
    .responsive-property(min-height, 750px, null, auto);
      @media (max-width: 1800px) {
        padding: 0 300px;
      }
      @media (max-width: 1700px) {
        padding: 0 200px;
      }
      @media (max-width: 1400px) {
        padding: 0 150px;
      }
      @media (max-width: @screen-md) {
        padding: 0 100px;
        min-height: auto;
      }
      @media (max-width: @screen-mds) {
        padding: 0 50px;
      }
      @media (max-width: @screen-sm) {
        padding: 0 38px; ;
      }
      @media (max-width: @screen-xss) {
        padding: 0 25px; ;
      }
    }
    h4 {
      text-align: justify;
    }
    h4:lang(en) {
      font-size: 20px;
    }
    h4:lang(he) {
      font-size: 22px;
    }
}
</style>