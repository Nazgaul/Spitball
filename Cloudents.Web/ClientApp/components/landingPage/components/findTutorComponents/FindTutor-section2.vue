<template>
  <v-layout class="landing-section2-container" lg6>
    <v-flex class="landing-section2-img-container">
      <img src="./images/FindTutor_section2.png" alt>
    </v-flex>
    <v-flex class="landing-section2-text-container">
      <h3 v-language:inner="'landingPage_section2_title'"></h3>
        <span v-language:inner="'landingPage_section2_pharagraph'"></span>
        <span class="landing-section2-text-bold" v-language:inner="'landingPage_section2_pharagraph_bold'"></span>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: 'section2'
};
</script>

<style lang="less">
.landing-section2-container {
  background-color: #21837f;
  .landing-section2-img-container {
    padding: 45px 0 0 0;
  }
  .landing-section2-text-container {
    padding: 112px 0 0 240px;
    h3 {
      padding-bottom: 6px; 
      font-size: 45px;
      font-weight: bold;
      line-height: 1.33;
      color: #facb57;
    }
    span {
      font-size: 22px;
      line-height: 1.45;
      color: #ffffff;
      font-weight: normal;
      &.landing-section2-text-bold{
        font-weight: bolder;
      }
    }
  }
}
</style>
