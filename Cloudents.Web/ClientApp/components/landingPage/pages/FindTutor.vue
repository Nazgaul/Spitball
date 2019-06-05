<template>
    <div>
        <div class="page-hero">
            <div class="half-grid-center">
                <div class="hero-content-warraper">
                    <h2 class="hero-title" v-language:inner="'landingPage_main_title'"></h2>
                    <div class="hero-text" v-language:inner="'landingPage_main_subtitle'"></div>
                  <router-link to="/tutor">
                    <button class="hero-btn" v-language:inner="'landingPage_main_btn'"></button>
                  </router-link>
                </div>
            </div>
            <div class="half-grid-center">
                <div class="hero-image">
                    <img src="../assets/images/FindTutor_1.png" alt="">
                </div>
            </div>
        </div>
            <div class="extra-details-warraper2 ">
            <div class="title-bar warraper">
                <div class="space-around-box2">

                    <div class="extra-details-box hide-on-sm" v-language:inner="'landingPage_main_stats_sources'"></div>
                    <div class="extra-details-box extra-details-box-stats">
                        <div class="text-block" v-language:inner="'landingPage_main_stats_reviews_presents'"></div>
                        <StarsRating :stars="5"></StarsRating>
                        <div class="text-block-2" v-language:inner="'landingPage_main_stats_reviews'"></div>
                    </div>
                    <div class="extra-details-box hide-on-sm" v-language:inner="'landingPage_main_stats_courses'"></div>
                    <div class="extra-details-box hide-on-sm" v-language:inner="'landingPage_main_stats_tutors'"></div>
                </div>
            </div>
        </div>

        <div class="intro-step backgroudDiff">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box warraper">
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title" v-language:inner="'landingPage_section1_title'"></h3>
                                <div class="intro-content" v-language:inner="'landingPage_section1_pharagraph'"></div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <div class="landingPage-higher-index">
                                <picture>
                                    <img src="../assets/images/group-42.png" alt="">
                                </picture>
                                </div>
                                <div class="float-background"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="intro-step green-background">
            <div class="step-box">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                            <div class="landingPage-higher-index"> 
                                <picture>
                                    <img src="../assets/images/group-27.png" alt="">
                                </picture>
                            </div>
                                <div class="float-background-bottom"></div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title yellow-title" v-language:inner="'landingPage_section2_title'"></h3>
                                <div class="intro-content white-content">
                                <span v-language:inner="'landingPage_section2_pharagraph'"></span> 
                                <span class="bold" v-language:inner="'landingPage_section2_pharagraph_bold'"></span></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="intro-step backgroudDiff">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box">
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title red-title"  v-language:inner="'landingPage_section3_title'"></h3>
                                <div class="intro-content" v-language:inner="'landingPage_section3_pharagraph'"></div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/images/group-18.png" alt="">
                        </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="intro-step backgroudDiffDarker">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/images/group-20.png" alt="">

                                </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title dark-blue-text" v-language:inner="'landingPage_section4_title'"></h3>
                                <div class="intro-content" v-language:inner="'landingPage_section4_pharagraph'"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step backgroudDiff">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box">
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title red-title" v-language:inner="'landingPage_section5_title'"></h3>
                                <div class="intro-content" v-language:inner="'landingPage_section5_pharagraph'"></div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/images/group-39.png" alt="">
                        </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step green-background">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box reverse-grid ">
                        <div class="half-grid-center">
                           <div class="intro-image-warraper">
                                <img src="../assets/images/group-49.png" alt="">
                            </div>
                        </div>
                        <div class="half-grid-center">
                              <div class="into-content-warraper">
                                <h3 class="intro-title yellow-title" v-language:inner="'landingPage_section6_title'"></h3>
                                <div class="intro-content white-content" v-language:inner="'landingPage_section6_pharagraph'"></div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step backgroudDiff">
            <div class="step-box buttom-space">
                <div class="container wide-on-sm">
                    <Carousel v-if="cards" :cards="cards"></Carousel>
                </div>
            </div>
        </div>
        <!-- <div class="light-thin-grid">
            <div class="container">
                <div class="space-around-box">
                    <ExtraLinks :links="extraLinks"></ExtraLinks>
                </div>
            </div>
        </div> -->
        <div class="footer-hero">
            <div class="container">
                <div class="centered-vertical-grid">
                    <h3 class="hero-footer-title" v-language:inner="'landingPage_subfooter_title'"></h3>
                    <a href="#" class="button pink-button" v-language:inner="'landingPage_subfooter_btn'" ></a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import StarsRating from '../components/StarsRating.vue';
    import ExtraLinks from '../components/ExtraLinks.vue';
    import Carousel from "../components/Carousel.vue";
    import {reviews} from '../assets/data/testimonialsData'

    export default {
        components: {
            StarsRating,
            ExtraLinks,
            Carousel
        },
        name: "home",
        data() {
            return {
                // extraLinks: [
                //     {title: 'Math', url: ''},
                //     {title: 'Math', url: ''},
                //     {title: 'Math', url: ''},
                //     {title: 'Math', url: ''},
                //     {title: 'Math', url: ''},
                //     {title: 'Math', url: ''}
                // ],
                cards: reviews[0]
            }

        },
    }
</script>

<style>
</style>