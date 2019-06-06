<template>
  <v-layout class="landing-section1-container" lg6>
    <v-flex class="landing-section1-text-container">
      <h3 v-language:inner="'landingPage_section1_title'"></h3>
      <h4 v-language:inner="'landingPage_section1_pharagraph'"></h4>
    </v-flex>
    <v-flex class="landing-section1-img-container">
      <img src="./images/FindTutor_section1.png" alt>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: 'section1'
};
</script>

<style lang="less">
.landing-section1-container {
  .landing-section1-text-container {
    padding: 170px 224px 112px 0;
    h3 {
      font-size: 45px;
      font-weight: bold;
      color: #21837f;
      line-height: 1.31;
      text-overflow: ellipsis;
    }
    h4 {
      padding-top: 5px;
      font-weight: normal;
      font-size: 22px;
      color: #6c6c6c;
    }
  }
  .landing-section1-img-container {
    padding: 68px 20px 0 0;
  }
}
</style>
