<template>
  <v-layout class="landing-section5-container" lg6 xs6>
    <v-flex class="landing-section5-text-container">
      <h3 v-language:inner="'landingPage_section5_title'"></h3>
      <h4 v-language:inner="'landingPage_section5_pharagraph'"></h4>
    </v-flex>
    <v-flex class="landing-section5-img-container">
      <img src="./images/FindTutor_section5.png" alt>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: "section5"
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-section5-container {
  background-color: #fbfbfb;
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;
  // margin-top: 68px;
  @media (max-width: @screen-sm) {
    flex-direction: column-reverse;
    margin-top: 0px;
  }
  .landing-section5-text-container {
    width: 36%;
    @media (max-width: @screen-sm) {
      width: 100%;
      padding-bottom: 32px;
      h4 {
        padding-top: 24px;
      }
    }

    h3 {
      text-overflow: ellipsis;
      font-size: 45px;
      font-weight: bold;
      line-height: 1.33;
      color: #21837f;

      @media (max-width: @screen-xs) {
        font-size: 24px;
      }
    }

    h4 {
      padding-top: 24px;
      text-overflow: ellipsis;
      font-weight: normal;
      line-height: 1.45;
      color: #6c6c6c;
      font-size: 22px;
      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
    }
  }

  .landing-section5-img-container {
    z-index: 2;
    width: 50%;
    display: flex;
    justify-content: flex-end;

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
}
</style>

