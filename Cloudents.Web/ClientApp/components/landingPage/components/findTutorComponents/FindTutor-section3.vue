<template>
    <v-layout class="landing-section3-container" lg6 xs6>
      <v-flex class="landing-section3-text-container">
          <h3 v-language:inner="'landingPage_section3_title'"></h3>
          <h4 v-language:inner="'landingPage_section3_pharagraph'"></h4>
      </v-flex>
      <v-flex class="landing-section3-img-container">
          <img src="./images/FindTutor_section3.png" alt>
      </v-flex>
    </v-layout>
</template>

<script>
export default {
  name: 'section3'
}
</script>

<style lang="less">
  @import "../../../../styles/mixin.less";

.landing-section3-container {
  z-index: 3;
  background-color: #fbfbfb;
  position: relative;
  display: flex;
  justify-content: space-between;
  align-items: center;
    @media (max-width: @screen-sm) {
        flex-direction: column-reverse;
    }
  .landing-section3-text-container{
    width: 36%;
      @media (max-width: @screen-sm) {
        width: 100%;
        padding-bottom: 32px;
        h4{
          padding-top: 24px;
        }
      }

    h3{
      text-overflow: ellipsis;
      font-size: 42px;
      font-weight: bold;
      line-height: 1.33;
      color: #e94567;
       @media (max-width: @screen-xs){
         font-size: 24px;
       }
    }

    h4{
      padding-top: 24px;
      text-overflow: ellipsis;
      font-weight: normal;
      line-height: 1.45;
      color: #6c6c6c;
      font-size: 22px;
                 @media (max-width: @screen-xs){
                   font-size: 16px;
       }
    }
  }

  .landing-section3-img-container{
    padding : 60px 0 38px; 
    z-index: 2;
    width: 50%;
    display: flex;
    justify-content: flex-end;

    @media (max-width: @screen-sm) {
      width: 100%;
      justify-content: center;
      padding: 38px 0 24px;

    }

    @media (max-width: @screen-xs) {
      img{
        width: 80%;
        height: 80%;
      }
    }
  }
}
</style>
