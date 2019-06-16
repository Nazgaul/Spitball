<template>
    <div>
        <div class="wide-parallax" pa-0>
            <landing-header></landing-header>

            <section class="hero">
                <div class="text-switch-container" v-show="isMobileView">
                    <a :class="{'white-text': dictionaryType === dictionaryTypesEnum.earn, 'yellow-text': dictionaryType === dictionaryTypesEnum.learn,}"
                       class="lp-header-link" @click="changeUrlType('learn')" v-language:inner>landingPage_header_learn_faster</a>
                    <a :class="{'white-text': dictionaryType === dictionaryTypesEnum.learn, 'yellow-text': dictionaryType === dictionaryTypesEnum.earn,}"
                       class="lp-header-link" @click="changeUrlType('earn')" v-language:inner>landingPage_header_earn_money</a>
                </div>
                <div class="hero-wrap">
                    <div class="hero-text-container">
                        <h1 class="hero-title" v-html="$Ph(`landingPage_${dictionaryType}_knowledge_title`)"></h1>
                        <h3 v-html="$Ph(`landingPage_${dictionaryType}_knowledge_subTitle`)"></h3>
                    </div>
                    <router-link :to="dictionaryType === dictionaryTypesEnum.earn ? {path: '/note'} : {path: '/register'}"
                                 class="cta-button">
                        <span v-show="dictionaryType === dictionaryTypesEnum.earn" v-language:inner>landingPage_upload_and_earn</span>
                        <span v-show="dictionaryType === dictionaryTypesEnum.learn"  v-language:inner>landingPage_join_spitball</span>

                    </router-link>
                    <a class="video-link" @click.prevent="updateVideoId(SpitballVideoId)">
                        <v-icon class="play-icon ml-2">sbf-play</v-icon>
                        <span v-language:inner>landingPage_how_it_works</span>
                    </a>
                </div>
                <statistics class="statistics" :statsData="statsData"></statistics>
            </section>
        </div>


        <section class="intro-one">
            <div class="title-container">
                <div class="spacer-one" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div class="title-wrap">
                    <h3 class="intro-one-title"
                        v-html="$Ph(`landingPage_${dictionaryType}_question_section_title`)"></h3>
                    <span class="intro-one-sub-title"
                          v-html="$Ph(`landingPage_${dictionaryType}_question_section_subTitle`)"></span>
                </div>
            </div>
            <div class="right-part overlap-above">
                <div class="background-white-boxed">
                    <div class="gif-container-question" :class="{'learn': dictionaryType === 'learn'}"></div>
                </div>
            </div>
            <div class="left-part gradient-background">
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div style="width:100%;" class="input-holder">
                    <v-combobox v-if="$vuetify.breakpoint.smAndUp"
                                class="input-subject"
                                v-model="selectedSubject"
                                :items="subjectList"
                                :label="subjectsPlaceholder"
                                :item-text="'subject'"
                                :item-value="'id'"
                                :placeholder="subjectsPlaceholder"
                                clearable
                                solo
                                return-object
                                :search-input.sync="search"
                                :append-icon="''"
                                :prepend-inner-icon="'sbf-search'"
                                :clear-icon="'sbf-close'"
                                @click:clear="clearData(search, selectedSubject)"
                                
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
                        <span class="dummy-placeholder">{{subjectsPlaceholder}}</span>
                    </div>
                </div>
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>

            </div>
        </section>

        <section class="intro-two">
            <div class="title-container">
                <div class="spacer-two" v-if="$vuetify.breakpoint.smAndUp"></div>
                <div class="title-wrap">
                    <h3 class="intro-two-title" v-html="$Ph(`landingPage_${dictionaryType}_upload_section_title`)"></h3>
                    <span class="intro-two-sub-title"
                          v-html="$Ph(`landingPage_${dictionaryType}_upload_section_subTitle`)"></span>
                </div>

            </div>
            <div class="left-part overlap-above">
                <div class="background-white-boxed ">
                    <div class="gif-container-document" :class="{'learn': dictionaryType === 'learn'}"></div>
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
                                :prepend-inner-icon="'sbf-search'"
                                :clear-icon="'sbf-close'"
                                @click:clear="clearData(searchUni, university)"
                                
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
                    <div class="dummy-input" @click="showMobileUniInput()" v-else>
                        <v-icon class="dummy-icon">sbf-search</v-icon>
                        <span class="dummy-placeholder">{{schoolNamePlaceholder}}</span>
                    </div>
                </div>
                <div style="flex-grow:1;width:100%;" v-if="$vuetify.breakpoint.smAndUp"></div>

            </div>
        </section>
        <section class="faq">
            <div class="faq-wrap">
                <h3 class="faq-title" v-language:inner>landingPage_what_is_spitball</h3>
                <p class="faq-text" v-language:inner>landingPage_what_is_spitball_desc</p>
                <h3 class="faq-title" v-language:inner>landingPage_how_spitball_differs</h3>
                <p class="faq-text" v-language:inner>landingPage_how_spitball_differs_desc</p>

                <h3 class="faq-title" v-language:inner>landingPage_how_spitball_work</h3>
                <p class="faq-text" v-language:inner>landingPage_how_spitball_work_desc</p>
                <h3 class="faq-title" v-language:inner>landingPage_how_sbl_money</h3>
                <p class="faq-text" v-language:inner>landingPage_how_sbl_money_desc</p>
                <h3 class="faq-title" v-language:inner>landingPage_how_get_sbl</h3>
                <p class="faq-text" v-language:inner>landingPage_how_get_sbl_desc</p>
                <h3 class="faq-title" v-language:inner>landingPage_how_is_catch</h3>
                <p class="faq-text" v-language:inner>landingPage_how_is_catch_desc</p>
            </div>
            <router-link :to="{path: '/register'}" class="cta-sbl sb-rounded-btn" v-language:inner>
                landingPage_get_your_sbl
            </router-link>
        </section>

        <!--dark-->
        <!--src="https://cdn.vuetifyjs.com/images/parallax/material.jpg">-->
        <section class="reviews">
            <h3 class="reviews-title" v-show="!$vuetify.breakpoint.xsOnly" v-language:inner>
                landingPage_spitball_student_title</h3>
            <div class="carousel-holder">
                <v-carousel
                        :touchless="isRtl ? true : false"
                        :height="$vuetify.breakpoint.xsOnly ? 470 : 500"
                        hide-delimiters
                        :prev-icon="isRtl ? 'sbf-arrow-right-carousel right' : 'sbf-arrow-left-carousel left'"
                        :next-icon="isRtl ?  'sbf-arrow-left-carousel left': 'sbf-arrow-right-carousel right'"
                        :cycle="false"
                        :max="'250'">
                    <v-carousel-item v-if="!$vuetify.breakpoint.xsOnly" v-for="(items,i) in reviewItems"
                                     :key="`desktop-${i}`">
                        <template v-for="(data, index) in items">
                            <div :key="`innerData_${index}`" class="review-item-wrap">
                                <div class="review-image-wrap">
                                    <img class="review-image" :src="require(`${data.image}`)" :alt="data.title">
                                </div>
                                <span class="review-name">{{data.name}}</span>
                                <span class="review-title">{{data.title}}</span>
                                <span class="review-text">{{data.text}}</span>
                            </div>
                        </template>
                    </v-carousel-item>
                    <v-carousel-item v-else v-for="(item, index) in mobileReviewItems"
                                     :key="`mobile-testimonials-${index}`">
                        <div class="review-item-wrap">
                            <div class="review-image-wrap">
                                <img class="review-image" :src="require(`${item.image}`)" :alt="item.title">
                            </div>
                            <span class="review-name">{{item.name}}</span>
                            <span class="review-title">{{item.title}}</span>
                            <span class="review-text">{{item.text}}</span>
                        </div>
                    </v-carousel-item>

                </v-carousel>
            </div>
        </section>
        <section class="subjects" v-if="$vuetify.breakpoint.smAndUp">
            <h3 class="subject-title" v-language:inner>landingPage_find_by_subject</h3>
            <span class="subject-sub-title" v-language:inner>landingPage_find_by_subject_desc</span>
            <v-layout row wrap v-bind="binding" class="layout-subject">
                <v-flex v-for="subjectItem in subjectList" class="subject-item" :key="`${subjectItem.id}`" xs3>
                    <v-card elevation="0" class="subject-card">
                        <v-card-text class="subject-text" @click="goToResulstQuestionsPage(subjectItem)">
                            {{subjectItem.subject}}
                        </v-card-text>
                    </v-card>
                </v-flex>
            </v-layout>
        </section>
        <section class="join">
            <h3 class="join-title" v-language:inner>landingPage_join_spitball_free</h3>
            <router-link :to="{path: '/register'}" class="join-cta" v-language:inner>landingPage_sign_up</router-link>
        </section>
        <landing-footer></landing-footer>
        <sb-dialog :onclosefn="hideVideoPlayer" class="video-dialog" :isPersistent="false"
                   :showDialog="playerVisible"
                   :popUpType="'videoPlayer'"
                   :content-class="'videoPlayerDialog'">
            <div v-if="$vuetify.breakpoint.xsOnly" class="mb-3 mt-3 d-flex justify-end">
                <v-icon class="mr-1 justify-end" style="flex-direction: row; max-width: 50px;"
                        @click="hideVideoPlayer()">sbf-close
                </v-icon>

            </div>
            <youtube
                    :video-id="youTubeVideoId" :player-width="playerWidth" :player-height="playerHeight"
                    :player-vars="{autoplay: 1}" @ready="readyPlayer">

            </youtube>
        </sb-dialog>
        <sb-dialog class="subjects-dialog"
                   :isPersistent="false"
                   :showDialog="mobileSubjectsDialog"
                   :popUpType="'subject-combobox'"
                   :content-class="'subjects-combo-pop'">


            <v-icon class="dialog-action close-icon" @click="closeSubjectInputDialog()">sbf-close</v-icon>
            <div class="combo-wrap">
                <v-combobox
                        style="z-index: 999;"
                        class="input-subject"
                        v-model="selectedSubject"
                        :items="subjectList"
                        :label="subjectsPlaceholder"
                        :item-text="'subject'"
                        :item-value="'id'"
                        :menu-props="{value: openDropdownSubjectMobile}"
                        :placeholder="subjectsPlaceholder"
                        clearable
                        solo
                        return-object
                        :search-input.sync="search"
                        :append-icon="''"
                        :prepend-inner-icon="'sbf-search'"
                        :clear-icon="'sbf-close'"
                        @click:clear="clearData(search, selectedSubject)"
                        :autofocus="mobileSubjectsDialog"
                        no-filter
                        :background-color="'rgba( 255, 255, 255, 1)'"
                >
                    <template slot="no-data">
                        <v-list-tile v-show="showBox">
                            <div class="subheading" v-language:inner>uniSelect_keep_typing</div>
                        </v-list-tile>
                        <v-list-tile >
                            <div style="cursor:pointer;" @click="getAllSubjects()" class="subheading dark"
                                 v-language:inner>uniSelect_show_all_schools
                            </div>
                        </v-list-tile>
                    </template>
                    <template slot="item" slot-scope="{ index, item, parent }" >
                        <v-list-tile-content @click="goToResulstQuestionsPage(item)" style="max-width:385px;">
                                <span v-html="$options.filters.boldText(item, search)"
                                >{{ item }} {{openDropdownSubjectMobile}}</span>
                        </v-list-tile-content>
                    </template>
                </v-combobox>
            </div>

        </sb-dialog>
        <sb-dialog class="uni-dialog"
                   :isPersistent="false"
                   :showDialog="mobileUniDialog"
                   :popUpType="'uni-combobox'"
                   :content-class="'uni-combo-pop'">

            <v-icon class="dialog-action close-icon" @click="closeUniInputDialog()">sbf-close</v-icon>
            <div class="combo-wrap">
                <v-combobox
                        style="z-index: 999;"
                        class="input-uni"
                        v-model="university"
                        :items="universities"
                        :label="schoolNamePlaceholder"
                        :menu-props="{value: openDropdownUniMobile}"
                        :placeholder="schoolNamePlaceholder"
                        clearable
                        solo
                        :search-input.sync="searchUni"
                        :append-icon="''"
                        :prepend-inner-icon="'sbf-search'"
                        :clear-icon="'sbf-close'"
                        @click:clear="clearData(searchUni, university)"
                        :autofocus="mobileUniDialog"
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


<style src="./landingPage.less" lang="less"></style>
<script src="./landingPage.js"></script>