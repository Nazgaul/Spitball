<template>
  <div class="landing-carousel-compenent-container" justify-center lg12 v-if="cards">
    <v-flex class="landing-carousel-arrows" @click="moveCarouselClick(-1)" :disabled="atHeadOfList">
      <img class="leftButton" :class="{'rtlButton': isRtl}" src="./images/FindTutor_next-btn.png">
    </v-flex>

    <div class="landing-carousel-slider-container">

      <div
        v-touch="{
          left: () => moveCarousel('left'),
          right: () => moveCarousel('right')
        }"
        class="landing-carousel-slider"
        :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}"
      >
        <div v-for="(card, index) in cards" :key="index" class="landing-carousel-card">
          <div class="landing-carousel-card-text">
            <p class>{{card.text}}</p>
          </div>

          <div class="landing-carousel-card-bottom">
            <div class="landing-carousel-card-img">
              <img
                v-show="card.image"
                :src="getImgUrl(card.image)"
                :alt="card.name"
                class="carouselIMG"
              >
            </div>
            <p class="landing-carousel-card-name">{{card.name}}</p>
            <p class="landing-carousel-card-title">{{card.title}}</p>
          </div>
        </div>
      </div>
    </div>

    <v-flex class="landing-carousel-arrows" @click="moveCarouselClick(1)" :disabled="atEndOfList">
      <img class="rightBtn" :class="{'rtlButton': isRtl}" src="./images/FindTutor_next-btn.png">
    </v-flex>
  </div>
</template>

<script>
// Gaby - we dont intent to use this component as a reusable one,
// thus data is imported from the component itself
import { reviews } from "./helpers/testimonials/data/testimonialsData.js";
import { mobileReviews } from "./helpers/testimonials/data/testimonialsData.js";

export default {
  name: "Carousel",
  data() {
    return {
      cards: this.ismobile ? reviews[0] : mobileReviews,
      currentOffset: 0,
      windowSize: window.innerWidth > 757 ? 2 : 1,
      paginationFactor: 0,
      isRtl: global.isRtl
    };
  },
  computed: {
    atEndOfList() {
      if (this.isRtl) {
        return (
          this.currentOffset >=
          this.paginationFactor * 1 * (this.cards.length - this.windowSize)
        );
      } else {
        return (
          this.currentOffset <=
          this.paginationFactor * -1 * (this.cards.length - this.windowSize)
        );
      }
    },
    atHeadOfList() {
      return this.currentOffset === 0;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    }
  },
  methods: {
    getImgUrl(path) {
      return require(`${path}`);
    },
    moveCarousel(dir, event) {      
      let self = this;
        let direction = dir === "left" ? 1 : -1;
        if (self.isRtl) {
          direction = direction*-1;
          if (direction === 1 && !self.atEndOfList) {
            self.currentOffset += self.paginationFactor;
          } else if (direction === -1 && !self.atHeadOfList) {
            self.currentOffset -= self.paginationFactor;
          }
        } else {
          if (direction === 1 && !self.atEndOfList) {
            self.currentOffset -= self.paginationFactor;
          } else if (direction === -1 && !self.atHeadOfList) {
            self.currentOffset += self.paginationFactor;
          }
        }
    },
    moveCarouselClick(direction) {
      if (this.isRtl) {
        if (direction === 1 && !this.atEndOfList) {
          this.currentOffset += this.paginationFactor;
        } else if (direction === -1 && !this.atHeadOfList) {
          this.currentOffset -= this.paginationFactor;
        }
      } else {
        if (direction === 1 && !this.atEndOfList) {
          this.currentOffset -= this.paginationFactor;
        } else if (direction === -1 && !this.atHeadOfList) {
          this.currentOffset += this.paginationFactor;
        }
      }
    }
  },
  mounted() {
    let totalCardWidth = document.querySelector(".landing-carousel-slider-container").offsetWidth / 2;
    if (!this.isMobile) {
      let mobileCard = document.querySelectorAll(".landing-carousel-card");
      let cardWidth = (totalCardWidth * 90) / 100;
      let marginCardLeft = (totalCardWidth * 5) / 100;
      let marginCardRight = (totalCardWidth * 5) / 100;
      mobileCard.forEach(card => {
        card.style.minWidth = `${cardWidth}px`;
        card.style.marginLeft = `${marginCardLeft}px`;
        card.style.marginRight = `${marginCardRight}px`;
      });
    }
    const element = document.getElementsByClassName("landing-carousel-card")[0];
    let margins = (totalCardWidth * 5) / 100;
    let width = element.offsetWidth;
    if (!this.isMobile) {
      this.paginationFactor = width + margins * 2;
    } else {
      let style = element.currentStyle || window.getComputedStyle(element);
      let margin = parseFloat(style.marginLeft) + parseFloat(style.marginRight);
      this.paginationFactor = width + margin;
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.landing-carousel-compenent-container {
  display: flex;
  align-items: center;
  padding: 142px 0 155px;
  @media (max-width: @screen-sm) {
    padding: 0;
  }
  .landing-carousel-arrows {
    flex-basis: 0;
    flex-shrink: 0;
    flex-grow: 0;
    img {
      cursor: pointer;
      width: 27px;
      height: 46px;
      object-fit: contain;
    }
    .leftButton {
      &.rtlButton {
        transform: scaleX(-1);
      }
    }
    .rightBtn {
      transform: scaleX(-1);
      &.rtlButton {
        transform: scaleX(1);
      }
    }
    @media (max-width: @screen-sm) {
      display: none;
    }
  }
  .landing-carousel-slider-container {
    overflow: hidden;
    @media (max-width: @screen-sm) {
      overflow: hidden;
      width: 100%;
      padding: 38px 0 32px;
    }
    .landing-carousel-slider {
      display: flex;
      transition: transform 150ms ease-out;
      transform: translatex(0px);

      .landing-carousel-card {
        height: 457px;
        display: flex;
        flex-direction: column;
        margin-left: 40px;
        @media (max-width: @screen-sm) {
          min-width: 326px;
          margin: 5px;
          height: auto;
        }
        @media (max-width: @screen-xss) {
          min-width: 100%;
        }
        .landing-carousel-card-text {
          border-top-left-radius: 4px;
          border-top-right-radius: 4px;
          background-color: #ffffff;
          height: 265px;
          padding: 34px 42px 0 38px;
          width: 100%;
          @media (max-width: @screen-sm) {
            padding: 24px 20px;
            height: 198px;
          }
          p {
            font-size: 18px;
            color: #505050;
            line-break: auto;
            line-height: 20px;
            overflow: hidden;
            text-overflow: ellipsis;
            @media (max-width: @screen-sm) {
              font-size: 12px;
            }
          }
        }
        .landing-carousel-card-bottom {
          height: 192px;
          border-bottom-left-radius: 4px;
          border-bottom-right-radius: 4px;
          background-color: #e94567;
          text-align: center;
          @media (max-width: @screen-sm) {
            height: 128px;
          }
          p {
            padding: 0;
            margin: 0;
            color: #ffffff;
            font-size: 20px;
            &.landing-carousel-card-name {
              font-weight: 600;
              letter-spacing: 1.5px;
              padding-top: 20px;
              @media (max-width: @screen-sm) {
                font-size: 16px;
              }
            }
            &.landing-carousel-card-title {
              padding-top: 5px;
              @media (max-width: @screen-sm) {
                font-size: 14px;
              }
            }
          }
          .landing-carousel-card-img {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            border: solid 5px #ffffff;
            overflow: hidden;
            margin: auto;
            margin-top: -74px;
            @media (max-width: @screen-sm) {
              width: 85px;
              height: 85px;
              border: solid 3px #ffffff;
              margin-top: -50px;
            }
            img {
              width: 100%;
              height: auto;
            }
          }
        }
      }
    }
  }
}
</style>