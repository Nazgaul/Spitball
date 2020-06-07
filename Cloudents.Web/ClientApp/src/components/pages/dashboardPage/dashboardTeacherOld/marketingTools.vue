<template>
    <div class="marketingTools mb-2 mb-sm-4 pb-0">

        <div class="headerTools pa-0 mb-4 mb-sm-2">
            <div class="titleTools">{{$t('marketing_title')}}</div>
        </div>

        <div class="mainBlogs d-flex-column d-sm-flex">
            <div class="leftBlogs mr-sm-2 mr-0 pb-4 pb-sm-0 mb-sm-1 justify-space-around">
                <div class="titleWrap">
                    <div class="title1 mb-2">
                        {{$t('marketing_tools_title1')}}
                    </div>
                    <div class="title2 mb-sm-0 mb-4">
                        {{$t('marketing_tools_title2')}}
                    </div>
                </div>
                <v-btn sel="marketing_btn" :to="{name: 'marketing'}" class="btn" rounded outlined color="#4c59ff" width="120">{{$t('marketing_lets_go')}}</v-btn>
            </div>

            <div class="rightBlogs d-flex-column d-sm-flex pa-0 mr-md-8">
                <img :src="marketingImage" alt="">
            </div>
        </div>
    </div>
</template>
<script>
import * as dialogNames from '../../global/dialogInjection/dialogNames.js';

export default {
  name: "marketingTools",
  data() {
    return {
      createCouponDialog: dialogNames.CreateCoupon
    }
  },
  computed: {
    marketingImage() {
      return this.$vuetify.breakpoint.xsOnly ? require('./images/marketingSmall.png') : require('./images/marketing.png');
    }
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.marketingTools {
  padding: 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  @media (max-width: @screen-xs) {
    box-shadow: none;
    border-radius: 0;
  }
  .headerTools {
      .titleTools {
        color: @global-purple;
        font-weight: 600;
        font-size: 16px;
      }
      .seeAllBlog {
        color: #4c59ff;
      }
   }
  .mainBlogs {
    
    @media (max-width: @screen-xs) {
      display: flex;
      flex-direction: column;
      text-align: center;
    }
    .leftBlogs {
      display: flex;
      flex-direction: column;
      justify-content: space-around;
      @media (max-width: @screen-xs) {
        display: block;
      }
      .titleWrap {
        max-width: 360px;
        @media (max-width: @screen-xs) {
          max-width: 100%;
          padding: 0 32px;
        }
        .title1 {
           font-size: 24px;
           font-weight: 600;
           letter-spacing: normal;
           color: #43425d;
          @media (max-width: @screen-md) {
            font-size: 22px;
          }
          @media (max-width: @screen-xs) {
            font-size: 18px;
          }
        }
        .title {
           font-weight: 600;
           letter-spacing: normal;
           color: #43425d;
        }
      }
      .btn {
        text-transform: initial;
        font-weight: 600;
      }
    }
    .rightBlogs {
      margin: 0 0 0 auto;

        @media (max-width: @screen-xs) {
          margin: 0 0 12px 0 !important;
          order: -1;
        }
    }
  }
}
</style>
