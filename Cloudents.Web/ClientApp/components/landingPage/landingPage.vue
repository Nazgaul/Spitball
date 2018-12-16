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
            <div class="right-part">
                <div class="placeholder">

                </div>
            </div>
            <div class="left-part">
                <v-combobox
                        class="hello-gaby"
                        v-model="selectedSubject"
                        :items="subjectList"
                        :label="'some label'"
                        :placeholder="'some placeholder'"
                        clearable
                        solo
                        :search-input.sync="search"
                        :append-icon="''"
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
                            <span v-html="$options.filters.boldText(item, search)">{{ item }}</span>
                        </v-list-tile-content>
                    </template>
                </v-combobox>
            </div>
        </section>

        <section class="intro-two">
            <div class="left-part">
                <div class="placeholder">

                </div>
            </div>
            <div class="right-part">
                <v-combobox
                        class="hello-gaby"
                        v-model="selectedSubject"
                        :items="subjectList"
                        :label="'some label'"
                        :placeholder="'some placeholder'"
                        clearable
                        solo
                        :search-input.sync="search"
                        :append-icon="''"
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
                            <span v-html="$options.filters.boldText(item, search)">{{ item }}</span>
                        </v-list-tile-content>
                    </template>
                </v-combobox>
            </div>
        </section>
        <section class="faq">
            <h3 class="title">What is Spitball</h3>

            <p class="quote">Mauris sit amet mauris a arcu eleifend ultricies eget ut dolor. Class aptent taciti
                sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.</p>
            <p class="author">— Patrick Farrell</p>

            <p class="quote">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam id felis et ipsum bibendum
                ultrices. Morbi vitae pulvinar velit. Sed aliquam dictum sapien, id sagittis augue malesuada eu.</p>
            <p class="author">— George Smith</p>

            <p class="quote">Donec commodo dolor augue, vitae faucibus tortor tincidunt in. Aliquam vitae leo quis mi
                pulvinar ornare. Integer eu iaculis metus.</p>
            <p class="author">— Kevin Blake</p>
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

        <section class="contact">
            <h3 class="title">Join our newsletter</h3>
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam id felis et ipsum bibendum ultrices.
                Morbi vitae pulvinar velit. Sed aliquam dictum sapien, id sagittis augue malesuada eu.</p>
            <hr>

            <form>
                <input type="email" placeholder="Email">
                <a href="#" class="btn">Subscribe now</a>
            </form>
        </section>
        <landing-footer></landing-footer>
        <sb-dialog class="video-dialog" :isPersistent="false" :showDialog="playerVisible" :popUpType="'videoPlayer'"
                   :content-class="'videoPlayerDialog'">
            <youtube-media
                    :video-id="youTubeVideoId"
                    :player-vars="{autoplay: 1}">

            </youtube-media>
        </sb-dialog>
    </div>
</template>


<style src="./landingPage.less" scoped lang="less"></style>
<script src="./landingPage.js"></script>