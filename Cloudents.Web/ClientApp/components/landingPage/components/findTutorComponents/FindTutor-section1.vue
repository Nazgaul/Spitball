<template>
  <v-layout class="landing-section1-container" lg6 xs6>
    <v-flex class="landing-section1-text-container">
      <h3 v-language:inner="'landingPage_section1_title'"></h3>
      <h4 v-language:inner="'landingPage_section1_pharagraph'"></h4>
    </v-flex>
    <v-flex class="landing-section1-img-container">
      <img src="./images/FindTutor_section1.png" alt>
    </v-flex>
    <div class="landing-section1-oval" :class="{'oval-rtl':isRtl}"/>
  </v-layout>
</template>

<script>

export default {
  name: 'section1',
  data(){
    return {
      isRtl: global.isRtl
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-section1-container {
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 68px; 
        @media (max-width: @screen-sm) {
        flex-direction: column-reverse;
      }
  .landing-section1-text-container {
    width: 36%;
      @media (max-width: @screen-sm) {
        width: 100%;
        padding-bottom: 32px;
        h4{
          padding-top: 24px;
        }
      }

    h3 {
      font-size: 45px;
      font-weight: bold;
      color: #21837f;
      line-height: 1.31;
      text-overflow: ellipsis;
       @media (max-width: @screen-xs){
          font-size: 24px;
       }
    }
    h4 {
      padding-top: 5px;
      font-weight: normal;
      font-size: 22px;
      color: #6c6c6c;
             @media (max-width: @screen-xs){
          font-size: 16px;
       }
    }
    @media (max-width: 1600px) {
        width: 50%; 
    }
      @media (max-width: @screen-sm) {
        width: 100%;
      }
  }

  .landing-section1-img-container {
    z-index: 2;
    width: 50%;
    display: flex;
    justify-content: flex-end;
    @media (max-width: @screen-sm) {
    width: 100%;
    justify-content: center;
    padding-bottom: 24px;
    }
    @media (max-width: @screen-xs) {
      img{
        width: 80%;
        height: auto;
      }
    }
  }
  
  .landing-section1-oval{
    position: absolute;
    width: 1430px;
    height: 317px;
    background-image: url("./images/FindTutor_section1-oval.png");
    background-size: 100%;
    bottom: -135px;
    right: 0;
    z-index: 1;
    &.oval-rtl{
      transform: scalex(-1);
    }
      @media (max-width: 1600px) {
        display: none;
       
      }
  }
}
</style>
