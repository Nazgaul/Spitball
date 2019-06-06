<template>
  <v-layout class="landing-section6-container" lg6>
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
.landing-section6-container {
  background-color: #21837f;
  .landing-section6-img-container {
    display: flex;
    align-items: center;
  }
  .landing-section6-text-container {
    padding: 214px 0 170px 152px;
    h3 {
      font-size: 45px;
      font-weight: bold;
      line-height: 1.2;
      color: #facb57;
    }
    h4 {
      font-size: 24px;
      color: #ffffff;
      font-weight: normal;
      padding-top: 26px;
    }
  }
}
</style>
