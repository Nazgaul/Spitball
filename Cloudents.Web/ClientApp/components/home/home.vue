<template v-once>
    <div class="wrapper">
        <div class="box-header">
            <v-container fluid>
                <v-layout row>
                    <v-flex class="logo-wrapper">
                        <logo class="logo"></logo>
                    </v-flex>

                    <header>
                        <v-toolbar flat style="background-color:transparent;" height="52">
                            <!--<v-toolbar-side-icon @click.stop="drawer = !drawer" class="hidden-md-and-up"><menu-icon></menu-icon></v-toolbar-side-icon>-->
                            <!--<v-spacer class="hidden-sm-and-down"></v-spacer>-->
                            <div class="hidden-sm-and-down" v-for="action in links" :key="action.name">
                                <a href="#">{{ action.name }}</a>
                            </div>
                        </v-toolbar>
                    </header>
                </v-layout>
                <div class="box-small-container" @click.stop="">
                    <div class="box-inner-header">
                        <!--<img src="./img/logo-spitball.png" width="227" height="54">-->

                        <h2 class="box-title">Simplify School</h2>
                        <div class="box-search mb-3">
                            <form id="labnol" method="get" @submit.prevent="search">
                                <!--<div class="speech">-->
                                <v-container>
                                    <v-layout row>
                                        <v-flex class="search-icon">
                                            <search-icon></search-icon>
                                        </v-flex>
                                        <v-flex class="tx-input">
                                            <v-menu offset-y full-width>
                                                <input slot="activator" @keyup.enter="search" autocomplete="off" type="text" required name="q" id="transcript" v-model.trim="msg" :placeholder="placeholder">
                                                <v-list>
                                                    <v-subheader>Some things you can ask me:</v-subheader>
                                                    <template v-for="(item, index) in items">
                                                        <v-list-tile @click="selectos(item)" :key="index">
                                                            <v-list-tile-action>
                                                                -
                                                            </v-list-tile-action>
                                                            <v-list-tile-content>
                                                                <v-list-tile-title v-text="item"></v-list-tile-title>
                                                            </v-list-tile-content>
                                                        </v-list-tile>
                                                    </template>
                                                </v-list>
                                            </v-menu>
                                        </v-flex>
                                        <v-flex class="mic" v-if="voiceEnable">
                                            <button type="button" class="demo-icon-mic" @click="$_voiceDetection"><component :is="(isRecording?'recording-icon':'mic-icon')"></component></button>
                                        </v-flex>
                                        <v-flex class="f-submit">
                                            <button type="submit">Search</button>
                                        </v-flex>
                                    </v-layout>
                                </v-container>


                                <!--</div>-->
                            </form>
                        </div>
                    </div>
                </div>
                <features-section></features-section>

            </v-container>
        </div>
        <section class="websites-section">
            <v-container class="websites text-xs-center">
                <h2>Content from your favorite websites</h2>
                <v-layout class="logos" row align-center justify-space-around wrap>
                    <v-flex v-for="site in sites" class="site">
                        <v-layout column align-center d-flex>
                            <img :src="require(`./img/${site.image}`)" />
                        </v-layout>
                    </v-flex>
                </v-layout>
                <p class="text-xs-right text-sm-center">… and many more.</p>
            </v-container>
        </section>
        <strips-section></strips-section>


        <section class="box-section testimonials">
            <v-carousel hide-delimiters>
                <v-carousel-item v-for="testimonial in testimonials" src="">
                    <div class="testimonial">
                        <p class="testimonial-text">"{{testimonial.testimonial}}"</p>
                        <div class="author">
                            <img :src="require(`./img/${testimonial.image}`)" />
                            <div>
                                <span class="name">{{testimonial.name}}</span>
                                <span class="uni">{{testimonial.uni}}</span>
                            </div>
                        </div>
                    </div>
                </v-carousel-item>
            </v-carousel>
        </section>

        <footer>
            <v-container grid-list-xl>
                <v-layout row wrap>
                    <v-flex md6 sm12 xs12 class="box-app">
                        <div class="title">GET THE SPITBALL APP</div>
                        <p>Study socially on your smartphone, laptop and tablet anytime, anywhere</p>
                        <a href="#" class="app"><img src="./img/app-store-icon.png" alt=""></a>
                    </v-flex>
                    <v-flex md3 sm6 xs6 class="box-list">
                        <div class="title">COMPANY</div>
                        <ul>
                            <li><a href="#">About Spitball</a></li>
                            <li><a href="#">Jobs</a></li>
                            <li><a href="#">Blog</a></li>
                        </ul>
                    </v-flex>
                    <v-flex md3 sm6 xs6 class="box-list">
                        <div class="title">SUPPORT</div>
                        <ul>
                            <li><a href="#">Privacy Policy</a></li>
                            <li><a href="#">Terms of Service</a></li>
                            <li><a href="#">Help</a></li>
                            <li><a href="#">Advertise with us</a></li>
                        </ul>
                    </v-flex>
                </v-layout>
                <v-layout row>
                    <v-flex md6 sm8 xs10 offset-md6>
                        <ul class="list-social-links">
                            <li><a href="#"><span class="demo-icon"><facebook-icon></facebook-icon></span></a></li>
                            <li><a href="#"><span class="demo-icon"><twitter-icon></twitter-icon></span></a></li>
                            <li><a href="#"><span class="demo-icon"><google-icon></google-icon></span></a></li>
                            <li><a href="#"><span class="demo-icon"><youtube-icon></youtube-icon></span></a></li>
                            <li><a href="#"><span class="demo-icon"><instegram-icon></instegram-icon></span></a></li>
                        </ul>
                    </v-flex>
                </v-layout>
            </v-container>
        </footer>

    </div>

</template>

<style src="./home.less" lang="less" scoped></style>
<script src="./home.js"></script>