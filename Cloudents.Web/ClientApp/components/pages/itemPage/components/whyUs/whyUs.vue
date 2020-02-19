<template>
  <div class="whyUs">
      <div class="whyUs__div" v-language:inner="'documentPage_why_spitball'"></div>
      <div class="whyUs__bottom">
          <div class="whyUs__bottom__cont">
              <sheild class="whyUs__bottom__cont__icon"/>
              <span class="whyUs__bottom__cont__span" v-language:inner="'documentPage_money_back'"></span>
          </div>
          <div class="whyUs__bottom__cont">
              <secure class="whyUs__bottom__cont__icon"/>
              <span class="whyUs__bottom__cont__span" v-language:inner="'documentPage_secure_payment'"></span>
          </div>
          <div class="whyUs__bottom__cont">
              <exams class="whyUs__bottom__cont__icon"/>
              <span class="whyUs__bottom__cont__span" v-language:inner="'documentPage_prepared_exams'"></span>
          </div>
      </div>
  </div>
</template>

<script>
import exams from './images/exams.svg';
import secure from './images/secure.svg';
import sheild from './images/sheild.svg';
export default {
    components:{sheild,exams,secure}
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.whyUs{
    width: 100%;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    background-color: #ffffff;
    border-radius: 8px;
    padding: 10px;
    margin: 16px 0;
    @media (max-width: @screen-md) {
        margin-bottom: 38px;
    }
    @media (max-width: @screen-xs) {
        box-shadow: none;
        border-radius: 0;
        padding: 14px 24px 14px 24px;
        margin-bottom: 16px;
    }
    &__div{
        @media (max-width: @screen-xs) {
            font-size: 18px;
            padding-bottom: 16px;
            font-weight: 700;
        }
        padding-bottom:24px;
        font-size: 20px;
        font-weight: 600;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        color: #43425d;
        text-align: center;
    }
    &__bottom{
        color: #43425d;
        display: flex;
        justify-content: space-between;
        text-align: center;
        @media (max-width: @screen-xs) {
            flex-direction: column;
            max-width: 236px;
            margin: 0 auto;
        }

        &__cont{
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            align-items: center;
            @media (max-width: @screen-xs) {
                padding-bottom: 14px;
                justify-content: normal;
                &:last-child{
                    padding-bottom: 0;
                }
            }
            &__icon{
                fill: red !important;
                margin-right: 8px;
                
                @media (max-width: @screen-md) {
                    margin-bottom: 4px;
                }
                @media (max-width: @screen-xs) {
                    margin-bottom: 0;
                }
            }
            &__span{
                font-size: 14px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: 1.4;
                letter-spacing: normal;
                @media (max-width: @screen-md) {
                    margin-bottom: 4px;
                }
                @media (max-width: @screen-xs) {
                    margin-bottom: 0;
                }
            }
        }
    }
}
</style>