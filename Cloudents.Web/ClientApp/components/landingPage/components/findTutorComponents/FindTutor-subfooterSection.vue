<template>
  <v-layout>
    <v-flex class="landing-sub-footer-section-container">
      <h3 v-language:inner="'landingPage_subfooter_title'"></h3>
      <button v-language:inner="'landingPage_subfooter_btn'"></button>
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
  name: "subfooterSection"
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
    font-weight: 800;
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
    padding: 9px 37px 14px 33px;
    border-radius: 28px;
    box-shadow: 0 3px 19px 0 rgba(0, 0, 0, 0.15);
    background-color: #21837f;
    font-size: 24px;
    font-weight: 600;
    text-align: center;
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
