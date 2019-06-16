<template>
  <v-layout class="landing-section4-container" lg6 xs6>
    <v-flex class="landing-section4-img-container">
      <img src="./images/FindTutor_section4.png" alt>
    </v-flex>
    <v-flex class="landing-section4-text-container">
      <h3 v-language:inner="'landingPage_section4_title'" class="pb-4"></h3>
      <h4 v-language:inner="'landingPage_section4_pharagraph'"></h4>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: "section4"
};
</script>

<style lang='less'>
@import "../../../../styles/mixin.less";

.landing-section4-container {
  background-color: #ecf5fd;
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;

  .landing-section4-img-container {
    display: flex;
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

  .landing-section4-text-container {
    padding: 20px 0;
    width: 36%;
    @media (max-width: @screen-sm) {
      width: 100%;
      padding-bottom: 32px;
    }
    @media (max-width: @screen-sm) {
      width: 100%;
    }
    h3 {
      font-size: 45px;
      padding-bottom: 6px;
      line-height: 1.33;
      color: #13374d;
      @media (max-width: @screen-xs) {
        font-size: 24px;
      }
    }
    h4 {
      font-weight: normal;
      color: #6c6c6c;
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
