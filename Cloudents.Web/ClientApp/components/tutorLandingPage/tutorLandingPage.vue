<template>
    
    <v-container class="tutor-landing-page-container">
        <top-nav></top-nav>
        <v-layout py-5 class="tutor-landing-page-header" align-center justify-center column>
            <v-flex pt-5 pb-3>
                <h1>Get a 1-on-1 lesson for ₪55/hr</h1>
            </v-flex>
            <v-flex pb-5>
                <h2>Find tutors who have experience teaching {your course} online</h2>
            </v-flex>
            <v-flex pt-4>
                <h3>95% starts reviews</h3>
            </v-flex>
            <div class="tutor-search-container">
                <tutor-search-component></tutor-search-component>
            </div>
        </v-layout>
        <v-layout class="tutor-landing-page-body" column>
            <v-flex class="tutor-landing-card-container" v-for="(item, index) in items" :key="index">
                <tutor-result-card class="mb-3 hidden-sm-and-down" :tutorData="item"></tutor-result-card>
                <tutor-result-card-mobile
                       class="mb-2 hidden-sm-and-up"
                        v-if="$vuetify.breakpoint.xsOnly" :tutorData="item"></tutor-result-card-mobile>
            </v-flex>
        </v-layout>
        <v-layout></v-layout>
        <v-layout></v-layout>
        <Footer></Footer>
    </v-container>
</template>

<script>
import tempTutorData from './tutorsData'

import tutorResultCard from '../results/tutorCards/tutorResultCard/tutorResultCard.vue';
import tutorResultCardMobile from '../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import topNav from '../landingPageTools/TopNav.vue'
import Footer from '../landingPageTools/Footer.vue'
import tutorSearchComponent from './components/tutorSearchInput/tutorSearchInput.vue'
export default {
    components:{
        tutorResultCard,
        tutorResultCardMobile,
        topNav,
        Footer,
        tutorSearchComponent
    },
    data(){
        return {
            items: tempTutorData
        }
    }
}
</script>

<style lang="less">
.tutor-landing-page-container{
    max-width: 100%;
    padding: 0;
    margin: 0;
    .tutor-landing-page-header{
        position: relative;
        background-color: #1b2441;
        h1{
            color: #3dc2ba;
            font-size: 50px;
            font-weight: bold;
            
        }
        h2{
            font-size: 28px;
            font-weight: bold;
            color: #ffffff;
        }
        h3{
            font-size: 22px;
            font-weight: 600;
            color: rgba(255, 255, 255, 0.87);
        }
        .tutor-search-container{
            width: 740px;
            position: absolute;
            bottom: -26px;
            box-shadow: 0 7px 13px 0 rgba(0, 0, 0, 0.28);
            border-radius: 4px;
        }
    }
    .tutor-landing-page-body{
            margin-top: 68px;
        .tutor-landing-card-container{
            margin: 0 auto;
            max-width: 900px;
        }
    }
}
</style>
