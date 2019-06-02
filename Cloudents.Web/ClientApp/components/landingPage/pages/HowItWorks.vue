<template>
    <div>
        <div class="step-box">
            <div class="container">
                <div class="centered-vertical-grid">
                    <h3 class="hero-title red-title">One-to-one online tutoring that raises grades</h3>
                    <h3 class="hero-small-title">Get personalized lessons from the comfort of your home</h3>
                    <div class="hero-small-image">
                        <img src="../assets/screen-shot-2019-04-17-at-14-58-31.png" alt="">
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step dark-blue-background">
            <div class="container">
                <div class="columns-grid">
                    <Column v-for="(feature, index) in features" :column="feature" :index="++index"></Column>
                </div>
            </div>
        </div>
        <div class="step-box buttom-space">
            <div class="container">
                <div class="space-around-box">
                    <div class="half-grid-center">
                        <div class="into-content-warraper">
                            <h3 class="intro-title red-title">Only the best tutors for your kids</h3>
                            <div class="intro-content">
                                We know kids learn best when someone talks to them in their language. This is why every
                                tutor gets hand picked and reviewed for their ability to communicate on their level.
                                        Most tutors are young and can easily connect with your kids.
                            </div>
                        </div>
                    </div>
                    <div class="half-grid-center">
                        <div class="intro-image-warraper">
                            <img src="assets/group-18.png" alt="">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step green-background bottom-space">
            <div class="step-box">
                <div class="container">
                    <div class="space-around-box reverse-grid">
                        <div class="half-grid-center">
                            <div class="intro-image-warraper">
                            <img src="../assets/homepage_img3.png" alt="">

                                <img src="/assets/group-15.svg" alt="">
                            </div>
                        </div>
                        <div class="half-grid-center">
                            <div class="into-content-warraper">
                                <h3 class="intro-title yellow-title">It’s flexible and fits your family’s schedule</h3>
                                <div class="intro-content white-content">
                                    Because lessons are online there’s no need to travel, so you can get your kids the
                                    best tutors, no matter where they’re located. Also lessons can happen anytime,
                                            anywhere so you can fit them in easily between activities and dinner and
                                    you never need to miss a lesson, even when you’re on vacation.
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
                            <h3 class="intro-title">See your kids progress</h3>
                            <div class="intro-content">
                                Each lesson is recorded for everyone’s benefit. Your kids can revisit each lesson
                                whenever they need and you can see which topics were covered and their progress in
                                each lesson
                            </div>
                        </div>
                    </div>
                    <div class="half-grid-center">
                        <div class="intro-image-warraper">
                            <img src="/assets/group-19.svg" alt="">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="intro-step">
            <div class="container">
                <div class="columns-grid">
                    <Column v-for="feature in features2" :column="feature"></Column>
                </div>
            </div>
        </div>
        <div class="footer-hero">
            <div class="container">
                <div class="centered-vertical-grid">
                    <h3 class="hero-footer-title">Talk to a tutor for free today and find out how they can help your child</h3>
                    <a href="#" class="button pink-button">Find a tutor</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import Column from '../components/Column.vue';

    export default {
        components:{
            Column
        },
        name: "FindTotur",
        data(){
            return{
                features:[
                    {
                        title: 'Talk face to face',
                        text: 'Get the experience of an in person lesson without needing to travel thanks to live video'
                    },
                    {
                        title: 'Share and collaborate',
                        text: 'Upload questions and old exams and master each subject on the whiteboard together'
                    },
                    {
                        title: 'Rewatch lessons',
                        text: 'Lessons are recorded so you can watch them again when you prepare for your exam'
                    },
                ],
                features2:[
                    {
                        thumb: '/assets/group-17.svg',
                        title: 'Talk face to face',
                        text: 'Get the experience of an in person lesson without needing to travel thanks to live video'
                    },
                    {
                        thumb: '/assets/group-17.svg',
                        title: 'Share and collaborate',
                        text: 'Upload questions and old exams and master each subject on the whiteboard together'
                    },
                    {
                        thumb: '/assets/group-17.svg',
                        title: 'Rewatch lessons',
                        text: 'Lessons are recorded so you can watch them again when you prepare for your exam'
                    },
                ]
            }
        }
    }
</script>

<style>

</style>