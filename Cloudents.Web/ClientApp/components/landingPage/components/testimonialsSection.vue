<template>
    <div class="testimonialsSection">
        <h1 class="tss-subtitle" v-language:inner="'homePage_tss_subtitle'"/>
        <h1 class="tss-title" v-language:inner="'homePage_tss_title'"/>
        <div class="tss-dvider"/>
        <div class="testimonialsCont" :style="{'pointer-events':$vuetify.breakpoint.smAndDown?'':'none'}">
            <sbCarousel :slideStep="1" :overflow="true" :arrows="false">
                <div class="testimonialCard"  v-for="(item, index) in testimonials" :key="index">
                    <img class="card-t" src="../images/cardT.png" alt="">
                    <div class="testimonialCard-top">
                        <img class="quote-card" src="../images/quote.png" alt="">
                        <p class="testimonialCard-txt">{{item.txt}}</p>
                        <h1 class="t-name">{{item.userName}}</h1>
                    </div>
                    <div class="tutorInfoT">
                        <img class="tutorImg-t" :src="getImg(item.userImg)">
                        <div class="tutorName-t">{{item.tutorName}}</div>
                    </div>
                </div>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import sbCarousel from '../../sbCarousel/sbCarousel.vue';
import { LanguageService } from "../../../services/language/languageService.js";

export default {
    components:{sbCarousel},
    data() {
        return {
            testimonials:[
                {
                    txt:LanguageService.getValueByKey("homePage_ts_a_txt"),
                    userName:LanguageService.getValueByKey("homePage_ts_a_uname"),
                    userImg: './staticCardImgs/ut1.png',
                    tutorName:LanguageService.getValueByKey("homePage_ts_a_tname"),
                },
                {
                    txt:LanguageService.getValueByKey("homePage_ts_b_txt"),
                    userName:LanguageService.getValueByKey("homePage_ts_b_uname"),
                    userImg: './staticCardImgs/ut2.png',
                    tutorName:LanguageService.getValueByKey("homePage_ts_b_tname"),
                },
                {
                    txt:LanguageService.getValueByKey("homePage_ts_c_txt"),
                    userName:LanguageService.getValueByKey("homePage_ts_c_uname"),
                    userImg: './staticCardImgs/ut2.png',
                    tutorName:LanguageService.getValueByKey("homePage_ts_c_tname"),
                },
            ]
        }
    },
  methods: {
    getImg(path) {
      return require(`${path}`);
    }
  },
  computed: {
    isMobile(){
        return this.$vuetify.breakpoint.xsOnly;
    }
  },

}
</script>

<style lang="less">
@import "../../../styles/mixin.less";

    .testimonialsSection{
        width: 100%;
        height: 620px;
        background-color: #f9f9fa;
        .tss-subtitle{
            padding-top: 34px;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-title{
            @media (max-width: @screen-xs) {
                font-size: 24px;
            }
            font-size: 30px;
            font-weight: bold;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-dvider{
            margin: 0 auto;
            margin-top: 16px;
            margin-bottom: 58px;
            width: 111px;
            height: 6px;
            background-color: #4c59ff;
        }
        .testimonialsCont{
            .responsiveLandingPage(928px,80px);
            overflow: inherit;
            display: flex;
            justify-content: center;
            .testimonialCard{
                padding: 0 20px;
                position: relative;
                width: 285px;
                height: 375px;
                border-radius: 8px;
                box-shadow: 0 2px 24px 0 rgba(0, 0, 0, 0.31);
                background-color: #ffffff;
                display: flex;
                flex-direction: column;
                justify-content: space-between;
                .card-t{
                    position: absolute;
                    top: 0;
                    right: -1px;
                }
                .testimonialCard-top{
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    .quote-card{
                        margin-top: 34px;
                    }
                    .testimonialCard-txt{
                        margin: 0;
                        padding: 0;
                        margin-top: 12px;
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 1.75;
                        letter-spacing: normal;
                        text-align: center;
                        color: #43425d;
                    }

                    .t-name{
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 2.51;
                        letter-spacing: normal;
                        color: #848bbc;
                    }
                }
                .tutorInfoT{
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    .tutorImg-t{
                        margin-bottom: 12px;
                    }
                    .tutorName-t{
                        font-size: 16px;
                        font-weight: normal;
                        font-stretch: normal;
                        font-style: normal;
                        line-height: 1.43;
                        letter-spacing: normal;
                        color: #4c59ff;
                        margin-bottom: 34px;
                    }
                }
            }
        }
    }
</style>