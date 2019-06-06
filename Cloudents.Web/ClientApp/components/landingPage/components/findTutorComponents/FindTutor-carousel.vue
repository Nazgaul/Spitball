<template>
  <div class="landing-carousel-compenent-container" justify-center lg12 v-if="cards">

    <v-flex class="landing-carousel-arrows" @click="moveCarouselClick(-1)" :disabled="atHeadOfList">
      <img src="./images/FindTutor_next-btn.png">
    </v-flex>

    <div class="landing-carousel-slider-conteiner">
        <div v-touch:swipe="moveCarousel()" class="landing-carousel-slider"
                  :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">

            <div v-for="(card, index) in cards" :key="index" class="landing-carousel-card">
                <div class="landing-carousel-card-text">
                    <p class="">{{card.text}}</p>
                </div>

                <div class="landing-carousel-card-bottom">
                  <div class="landing-carousel-card-img">
                    <img v-show="card.image" :src="getImgUrl(card.image)" :alt="card.name" class="carouselIMG">
                  </div>
                    <p class="landing-carousel-card-name">{{card.name}}</p>
                    <p class="landing-carousel-card-title">{{card.title}}</p>
                </div>
            </div>

        </div>
    </div>

    <v-flex class="landing-carousel-arrows" @click="moveCarouselClick(1)" :disabled="atEndOfList">
      <img class="rightBtn" src="./images/FindTutor_next-btn.png">
    </v-flex>

  </div>
</template>

<script>
// Gaby - we dont intent to use this component as a reusable one, 
// thus data is imported from the component itself
import { reviews } from "./helpers/testimonials/data/testimonialsData.js";

export default {
  name: "Carousel",
  data() {
    return {
      cards: reviews[0],
      currentOffset: 0,
      windowSize: window.innerWidth > 757 ? 2 : 1,
      paginationFactor: 0
    };
  },
  computed: {
    atEndOfList() {
      return (
        this.currentOffset <=
        this.paginationFactor * -1 * (this.cards.length - this.windowSize)
      );
    },
    atHeadOfList() {
      return this.currentOffset === 0;
    }
  },
  methods: {
    getImgUrl(path) {
      return require(`${path}`);
    },
    moveCarousel() {
      let self = this;
      return function(dir, event) {
        let direction = dir === "left" ? 1 : -1;
        if (direction === 1 && !self.atEndOfList) {
          self.currentOffset -= self.paginationFactor;
        } else if (direction === -1 && !self.atHeadOfList) {
          self.currentOffset += self.paginationFactor;
        }
      };
    },
    moveCarouselClick(direction) {
      if (direction === 1 && !this.atEndOfList) {
        this.currentOffset -= this.paginationFactor;
      } else if (direction === -1 && !this.atHeadOfList) {
        this.currentOffset += this.paginationFactor;
      }
    }
  },
  mounted() {
    const element = document.getElementsByClassName("landing-carousel-card")[0];
    let style = element.currentStyle || window.getComputedStyle(element),
      width = element.offsetWidth,
      margin =
        window.innerWidth > 757
          ? parseFloat(style.marginLeft)
          : parseFloat(style.marginLeft) + parseFloat(style.marginRight);
    this.paginationFactor = width + margin;
  },
  created() {
    console.log(reviews[0])
  },
};
</script>

<style lang="less">
.landing-carousel-compenent-container {
  display: flex;
  align-items: center;
  padding: 142px 0 155px;
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
    .rightBtn {
      transform: scaleX(-1);
    }
  }
  .landing-carousel-slider-conteiner{
    overflow: hidden;
    .landing-carousel-slider{
      display: flex;
      transition: transform 150ms ease-out;
      transform: translatex(0px);
      .landing-carousel-card{  
        min-width: 508px;
        height: 457px;
        display: flex;
        flex-direction: column;
        margin-left: 40px;
        .landing-carousel-card-text{
          border-top-left-radius: 4px;
          border-top-right-radius: 4px;
          background-color: #ffffff;
          height: 265px;
          padding: 34px 42px 0 38px;
            width: 100%;
          p{  
              font-size: 18px;
              color: #505050;
              line-break: auto;
              line-height: 20px;
              overflow: hidden;
              text-overflow: ellipsis;
          }
        }
        .landing-carousel-card-bottom{
          height: 192px;
          border-bottom-left-radius: 4px;
          border-bottom-right-radius: 4px;
          background-color: #e94567;
          text-align: center;
          p{
            padding: 0;
            margin: 0;
            color: #ffffff;
            font-size: 20px;
            &.landing-carousel-card-name{
              font-weight: 600;
              letter-spacing: 1.5px;
              padding-top: 20px;
            }
            &.landing-carousel-card-title{
              padding-top: 5px;
            }
          }
        .landing-carousel-card-img{
              width: 150px;
              height: 150px;
              border-radius: 50%;
              border: solid 5px #ffffff;
              overflow: hidden;
              margin: auto;
              margin-top: -74px;
              img{
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