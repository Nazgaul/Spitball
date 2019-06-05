<template>
    <div class="testimonials-warraper">
        <div class="carousel-arrow" @click="moveCarouselClick(-1)" :disabled="atHeadOfList">
            <img src="../assets/images/next-btn.png" alt="">     
        </div>
        <div class="testimonials-box">
            <!-- TODO v-touch is deprecated remove it! -->
            <div v-touch:swipe="moveCarousel()" class="testimonial-slider"
                     :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">
                <div v-for="(card, index) in cards" :key="index" class="testimonial">
                    <div class="testimonial-text">
                        <p>{{card.text}}</p>
                    </div>
                    <div class="tesimonials-details-box">
                        <div class="testimonial-thumb">
                            <img v-show="card.image" :src="getImgUrl(card.image)" :alt="card.name" class="carouselIMG">
                        </div>
                        <div class="testimonial-name">{{card.name}}</div>
                        <div class="testimonial-job-title">{{card.title}}</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="carousel-arrow prev" @click="moveCarouselClick(1)" :disabled="atEndOfList">
            <img src="../assets/images/next-btn.png" alt="" style="{transform: scaleX(-1)}">
        </div>
    </div>
</template>

<script>
    export default {
        name: "Carousel",
        props: ['cards'],
        data() {
            return {
                currentOffset: 0,
                windowSize: window.innerWidth > 757 ? 2 : 1,
                paginationFactor: 0,
            }
        },
        computed: {
            atEndOfList() {
                return this.currentOffset <= (this.paginationFactor * -1) * (this.cards.length - this.windowSize);
            },
            atHeadOfList() {
                return this.currentOffset === 0;
            },
        },
        methods: {
            getImgUrl(path) {
                return require(`${path}`)
            },
            moveCarousel() {
                let self = this;
                return function(dir, event){
                    let direction = dir === 'left' ? 1 : -1;
                    if (direction === 1 && !self.atEndOfList) {
                        self.currentOffset -= self.paginationFactor;
                    } else if (direction === -1 && !self.atHeadOfList) {
                        self.currentOffset += self.paginationFactor;
                    }
                }
            },
            moveCarouselClick(direction) {
                if (direction === 1 && !this.atEndOfList) {
                    this.currentOffset -= this.paginationFactor;
                } else if (direction === -1 && !this.atHeadOfList) {
                    this.currentOffset += this.paginationFactor;
                }
            },
        },
        mounted() {
            const element = document.getElementsByClassName('testimonial')[0];
             let style = element.currentStyle || window.getComputedStyle(element),
                width = element.offsetWidth,
                margin = window.innerWidth > 757 ? parseFloat(style.marginLeft)
                    : parseFloat(style.marginLeft) + parseFloat(style.marginRight);
            this.paginationFactor = width + margin;
        }
    }
</script>

<style>

</style>