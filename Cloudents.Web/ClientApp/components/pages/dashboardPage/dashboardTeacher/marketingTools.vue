<template>
    <div class="marketingTools mb-2 mb-sm-4">

        <div class="headerTools pa-0 mb-4 mb-sm-2 d-block d-sm-none">
            <div class="titleTools">{{$t('marketing_title')}}</div>
        </div>

        <div class="wrapper d-flex text-center">
            <div class="sharePost me-2 py-4">
              <shareIcon width="92" />
              <div>Share a social post</div>
              <div>Stand out on Facebook & Instagram</div>
              <v-btn>Share</v-btn>
            </div>

            <div class="createCoupon ms-2 py-4">
              <createCouponIcon width="125" />
              <div>Create a coupon</div>
              <div>Create custom coupons and promotions</div>
              <v-btn>Create</v-btn>
            </div>
        </div>
    </div>
</template>
<script>
import * as dialogNames from '../../global/dialogInjection/dialogNames.js';
import shareIcon from './images/share.svg';
import createCouponIcon from './images/createCoupon.svg';

export default {
  name: "marketingTools",
  components: {
    shareIcon,
    createCouponIcon,
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.marketingTools {
  // padding: 16px;
  // background: white;
  // box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
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

  .wrapper {
    .sharePost, .createCoupon {
      border-radius: 8px;
      flex: 1;
      background: white;
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    }
    .sharePost {}
    .createCoupon {}
  }
  // .mainBlogs {
    
  //   @media (max-width: @screen-xs) {
  //     display: flex;
  //     flex-direction: column;
  //     text-align: center;
  //   }
  //   .leftBlogs {
  //     display: flex;
  //     flex-direction: column;
  //     justify-content: space-around;
  //     @media (max-width: @screen-xs) {
  //       display: block;
  //     }
  //     .titleWrap {
  //       max-width: 360px;
  //       @media (max-width: @screen-xs) {
  //         max-width: 100%;
  //         padding: 0 32px;
  //       }
  //       .title1 {
  //          font-size: 24px;
  //          font-weight: 600;
  //          letter-spacing: normal;
  //          color: #43425d;
  //         @media (max-width: @screen-md) {
  //           font-size: 22px;
  //         }
  //         @media (max-width: @screen-xs) {
  //           font-size: 18px;
  //         }
  //       }
  //       .title {
  //          font-weight: 600;
  //          letter-spacing: normal;
  //          color: #43425d;
  //       }
  //     }
  //     .btn {
  //       text-transform: initial;
  //       font-weight: 600;
  //     }
  //   }
  //   .rightBlogs {
  //     margin: 0 0 0 auto;

  //       @media (max-width: @screen-xs) {
  //         margin: 0 0 12px 0 !important;
  //         order: -1;
  //       }
  //   }
  // }
}
</style>
