<template>
  <v-layout class="landing-headeline-section-container" justify-space-between row wrap>
    <v-flex class="landing-headeline-section-text-container" xs12 lg6 >
      <h2 v-language:inner="'landingPage_main_title'"></h2>
      <h3 v-language:inner="'landingPage_main_subtitle'"></h3>
      <button @click="goToTutorPage" v-language:inner="'landingPage_main_btn'"></button>
    </v-flex>
    <v-flex class="landing-headeline-section-img-container" hidden-md-and-down lg6>
        <img src="./images/FindTutor_headeline.png" alt>
    </v-flex>
  </v-layout>
</template>

<script>
export default {
    name: 'headelineSection',
    methods:{
        goToTutorPage(){
            this.$router.push({
                path:"/tutor",
            })
        }
    }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-headeline-section-container{
    .landing-headeline-section-text-container{
        background-color: #facb57;
        padding: 96px 84px 0 258px;
        @media (max-width: @screen-md) {
            padding: 55px 20px 40px 20px;
        }
        h2{
            font-size: 50px;
            font-weight: bold;
            letter-spacing: -0.5px;
            color:#13374d;
            @media (max-width: @screen-md){
                font-size: 35px;
                font-weight: bolder;
            }
        }
        h3{
            margin: 70px 0 42px 0;
            font-size: 28px;
            font-weight: bold;
            color: #fff;
            @media (max-width: @screen-md){
                font-size: 16px;
                font-weight: bolder;
                line-height: 1.56;
                margin: 10px 0 40px;
            }
        }
        button{
            font-size: 24px;
            font-weight: 600;
            color:#fff;
            border-radius: 28px;
            box-shadow: 0 3px 19px 0 rgba(0, 0, 0, 0.15);
            background-color: #13374d;
            padding:10px 38px;
        }
    } 
    .landing-headeline-section-img-container{
        img{
            height:100%;
        }
    }
}
</style>
