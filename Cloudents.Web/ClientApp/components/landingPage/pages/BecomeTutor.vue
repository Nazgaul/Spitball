<template>
    <div>
        <div class="page-hero">
            <div class="half-grid-center">
                <div class="hero-content-warraper">
                    <h2 class="hero-title">
                        The best job for students. Become an online tutor.</h2>
                    <div class="hero-text">Flexible one-on-one teaching that brings result.
                    </div>
                    <a href="#" class="button">Apply now</a>
                </div>
            </div>
            <div class="half-grid-center">
                <div class="hero-image" style='{background-image: url("../assets/image/TUTOR_1.png")}'></div>
            </div>
        </div>

        <div class="intro-step margin-top-50">
            <div class="container">
                <h2 class="intro-title centered-title">Why online tutoring?</h2>
                <div class="columns-grid">
                    <Column v-for="(feature, index) in features" :key="index" :column="feature"></Column>
                </div>
            </div>
        </div>
        <div class="step-box buttom-space">
            <div class="container">
                <div class="space-around-box">
                    <div class="half-grid-center">
                        <div class="into-content-warraper">
                            <h3 class="intro-title">It’s flexible, and fits your schedule</h3>
                            <div class="intro-content">
                                Work, lectures, essays, exams, social life… students are juggling a lot. This is why
                                Spitball online lessons are great, you can fit them in anytime, anywhere. In the
                                morning, in the evening, or on a Friday afternoon – whenever you have time. You’re in
                                control and can do as little or as much as you like
                            </div>
                        </div>
                    </div>
                    <div class="half-grid-center">
                        <div class="intro-image-warraper">
                        
                            <img src="../assets/images/group-15.png" alt="">
                            <!-- <img src="/assets/homepage_img2.png" alt=""> -->

                            <!-- <img src="/assets/group-15-no-floor.svg" alt=""> -->
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step green-background">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/images/group-19.png" alt="">
                                <!-- <div class="float-background2"></div> -->
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title yellow-title">You make some cash to help pay the bills</h3>
                                <div class="intro-content white-content">
                                    Tutoring online is ideal to earn some extra cash during your studies. It pays
                                    better than most other jobs, and it doesn’t require you to clean up or serve food.
                                            Better yet you can earn cash today and spend it tomorrow. Cha-ching!
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="step-box buttom-space">
            <div class="container">
                <div class="space-around-box">
                    <div class="half-grid-center">
                        <div class="into-content-warraper">
                            <h3 class="intro-title red-title">It’s the perfect job to improve your CV</h3>
                            <div class="intro-content">
                                As a private tutor, you’re basically running your own business and learn all the
                                skills that will impress your future employers
                            </div>
                            <ul class="list">
                                <li class="styled-li">Dealing with customers (parents and pupils)</li>
                                <li class="styled-li">Time management</li>
                                <li class="styled-li">Great communication with people</li>
                            </ul>
                        </div>
                    </div>
                    <div class="half-grid-center">
                        <div class="intro-image-warraper">
                                <img src="../assets/images/group-28.png" alt="">                  
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step pink-background">
            <div class="container">
                <h2 class="hero-centered-title">I love the fact that it improves my communication skills. I get to talk about subjects I love, and interact with lots of interesting people.</h2>
                <div class="testimonial-job-title hero-sub-title">Lilach, TAU</div>
            </div>
        </div>
        <div class="intro-step bright-background">
            <div class="step-box buttom-space">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                                <img src="../assets/images/group-29.png" alt="">                  
                                                         
                                
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title dark-blue-text">We help you succeed</h3>
                                <div class="intro-content">
                                    We want you to be an amazing tutor, so we support you in every possible way.
                                            We find pupils for you to teach so you don’t have to. Our learning
                                    platform is easy to use and makes each lesson feel personal. And we’re always here
                                    to answer your questions (or simply to give you a well-deserved pat on the back).
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step yellow-background">
            <div class="container">
                <h2 class="intro-title centered-title">Awesome!<br>So how do I get started?</h2>
                <div class="testimonial-job-title dark-blue-sub-title">It’s super easy.</div>
                <div class="columns-grid">
                    <div class="colomn">
                        <div class="colomn-head">
                            <div class="number dark-blue">1</div>
                            <div class="colomn-title dark-blue">Create an account</div>
                        </div>
                    </div>
                    <div class="colomn">
                        <div class="colomn-head">
                            <div class="number dark-blue">2</div>
                            <div class="colomn-title dark-blue">Complete your profile</div>
                        </div>
                    </div>
                    <div class="colomn">
                        <div class="colomn-head">
                            <div class="number dark-blue">3</div>
                            <div class="colomn-title dark-blue">Start tutoring</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="intro-step">
            <div class="step-box buttom-space">
                <div class="container wide-on-sm">
                    <h2 class="intro-title pink-title">See what other students say about tutoring with us</h2>
                    <Carousel :cards="testimonials"></Carousel>
                    <div class="centered-vertical-grid"><a href="#" class="button pink-button">Apply now</a></div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    import Column from '../components/Column.vue';
    import Carousel from '../components/Carousel.vue'

    export default {
        components:{
            Column,
            Carousel
        },
        name: "BecomeTotur",
        data(){
            return{
                features:[
                    {
                        thumb: './group-6.png',
                        title: 'Great pay',
                        text: 'Earn up to 100 NIS/hour. No need to spent time or money on travel. Enjoy.'
                    },
                    {
                        thumb: './spring.png',
                        title: 'Flexible',
                        text: 'Choose where and when you want to teach. All you need is a computer. '
                    },
                    {
                        thumb: './group-34.png',
                        title: 'Feel good',
                        text: 'Help someone succeed and get extra skills to put on your CV'
                    },
                ],
                testimonials: [
                    {
                        src: 'https://cdn.vuetifyjs.com/images/carousel/squirrel.jpg',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON1',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: 'https://cdn.vuetifyjs.com/images/carousel/squirrel.jpg',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON2',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: 'https://cdn.vuetifyjs.com/images/carousel/squirrel.jpg',
                        content: 'It really saves me time and effort. Chamer is exactly what our business has been lacking. Chamer was worth a fortune to my company.',
                        name: 'ALEX PARKINSON3',
                        jobTitle: 'Uni name / parent'
                    },
                    {
                        src: 'https://cdn.vuetifyjs.com/images/carousel/squirrel.jpg',
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