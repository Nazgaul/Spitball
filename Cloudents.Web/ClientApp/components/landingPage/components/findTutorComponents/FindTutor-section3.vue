<template>
    <v-layout class="landing-section3-container" lg6>
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
.landing-section3-container {
  background-color: #fbfbfb;
  .landing-section3-text-container{
    padding: 170px 170px 0 0;
    h3{
    text-overflow: ellipsis;
    font-size: 45px;
    font-weight: bold;
    line-height: 1.33;
    color: #e94567;
    }
    h4{
      padding-top: 24px;
    text-overflow: ellipsis;
    font-weight: normal;
    line-height: 1.45;
    color: #6c6c6c;
    font-size: 22px;
    }
  }
  .landing-section3-img-container{
    padding: 68px 20px 0 0;
  }
}
</style>
