<template>
  <v-layout>
    <v-flex class="landing-sub-footer-section-container">
      <h3 v-language:inner="'landingPage_subfooter_title'"></h3>
      <button @click="goToTutorPage" v-language:inner="'landingPage_subfooter_btn'"></button>
    </v-flex>
  </v-layout>
  <!-- <v-layout>
        <v-flex class="centered-vertical-grid">
          <h3 v-language:inner="'landingPage_subfooter_title'"></h3>
          <a href="#" v-language:inner="'landingPage_subfooter_btn'"></a>
        </v-flex>
  </v-layout>-->
</template>

<script>
export default {
  name: "subfooterSection",
  methods:{
        goToTutorPage(){
            this.$router.push({name:"tutorLandingPage"})
        }
    }
};
</script>

<style lang='less'>
@import "../../../../styles/mixin.less";

.landing-sub-footer-section-container {
text-align: center;
  background-color: #facb57;
  padding: 80px 0 108px;
  h3 {
    font-size: 38px;
    font-weight: 700;
    text-align: center;
    color: #21837f;
    padding-bottom: 48px;
      @media (max-width: @screen-sm) {
      padding-bottom: 36px;
      text-align: start;
      font-size: 24px;

    } 
  }
  button{
    padding: 9px 37px 9px 33px;
    border-radius: 28px;
    box-shadow: 0 3px 19px 0 rgba(0, 0, 0, 0.15);
    background-color: #27CCBB;
    font-size: 22px;
    font-weight: 600;
    text-align: center;
    outline: none;
    color: #ffffff;
      @media (max-width: @screen-sm) {
      padding: 12px 14px;
    }
  }
  @media (max-width: @screen-sm) {
      width: 100%;
      padding: 50px 24px 32px 24px;
      text-align: start;
    }
}
</style>
