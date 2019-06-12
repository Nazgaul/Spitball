<template>
  <v-layout class="landing-section6-container" lg6 xs6>
    <v-flex class="landing-section6-img-container">
      <img src="./images/FindTutor_section6.png" alt>
    </v-flex>
    <v-flex class="landing-section6-text-container">
      <h3 v-language:inner="'landingPage_section6_title'"></h3>
      <h4 v-language:inner="'landingPage_section6_pharagraph'"></h4>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: 'section6'
};
</script>

<style lang='less'>
@import "../../../../styles/mixin.less";

.landing-section6-container {
  background-color: #21837f;
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;
  .landing-section6-img-container {
    display: flex;
    // justify-content: flex-end;
    padding : 60px 0 38px; 

    @media (max-width: @screen-sm) {
      width: 100%;
      justify-content: center;
      padding: 38px 0 24px;
    }
    @media (max-width: @screen-xs) {
      img {
        width: 80%;
        height: 80%;
      }
    }
  }

  .landing-section6-text-container {
    width: 36%;
    @media (max-width: @screen-sm) {
      width: 100%;
      padding-bottom: 32px;
      h4 {
        padding-top: 24px;
      }
    }
    @media (max-width: @screen-sm) {
      width: 100%;
    }
    h3 {
      font-size: 45px;
      padding-bottom: 6px;
      line-height: 1.33;
      color: #facb57;
      @media (max-width: @screen-xs) {
        font-size: 24px;
      }
    }
    h4 {
      font-size: 22px;
      font-weight: normal;
      color: #ffffff;
      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
    }
  }

  @media (max-width: @screen-sm) {
    flex-direction: column;
    margin-top: 0px;
  }
}
</style>
