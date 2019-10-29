<template>
  <div class="tutor-carousel-component-container" justify-center xs12 v-if="cards">
    <v-flex class="tutor-carousel-arrows" :class="{'disabled': atHeadOfList}" @click="moveCarouselClick(-1)" :disabled="atHeadOfList">
      <img class="left-img-arrow-btn" :class="{'switchSide':isRtl}" src="./images/arrow.png">
    </v-flex>
  <div v-touch="{
          left: () => moveCarousel('left'),
          right: () => moveCarousel('right')
        }" class="tutor-carousel-slider-wrapper">
     <div class="tutor-carousel-slider-container" :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">
      <div v-for="(card, index) in cards" :key="index" class="tutor-carousel-card">
            <div class="tutor-carousel-card-top pb-4">
              <p>{{card.text}}</p>
              <img :src="getImgUrl(card.image)" v-show="card.image"/>
            </div>
            <div class="tutor-carousel-card-bottom">
              <div class="tutor-carousel-quotes">
                <quotes></quotes>  
              </div>
              <div class="tutor-carousel-rating">
                <span class="pb-2"><v-icon v-for="n in 5" :key="n" class="tutor-page-star">sbf-star-rating-full</v-icon></span>
                <p>{{card.name}}</p>
              </div>
            </div>
      </div>
    </div>
  </div>
   

    <v-flex class="tutor-carousel-arrows" @click="moveCarouselClick(1)" :disabled="atEndOfList">
      <img class="right-img-arrow-btn" :class="{'switchSide':isRtl}" src="./images/arrow.png">
    </v-flex>
  </div>
</template>

<script>
import { mobileReviews, reviews } from "./data/testimonialsData.js";
import quotes from "./images/quote.svg";

export default {
  name: "Carousel",
  components: {
    quotes
  },
  data() {
    return {
      cards: this.ismobile ? reviews[0] : mobileReviews,
      currentOffset: 0,
      windowSize: window.innerWidth > 757 ? 2 : 1,
      paginationFactor: 0,
      isRtl : global.isRtl
    };
  },
  computed: {
    atEndOfList() {     
      if(this.isRtl){
        return (this.currentOffset >= this.paginationFactor * 1 * (this.cards.length - this.windowSize));
      } else{
        return (this.currentOffset <= this.paginationFactor * -1 * (this.cards.length - this.windowSize));
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
    moveCarousel(dir) {
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
      if(this.isRtl) {
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
    },
    getImgUrl(path) {
      return require(`${path}`);
    }
  },
    mounted() { 
    // calculate cards on screen  
    let cardsDisplayed = this.isMobile ? 1 : 2;  
    let totalCardWidth = document.querySelector('.tutor-carousel-slider-wrapper').offsetWidth / cardsDisplayed;    
    let mobileCards = document.querySelectorAll('.tutor-carousel-card');
    let cardWidth = (totalCardWidth * 90) /100;
    let marginCardLeft = (totalCardWidth * 5) /100;
    let marginCardRight = (totalCardWidth * 5) /100;
    mobileCards.forEach((card)=>{
        card.style.minWidth = `${cardWidth}px`;
        card.style.marginLeft = `${marginCardLeft}px`;
        card.style.marginRight = `${marginCardRight}px`;
    })

    //calculate offset to swipe
    const element = mobileCards[0];
    let margins = (totalCardWidth * 5) /100;
    let width = element.offsetWidth;
    if(!this.isMobile){
      this.paginationFactor = width + (margins*2);
    }else{
       let style = element.currentStyle || window.getComputedStyle(element);
       let margin = parseFloat(style.marginLeft) + parseFloat(style.marginRight);
       this.paginationFactor = width + margin;
    }
  },
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.tutor-carousel-component-container {
  display: flex;
  flex-direction: row;
  align-items: center;
  overflow: hidden;
  .tutor-carousel-arrows{
    cursor: pointer;
    &.disabled {opacity: .5;}
    img{
      width:27px; 
      height:46px;
      &.left-img-arrow-btn{
        &.switchSide{
          transform: scaleX(-1);
        }
      }
      &.right-img-arrow-btn{
        transform: scaleX(-1);
        &.switchSide{
          transform: scaleX(1);
        }
      }
    }
    @media (max-width: @screen-sm) {
      display: none;
    }
  }
  .tutor-carousel-slider-wrapper{
    overflow: hidden;
    padding: 40px 0;
    .tutor-carousel-slider-container {
        display: flex;
        flex-direction: row;
        transition: transform 150ms ease-out;

        .tutor-carousel-card {
          background: #fff;
          padding: 20px;
          border: 1px solid #f0f0f7;
          .tutor-carousel-card-top{
            display: flex;
            p {
              padding-right: 50px;
              line-height: 30px;
              @media (max-width: @screen-sm) {
                padding-right: 15px;
              }
              .giveMeEllipsis(3,16)
            }
            img {
              min-width: 82px;
              border-radius: 50%;
              @media (max-width: @screen-sm) {
                width: 86px;
                height: 86px;
              }
            }
          }
          .tutor-carousel-card-bottom{
            display: flex;
            justify-content: space-between;
            align-items: center;
            .tutor-carousel-rating{
              display: flex;
              flex-direction: column;
              align-items: center;
              .tutor-page-star {
                color: #ffca54;
                @media (max-width: @screen-sm) {
                  font-size: 18px;
                }  
              }
              p {
                font-weight: bold;
              }
            }
          }
          @media (max-width: @screen-sm) {
            border-radius: 6px;    
          }  
        }
      }
  }
  
}
</style>