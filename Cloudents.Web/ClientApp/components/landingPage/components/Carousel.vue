<template>
    <div class="testimonials-warraper">
        <div class="carousel-arrow" @click="moveCarousel(-1)" :disabled="atHeadOfList">
            <img src="/assets/prev-btn.svg" alt="">
        </div>
        <div class="testimonials-box">
            <!-- TODO v-touch is deprecated remove it! -->
            <!-- <v-touch @swipeleft="moveCarousel(1)" @swiperight="moveCarousel(-1)" class="testimonial-slider"
                     :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">
                <div v-for="card in cards" class="testimonial">
                    <div class="testimonial-text">
                        <p>{{card.content}}</p>
                    </div>
                    <div class="tesimonials-details-box">
                        <div class="testimonial-thumb">
                            <img v-show="card.src" :src="card.src" :alt="card.name">
                        </div>
                        <div class="testimonial-name">{{card.name}}</div>
                        <div class="testimonial-job-title">{{card.jobTitle}}</div>
                    </div>
                </div>
            </v-touch> -->
        </div>
        <div class="carousel-arrow" @click="moveCarousel(1)" :disabled="atEndOfList">
            <img src="/assets/next-btn.svg" alt="">
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
            moveCarousel(direction) {
                if (direction === 1 && !this.atEndOfList) {
                    this.currentOffset -= this.paginationFactor;
                } else if (direction === -1 && !this.atHeadOfList) {
                    this.currentOffset += this.paginationFactor;
                }
            },
        },
        mounted() {
            // const element = document.getElementsByClassName('testimonial')[0];
            //  let style = element.currentStyle || window.getComputedStyle(element),
            //     width = element.offsetWidth,
            //     margin = window.innerWidth > 757 ? parseFloat(style.marginLeft)
            //         : parseFloat(style.marginLeft) + parseFloat(style.marginRight);
            // this.paginationFactor = width + margin;
        }
    }
</script>

<style>

</style>