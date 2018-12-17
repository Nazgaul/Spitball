<template>
    <div>
        <v-parallax
                height="745"
                class="wide-parallax"
                dark
                pa-0
                :src="require('./images/home_page_one.jpg')">
            <section class="hero" style="height: 100%;">
                <div class="hero-wrap">
                    <div class="hero-text-container">
                        <h1 class="hero-title">Earn From Your Knowledge</h1>
                        <h3>Help Students, Earn SBL and Cash out with Amazon Gift Cards</h3>
                    </div>
                    <a href="#" class="cta-button">Join Spitball its 100% Free</a>
                    <a class="video-link" @click.prevent="updateVideoId('6lt2JfJdGSY')">
                        <v-icon class="play-icon">sbf-play</v-icon>
                        See how it works</a>
                </div>
                <statistics class="statistics"></statistics>
            </section>
        </v-parallax>


        <section class="intro-one">
            <div class="right-part overlap-above">
                <div class="background-white-boxed">

                </div>
            </div>
            <div class="left-part gradient-background">
                <div class="input-holder">
                    <v-combobox
                            class="input-subject"
                            v-model="selectedSubject"
                            :items="subjectList"
                            :label="'some label'"
                            :item-text="'subject'"
                            :item-value="'id'"
                            :placeholder="'some placeholder'"
                            clearable
                            solo
                            return-object
                            :search-input.sync="search"
                            :append-icon="''"
                            :prepend-inner-icon="'sbf-search'"
                            :clear-icon="'sbf-close'"
                            @click:clear="clearData(search, selectedSubject)"
                            autofocus
                            no-filter
                            :background-color="'rgba( 255, 255, 255, 1)'"
                    >
                        <template slot="no-data">
                            <v-list-tile v-show="showBox">
                                <div class="subheading" v-language:inner>uniSelect_keep_typing</div>
                            </v-list-tile>
                            <v-list-tile>
                                <div style="cursor:pointer;" @click="getAllSubjects()" class="subheading dark"
                                     v-language:inner>uniSelect_show_all_schools
                                </div>
                            </v-list-tile>
                        </template>
                        <template slot="item" slot-scope="{ index, item, parent }">
                            <v-list-tile-content style="max-width:385px;">
                                <span v-html="$options.filters.boldText(item, search)" @click="goToResulstQuestionsPage(item)">{{ item }}</span>
                            </v-list-tile-content>
                        </template>
                    </v-combobox>
                </div>
            </div>
        </section>

        <section class="intro-two">
            <div class="left-part overlap-above">
                <div class="background-white-boxed ">

                </div>
            </div>
            <div class="right-part gradient-background">
                <div class="input-holder">
                    <v-combobox
                            class="input-uni"
                            v-model="university"
                            :items="universities"
                            :label="schoolNamePlaceholder"
                            :placeholder="schoolNamePlaceholder"
                            clearable
                            solo
                            :search-input.sync="searchUni"
                            :append-icon="''"
                            :clear-icon="'sbf-close'"
                            @click:clear="clearData(searchUni, university)"
                            autofocus
                            no-filter

                            :background-color="'rgba( 255, 255, 255, 1)'"
                    >
                        <template slot="no-data">
                            <v-list-tile v-show="showBoxUni">
                                <div class="subheading" v-language:inner>uniSelect_keep_typing</div>
                            </v-list-tile>
                            <v-list-tile>
                                <div style="cursor:pointer;" @click="getAllUniversities()" class="subheading dark" v-language:inner>uniSelect_show_all_schools</div>
                            </v-list-tile>
                        </template>
                        <template slot="item" slot-scope="{ index, item, parent }">
                            <v-list-tile-content style="max-width:385px;">
                                <span v-html="$options.filters.boldText(item.text, searchUni)">{{ item.text }}</span>
                            </v-list-tile-content>
                        </template>
                    </v-combobox>




                </div>

            </div>
        </section>
        <section class="faq">
            <div class="faq-wrap">
                <h3 class="faq-title">What is Spitball</h3>
                <p class="faq-text">Spitball is an online marketplace where students acquire knowledge and earn from
                    helping others. Our platform allows students to ask and answer homework problems, share study
                    documents and buy books. On spitball students get the right incentives to learn and earn and help
                    each other.</p>
                <h3 class="faq-title">How does Spitball differ from other websites?</h3>
                <p class="faq-text">Spitball brings power back to students, no subscription fees, no ads, no catch. With
                    Spitball you get the tools to learn smarter and earn so you can enjoy life.
                </p>

                <h3 class="faq-title">How does spitball work?</h3>
                <p class="faq-text">Students that need help with their studies, choose their campus and class browse
                    study documents or ask homework questions. Students that know the answer will help solve these
                    problems, and the most helpful answer or document gets rewarded with SBL.</p>
                <h3 class="faq-title">What is an SBL?</h3>
                <p class="faq-text">SBL is the currency used on the Spitball platform. SBL can be converted into real
                    money through Amazon Gift cards.</p>
                <h3 class="faq-title">How do I get SBL?</h3>
                <p class="faq-text">You get SBL when you register and earn more SBL by helping others or referring
                    friends.</p>
                <h3 class="faq-title">Wait, I can earn real money on Spitball, what’s the catch?</h3>
                <p class="faq-text">That’s right! There is no catch. Earned SBL’s are redeemable for Amazon gift
                    cards</p>
            </div>
            <button class="cta-sbl sb-rounded-btn">Get Your SBL</button>
        </section>

        <v-parallax class="overflowing-parallax"
                    dark
                    src="https://cdn.vuetifyjs.com/images/parallax/material.jpg">
            <section class="reviews">
                <h3 class="title" style="color: #ffffff;">Students talk about Spitball</h3>
                <div class="carousel-holder">
                    <v-carousel height="350">
                        <v-carousel-item v-for="(item,i) in items" :key="i"
                                         :src="item.src">

                        </v-carousel-item>
                    </v-carousel>
                </div>
            </section>
        </v-parallax>
        <section class="subjects">
            <h3 class="subject-title">Find Homework Help By Subject</h3>
            <span class="subject-sub-title">Just pick a subject, and we'll find the right tutor for you</span>
            <v-layout row wrap v-bind="binding" class="layout-subject">
                <v-flex v-for="subjectItem in subjectList" class="subject-item" :key="`${subjectItem.id}`" xs3>
                    <v-card class="subject-card">
                        <v-card-text class="subject-text" @click="updateSubject(subjectItem)">
                            {{subjectItem.subject}}
                        </v-card-text>
                    </v-card>
                </v-flex>
            </v-layout>
        </section>
        <section class="join">
            <h3 class="join-title">Join our newsletter</h3>
            <button class="join-cta">Join Now</button>
        </section>
        <landing-footer></landing-footer>
        <sb-dialog class="video-dialog" :isPersistent="false" :showDialog="playerVisible" :popUpType="'videoPlayer'"
                   :content-class="'videoPlayerDialog'">
            <youtube
            :video-id="youTubeVideoId"
            :player-vars="{autoplay: 1}">

            </youtube>
        </sb-dialog>
    </div>
</template>


<style src="./landingPage.less" scoped lang="less"></style>
<script src="./landingPage.js"></script>