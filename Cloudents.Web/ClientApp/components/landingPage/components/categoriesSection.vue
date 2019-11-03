<template>
  <div class="categoriesSection">
    <h1 class="cs-title" v-language:inner="'homePage_cs_title'" />
    <div class="categories-top">
      <sbCarousel :slideStep="1" :arrows="$vuetify.breakpoint.smAndDown" v-if="!isMobile">
        <router-link
          :to="{ path: '/feed', query: {term: card.name}}"
          v-for="(card, index) in categoriesCards"
          :key="index"
          class="categories-card"
          :class="{'hidden-md-and-down': index+1 == categoriesCards.length}"
          :style="{'backgroundImage': `url(${getImg(card.img)}`}"
        >
          <span class="card-title">{{card.name}}</span>
        </router-link>
      </sbCarousel>
      <router-link to="/" v-else class="categories-chips">
        <span v-for="(card, index) in categoriesCards" :key="index" class="chip-title">{{card.name}}</span>
      </router-link>
    </div>
    <div class="categories-bottom">
      <div class="categories-banner">
        <span class="banner-title" v-language:inner="'homePage_banner_title'" />
        <span class="banner-content">
          <compSVG class="banner-svg" />
          <span v-language:inner="'homePage_banner_content_access'" />
        </span>
        <span class="banner-content">
          <pplSVG class="banner-svg" />
          <span v-language:inner="'homePage_banner_content_find'" />
        </span>
        <span class="banner-content">
          <vidSVG class="banner-svg" />
          <span v-language:inner="'homePage_banner_content_choose'" />
        </span>
      </div>
      <div class="categories-carousel" v-if="!$vuetify.breakpoint.smAndDown">
        <sbCarousel :slideStep="1">
          <router-link
            :to="{ path: '/feed', query: {term: item.name}}"
            class="carousel-card"
            v-for="(item, index) in carouselCards"
            :key="index"
            :style="{'backgroundImage': `url(${getImg(item.img)}`}"
          >
            <span class="card-title">{{item.name}}</span>
          </router-link>
        </sbCarousel>
      </div>
    </div>
  </div>
</template>

<script>
import compSVG from "../images/com.svg";
import pplSVG from "../images/ppl.svg";
import vidSVG from "../images/vid.svg";
import sbCarousel from "../../sbCarousel/sbCarousel.vue";
export default {
  components: {
    compSVG,
    pplSVG,
    vidSVG,
    sbCarousel
  },
  data() {
    return {
      categoriesCards: [
        {
          name: "Algebra",
          img: "./staticCardImgs/alg.png"
        },
        {
          name: "Computer Sciences",
          img: "./staticCardImgs/computer-sciences.png"
        },
        {
          name: "Economics",
          img: "./staticCardImgs/economics.png"
        },
        {
          name: "Statistics",
          img: "./staticCardImgs/statistics.png"
        },
        {
          name: "Psychology",
          img: "./staticCardImgs/psychology.png"
        }
      ],
      carouselCards: [
        {
          name: "Math 5 levels",
          img: "./staticCardImgs/mathC.png"
        },
        { name: "English", img: "./staticCardImgs/englishC.png" },
        { name: "Statistics", img: "./staticCardImgs/statisticsC.png" },
        {
          name: "Math 5 levels",
          img: "./staticCardImgs/mathC.png"
        },
        { name: "English", img: "./staticCardImgs/englishC.png" },
        { name: "Statistics", img: "./staticCardImgs/statisticsC.png" }
      ]
    };
  },
  methods: {
    getImg(path) {
      return require(`${path}`);
    }
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    }
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";

.categoriesSection {
  .responsiveLandingPage(1354px, 80px);
  margin-top: 50px;
  padding-bottom: 74px;
  @media (max-width: @screen-xs) {
    width: 100%;
    padding-bottom: 20px;
    margin-top: 0;
  }
  .cs-title {
    @media (max-width: @screen-xs) {
      margin: 0 auto;
      width: calc(~"100% - 16px");
      padding-bottom: 0;
      font-size: 20px;
    }

    font-size: 24px;
    font-weight: bold;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    color: #43425d;
  }
  .categories-top {
    margin-top: 18px;
    margin-bottom: 80px;
    @media (max-width: @screen-xs) {
      margin-bottom: 20px;
    }
    display: flex;
    justify-content: space-between;
    color: white;
    .categories-card {
      color: white;
      position: relative;
      width: 242px;
      height: 136px;
      background-repeat: no-repeat;
      background-size: cover;
      :last-child {
        margin-right: 0;
      }
      .card-title {
        position: absolute;
        left: 14px;
        top: 8px;
      }
    }
    .categories-chips {
      @media (max-width: @screen-xs) {
        margin: 0 auto;
        width: calc(~"100% - 16px");
      }
      color: #4c59ff;
      display: flex;
      flex-wrap: wrap;
      .chip-title {
        margin-bottom: 14px;
        font-size: 14px;
        font-weight: bold;
        margin-right: 6px;
        padding: 10px;
        border-radius: 20px;
        border: solid 1px #4c59ff;
      }
    }
  }
  .categories-bottom {
    display: flex;
    justify-content: space-between;
    height: 254px;
    @media (max-width: @sbScreen-tablet) {
      background-color: #f9f9fa;
      justify-content: center;
      height: 316px;
    }
    .categories-banner {
      width: 519px;
      background-color: #f9f9fa;
      padding: 12px 18px;
      @media (max-width: @screen-xs) {
        padding: 30px 0;
      }
      .banner-title {
        @media (max-width: @screen-xs) {
          font-size: 24px;
          text-align: center;
        }
        display: block;
        font-size: 26px;
        font-weight: bold;
        color: #43425d;
        margin-bottom: 22px;
      }
      .banner-content {
        @media (max-width: @screen-xs) {
          padding: 0 10px;
          margin-bottom: 10px;
        }
        display: inline-flex;
        font-size: 14px;
        color: #43425d;
        align-items: center;
        :last-child {
          margin-bottom: 0;
        }
        .banner-svg {
          @media (max-width: @screen-xs) {
            margin-right: 10px;
          }
        }
      }
    }
    .categories-carousel {
      width: calc(~"100% - 554px");
      .carousel-card {
        width: 242px;
        position: relative;
        height: 256px;
        background-repeat: no-repeat;
        background-size: cover;
        color: white;
        :last-child {
          margin-right: 0;
        }
        .card-title {
          position: absolute;
          left: 14px;
          top: 8px;
        }
      }
    }
  }
}
</style>