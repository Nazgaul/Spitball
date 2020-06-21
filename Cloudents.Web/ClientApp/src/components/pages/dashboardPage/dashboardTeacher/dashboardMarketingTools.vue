<template>
    <div class="marketingTools mb-2 mb-sm-4">

    <marketingAction :resource="resource" />
        <!-- <div class="headerTools pa-0 mb-4 mb-sm-2 d-block d-sm-none">
            <div class="titleTools" v-t="'marketing_title'"></div>
        </div>

        <div class="wrapper d-flex text-center">
            <div class="sharePost me-2 py-4">
              <shareIcon width="92" />
              <div class="text1 mt-4 mb-1" v-t="'marketing_tools_share_title1'"></div>
              <div class="text2 mb-4" v-t="'marketing_tools_share_title2'"></div>
              <v-btn color="#4c59ff" rounded outlined depressed :to="{name: marketingRoute}">
                <span class="btnText" v-t="'marketing_tools_share_btn'"></span>
              </v-btn>
            </div>

            <div class="createCoupon ms-2 py-4">
              <createCouponIcon width="125" />
              <div class="text1 mt-4 mb-1" v-t="'marketing_tools_coupon_title1'"></div>
              <div class="text2 mb-4" v-t="'marketing_tools_coupon_title2'"></div>
              <v-btn color="#4c59ff" rounded outlined depressed :to="{name: marketingRoute, query: {dialog: 'createCoupon'}}">
                <span class="btnText" v-t="'marketing_tools_coupon_btn'"></span>
              </v-btn>
            </div>
        </div> -->
    </div>
</template>
<script>
import * as routeNames from '../../../../routes/routeNames'
import marketingAction from '../../marketing/marketingActions/marketingActions.vue'
// import shareIcon from './images/share.svg';
// import createCouponIcon from './images/createCoupon.svg';

export default {
  name: "marketingTools",
  components: {
    marketingAction,
    // shareIcon,
    // createCouponIcon,
  },
  data() {
    return {
        resource: {
          box1: {
              title1: this.$t('marketing_sharePost_title1'),
              title2: this.$t('marketing_sharePost_title2'),
              image: require('./images/sharePost.png'),
              buttonText: this.$t('marketing_lets_go'),
              route: {name: routeNames.Marketing}
          },
          box2: {
              title1: this.$t('marketing_createOffer_title1'),
              title2: this.$t('marketing_createOffer_title2'),
              image: require('./images/specialOffer.png'),
              buttonText: this.$t('marketing_get_started'),
              // route: {name: routeNames.Marketing, query: {dialog:"createCoupon"}}
              route: {name: routeNames.MyCoupons}
          }
      },
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

  .wrapper {
    color: @global-purple;
    .sharePost, .createCoupon {
      border-radius: 8px;
      flex: 1;
      background: white;
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    }
    .text1 {
      font-weight: 600;
      font-size: 16px;
    }
    .text2 {
      font-size: 12px;
    }
    .btnText {
      font-weight: 600;
    }
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
