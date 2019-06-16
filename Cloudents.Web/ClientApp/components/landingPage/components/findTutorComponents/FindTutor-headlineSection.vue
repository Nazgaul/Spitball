<template>
  <v-layout class="landing-headeline-section-container" justify-space-between row wrap>
    <v-flex class="landing-headeline-section-text-container" xs12 lg6 >
      <h2 v-language:inner="'landingPage_main_title'"></h2>
      <h3 v-language:inner="'landingPage_main_subtitle'"></h3>
      <button @click="goToTutorPage" v-language:inner="'landingPage_main_btn'"></button>
    </v-flex>
    <v-flex class="landing-headeline-section-img-container" hidden-md-and-down lg6>
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
        padding: 98px 84px 130px 258px;
        @media (max-width: 1600px){
            padding: 96px 40px 40px 220px;

        }
        @media (max-width: 1400px){
            padding: 96px 50px 40px 150px;

        }
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
            color: #21837f;
            @media (max-width: 1600px){
                margin: 30px 0 42px 0;
            }
            @media (max-width: @screen-md){
                font-size: 16px;
                font-weight: bolder;
                line-height: 1.56;
                margin: 10px 0 40px;
            }
        }
        button{
            font-size: 18px;
            font-weight: 600;
            color: #fff;
            border-radius: 28px;
            box-shadow: 0 3px 19px 0 rgba(0, 0, 0, 0.15);
            background-color: #27ccbb;
            padding: 12px 20px;
        }
    } 
    .landing-headeline-section-img-container{
        background-image: url('./images/hero-landing-page.jpg');
        background-position: center center;
        background-size: cover;
    }
}
</style>
