<template>
    <div>

        <div
                :height="$vuetify.breakpoint.xsOnly ? '560' : '745'"
                class="wide-parallax"
                pa-0>

            <landing-header></landing-header>

            <section class="hero">
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
        </div>


        <section class="intro-one">
            <div class="title-container">
                <div class="spacer-one" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div class="title-wrap">
                    <h3 class="intro-one-title">Answer Questions, Earn SBL</h3>
                    <span class="intro-one-sub-title">Cash out your SBL into amazon gift cards </span>
                </div>
            </div>
            <div class="right-part overlap-above">
                <div class="background-white-boxed">

                </div>
            </div>
            <div class="left-part gradient-background">
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div style="width:100%;" class="input-holder">
                    <v-combobox v-if="$vuetify.breakpoint.smAndUp"
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
                            <v-list-tile-content style="max-width:385px;" @click="goToResulstQuestionsPage(item)">
                                <span v-html="$options.filters.boldText(item, search)"
                                      >{{ item }}</span>
                            </v-list-tile-content>
                        </template>
                    </v-combobox>
                    <div class="dummy-input" @click="showMobileSubjectInput()" v-else>
                        <v-icon class="dummy-icon">sbf-search</v-icon>
                        <span class="dummy-placeholder">some text</span>
                    </div>
                </div>
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>

            </div>
        </section>

        <section class="intro-two">
            <div class="title-container">
                <div class="spacer-two" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div class="title-wrap">
                    <h3 class="intro-two-title">Upload Documents For Your Classes</h3>
                    <span class="intro-two-sub-title">Class Notes, Study Guides, Exams Preparations</span>
                </div>

            </div>
            <div class="left-part overlap-above">
                <div class="background-white-boxed ">

                </div>
            </div>
            <div class="right-part gradient-background">
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div class="input-holder" style="width: 100%;">
                    <v-combobox v-if="$vuetify.breakpoint.smAndUp"
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
                                <div style="cursor:pointer;" @click="getAllUniversities()" class="subheading dark"
                                     v-language:inner>uniSelect_show_all_schools
                                </div>
                            </v-list-tile>
                        </template>
                        <template slot="item" slot-scope="{ index, item, parent }">
                            <v-list-tile-content style="max-width:385px;"  @click="goToResultDocumentsPage(item)">
                                <span v-html="$options.filters.boldText(item.text, searchUni)">{{ item.text }}</span>
                            </v-list-tile-content>
                        </template>
                    </v-combobox>
                    <div class="dummy-input" @click="showMobileUniInput()" v-else>
                        <v-icon class="dummy-icon">sbf-search</v-icon>
                        <span class="dummy-placeholder">text some</span>
                    </div>
                </div>
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>

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

        <!--<v-parallax class="overflowing-parallax"-->
        <!--dark-->
        <!--src="https://cdn.vuetifyjs.com/images/parallax/material.jpg">-->
        <section class="reviews">
            <h3 class="reviews-title" v-show="$vuetify.breakpoint.xsAndUp">Students talk about Spitball</h3>
            <div class="carousel-holder">
                <v-carousel
                        height=""
                        hide-delimiters
                        :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-left-carousel left'"
                        :next-icon="isRtl ?  'sbf-arrow-right left': 'sbf-arrow-right-carousel right'"
                        :cycle="false"
                        :max="'250'">
                    <v-carousel-item v-for="(items,i) in formattedReviews"  :key="`desktop-${i}`" v-if="!$vuetify.breakpoint.xsOnly">
                        <template v-for="(data, index) in items">
                            <div :key="`innerData_${index}`" class="review-item-wrap">
                                <img  class="review-image"  :src="require(`${data.image}`)"  :alt="data.title">
                                <span class="review-name">{{data.name}}</span>
                                <span class="review-title">{{data.title}}</span>
                                <span class="review-text">{{data.text}}</span>
                            </div>
                        </template>
                    </v-carousel-item>
                    <v-carousel-item v-for="(item,index) in formattedReviews"  v-else>
                        <template >
                            <div :key="`innerData_${index}`" class="review-item-wrap">
                                <img  class="review-image"  :src="require(`${item.image}`)"  :alt="item.title">
                                <span class="review-name">{{item.name}}</span>
                                <span class="review-title">{{item.title}}</span>
                                <span class="review-text">{{item.text}}</span>
                            </div>
                        </template>
                    </v-carousel-item>

                </v-carousel>
            </div>
        </section>
        <!--</v-parallax>-->
        <section class="subjects" v-if="$vuetify.breakpoint.smAndUp">
            <h3 class="subject-title">Find Homework Help By Subject</h3>
            <span class="subject-sub-title">Just pick a subject, and we'll find the right tutor for you</span>
            <v-layout row wrap v-bind="binding" class="layout-subject">
                <v-flex v-for="subjectItem in subjectList" class="subject-item" :key="`${subjectItem.id}`" xs3>
                    <v-card class="subject-card">
                        <v-card-text class="subject-text" @click="goToResulstQuestionsPage(subjectItem)">
                            {{subjectItem.subject}}
                        </v-card-text>
                    </v-card>
                </v-flex>
            </v-layout>
        </section>
        <section class="join">
            <h3 class="join-title">Join Spitball Its Free!</h3>
            <button class="join-cta">SIGN UP</button>
        </section>
        <landing-footer></landing-footer>
        <sb-dialog class="video-dialog" :isPersistent="false" :showDialog="playerVisible" :popUpType="'videoPlayer'"
                   :content-class="'videoPlayerDialog'">
            <youtube
                    :video-id="youTubeVideoId"
                    :player-vars="{autoplay: 1}">

            </youtube>
        </sb-dialog>
        <sb-dialog class="video-dialog" :isPersistent="false" :showDialog="mobileSubjectsDialog"
                   :popUpType="'subject-combobox'"
                   :content-class="'subjects-combo-pop'">


            <v-icon class="dialog-action close-icon" @click="closeSubjectInputDialog()">sbf-close</v-icon>
            <div class="combo-wrap">

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
                        <v-list-tile-content @click="goToResulstQuestionsPage(item)" style="max-width:385px;">
                                <span v-html="$options.filters.boldText(item, search)"
                                >{{ item }}</span>
                        </v-list-tile-content>
                    </template>
                </v-combobox>
            </div>

        </sb-dialog>
        <sb-dialog class="video-dialog" :isPersistent="false" :showDialog="mobileUniDialog" :popUpType="'uni-combobox'"
                   :content-class="'uni-combo-pop'">

            <v-icon class="dialog-action close-icon" @click="closeUniInputDialog()">sbf-close</v-icon>
            <div class="combo-wrap">
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
                        :prepend-inner-icon="'sbf-search'"
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
                            <div style="cursor:pointer;" @click="getAllUniversities()" class="subheading dark"
                                 v-language:inner>uniSelect_show_all_schools
                            </div>
                        </v-list-tile>
                    </template>
                    <template slot="item" slot-scope="{ index, item, parent }">
                        <v-list-tile-content style="max-width:385px;" @click="goToResultDocumentsPage(item)">
                            <span v-html="$options.filters.boldText(item.text, searchUni)">{{ item.text }}</span>
                        </v-list-tile-content>

                    </template>
                </v-combobox>
            </div>

        </sb-dialog>


    </div>
</template>


<style src="./landingPage.less"  lang="less"></style>
<script src="./landingPage.js"></script>