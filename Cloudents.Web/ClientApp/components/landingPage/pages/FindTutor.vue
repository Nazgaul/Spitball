<template>
    <div>
        <div class="page-hero">
            <div class="half-grid-center">
                <div class="hero-content-warraper">
                    <h2 class="hero-title">Find the perfect tutor to boost your child’s grades</h2>
                    <div class="hero-text">
                        Private 1 - on - 1 online lessons with expert tutors of your choice for only 55 NIS.
                    </div>
                    <a href="#" class="button">Find a tutor</a>
                </div>
            </div>
            <div class="half-grid-center">
                <div class="hero-image"></div>
            </div>
        </div>
        <div class="extra-details-warraper">
            <div class="container">
                <div class="space-around-box">
                    <div class="extra-details-box hide-on-sm">
                        Used by 300 + schools
                    </div>
                    <div class="extra-details-box">
                        <div class="text-block">98, 000</div>
                        <StarsRating :stars="4"></StarsRating>
                        <div class="text-block-2">reviews</div>
                    </div>
                    <div class="extra-details-box hide-on-sm">
                        1m + use our study notes
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box">
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title">Better grades only for 55 NIS/hr</h3>
                                <div class="intro-content">
                                    Helping your kids get better grades doesn’t need to be expensive.For a limited time,
                                    Spitball offers one on one online lessons at a discount.No subscriptions or upfront
                                    payments, you only pay for the time you need and only if you’re 100 % satisfied with the lesson.
                                </div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <picture>
                                    <!-- <source media="(min-width: 1108px)" srcset="../assets/group-15.svg"> -->
                                    <img src="../assets/homepage_img1.png" alt="">
                                    <!-- <img src="../assets/group-15-small.svg" alt=""> -->
                                </picture>
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
                                <picture>
                                    <source media="(min-width: 1175px)" srcset="../assets/group-12.svg">
                                    <img src="../assets/group-12-small.svg" alt="">
                                </picture>
                                <div class="float-background2"></div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title yellow-title">It’s flexible and fits your family’s schedule</h3>
                                <div class="intro-content white-content">
                                    Because lessons are online there’z no need to travel, so you can get your kids the
                                    best tutors, no matter where they’re located.Also lessons can happen anytime,
                                            anywhere so you can fit them in easily between activities and dinner and
                                    you never need to miss a lesson, even when you’re on vacation.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box">
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title red-title">Better grades only for 55 NIS/hr</h3>
                                <div class="intro-content">
                                    We know kids learn best when someone talks to them in their language.This is why
                                    every tutor gets hand picked and reviewed for their ability to communicate on their
                                    level.Most tutors are young and can easily connect with your kids.
                                </div>
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/group-18.png" alt=""></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step bright-background">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/group-19.svg" alt=""></div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title dark-blue-text">See your kids progress</h3>
                                <div class="intro-content">
                                    Each lesson is recorded for everyone’ s benefit.Your kids can revisit each lesson
                                    whenever they need and you can see which topics were covered and their progress
                                    in each lesson
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step">
            <div class="step-box buttom-space">
                <div class="container wide-on-sm">
                    <Carousel :cards="testimonials"></Carousel>
                </div>
            </div>
        </div>
        <div class="light-thin-grid">
            <div class="container">
                <div class="space-around-box">
                    <ExtraLinks :links="extraLinks"></ExtraLinks>
                </div>
            </div>
        </div>
        <div class="footer-hero">
            <div class="container">
                <div class="centered-vertical-grid">
                    <h3 class="hero-footer-title">Talk to a tutor for free today and find out how they can help your
                        child</h3>
                    <a href="#" class="button pink-button">Find a tutor</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import StarsRating from '../components/StarsRating.vue';
    import ExtraLinks from '../components/ExtraLinks.vue';
    import Carousel from "../components/Carousel.vue";

    export default {
        components: {
            StarsRating,
            ExtraLinks,
            Carousel
        },
        name: "home",
        data() {
            return {
                extraLinks: [
                    {title: 'Math', url: ''},
                    {title: 'Math', url: ''},
                    {title: 'Math', url: ''},
                    {title: 'Math', url: ''},
                    {title: 'Math', url: ''},
                    {title: 'Math', url: ''}
                ],
                testimonials: [
                    {
                        src: '',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON1',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: '',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON2',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: '',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON3',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: '',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON4',
                        jobTitle: 'Uni name / parent'
                    },
                ]
            }
        }
    }
</script>

<style>

</style>