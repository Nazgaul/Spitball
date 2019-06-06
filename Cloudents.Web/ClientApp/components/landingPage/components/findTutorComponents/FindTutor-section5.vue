<template>
  <v-layout class="landing-section5-container" lg6>
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
  name: 'section5'
};
</script>

<style lang="less">
.landing-section5-container {
  background-color: #fbfbfb;
  .landing-section5-text-container{
    padding: 200px 180px 182px 0;
    h3{
    text-overflow: ellipsis;
    font-size: 45px;
    font-weight: bold;
    line-height: 1.33;
    color: #21837f;
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
  .landing-section5-img-container{
    padding: 70px 20px 0 0;
  }
}
</style>

