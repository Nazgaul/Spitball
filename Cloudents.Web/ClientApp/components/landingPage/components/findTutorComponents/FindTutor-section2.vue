<template>
  <v-layout class="landing-section2-container" lg6 xs6>
    <v-flex class="landing-section2-img-container">
      <img src="./images/FindTutor_section2.png" alt>
    </v-flex>
    <v-flex class="landing-section2-text-container">
      <h3 v-language:inner="'landingPage_section2_title'"></h3>
      <span v-language:inner="'landingPage_section2_pharagraph'"></span>
      <span
        class="landing-section2-text-bold"
        v-language:inner="'landingPage_section2_pharagraph_bold'"
      ></span>
    </v-flex>
    <div class="landing-section2-oval" :class="{'oval-rtl':isRtl}"/>
  </v-layout>
</template>

<script>
export default {
  name: "section2",
  data(){
    return {
      isRtl: global.isRtl
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-section2-container {
  z-index: 3;
  background-color: #21837f;
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;

  .landing-section2-img-container {
    z-index: 2;
    width: 50%;
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

  .landing-section2-text-container {
    width: 36%;
    @media (max-width: @screen-sm) {
      width: 100%;
      padding-bottom: 32px;
      h4 {
        padding-top: 24px;
      }
    }
    @media (max-width: 1600px) {
      width: 50%;
    }
    @media (max-width: @screen-sm) {
      width: 100%;
    }
    h3 {
      padding-bottom: 6px;
      font-size: 45px;
      font-weight: bold;
      line-height: 1.33;
      color: #facb57;
      @media (max-width: @screen-xs) {
        font-size: 24px;
        padding-bottom: 24px;
      }
    }
    span {
      font-size: 22px;
      line-height: 1.45;
      color: #ffffff;
      font-weight: normal;
      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
      &.landing-section2-text-bold {
        font-weight: bolder;
      }
    }
  }
  @media (max-width: @screen-sm) {
    flex-direction: column;
    margin-top: 0px;
  }
  .landing-section2-oval {
    position: absolute;
    width: 1430px;
    height: 317px;
    background-image: url("./images/FindTutor_section2-oval.png");
    background-size: 100%;
    z-index: 1;
    bottom: -175px;
    left: 0;
    &.oval-rtl{
      transform: scalex(-1);
    }
    @media (max-width: 1600px) {
      display: none;
    }
  }
}
</style>
