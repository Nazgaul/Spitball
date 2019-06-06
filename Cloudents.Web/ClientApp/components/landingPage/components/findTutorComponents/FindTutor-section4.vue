<template>
  <v-layout class="landing-section4-container" lg6>
    <v-flex class="landing-section4-img-container">
      <img src="./images/FindTutor_section4.png" alt>
    </v-flex>
    <v-flex class="landing-section4-text-container">
      <h3 v-language:inner="'landingPage_section4_title'"></h3>
      <h4 v-language:inner="'landingPage_section4_pharagraph'"></h4>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
  name: 'section4'
};
</script>

<style lang='less'>
.landing-section4-container {
  background-color: #ecf5fd;
  .landing-section4-img-container {
    display: flex;
    align-items: center;
  }
  .landing-section4-text-container {
    padding: 168px 0 200px 150px;
    h3 {
      font-size: 45px;
      font-weight: bold;
      color: #13374d;
      padding-bottom: 24px;
    }
    h4 {
      font-size: 22px;
      font-weight: normal;
      color: #6c6c6c;
    }
  }
}
</style>
