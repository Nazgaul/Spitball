<template functional>
    <div class="wrapper h-p">
        <div class="box-header">
            <v-container fluid class="limited-width">
                <v-layout row>
                    <v-flex class="logo-wrapper">
                        <slot name="logo"></slot>
                    </v-flex>

                    <header>
                        <v-toolbar flat style="background-color:transparent;" height="52">
                            <div class="hidden-sm-and-down" v-for="action in props.links" :key="action.name">
                                <a href="#">{{ action.name }}</a>
                            </div>
                        </v-toolbar>
                    </header>
                </v-layout>
                <div class="box-small-container" @click.stop="">
                    <div class="box-inner-header">
                        <h2 class="box-title">Simplify School</h2>
                        <div class="box-search">
                            <form id="labnol" method="get" @submit.prevent="props.submitFunction">
                                <!--<div class="speech">-->
                                <v-container>
                                    <v-layout row>
                                        <v-flex class="tx-input">
                                            <v-menu offset-y full-width content-class="h-p-menu">
                                                <span slot="activator">
                                                    <slot name="inputField"></slot>
                                                </span>
                                                <v-list>
                                                    <v-subheader>Some things you can ask me:</v-subheader>
                                                    <template v-for="(item, index) in props.items">
                                                        <v-list-tile @click="props.selectosFunction(item)" :key="index">
                                                            <v-list-tile-action hidden-xs-only>
                                                                <v-icon>sbf-search</v-icon>
                                                            </v-list-tile-action>
                                                            <v-list-tile-content>
                                                                <v-list-tile-title v-text="item"></v-list-tile-title>
                                                            </v-list-tile-content>
                                                        </v-list-tile>
                                                    </template>
                                                </v-list>
                                            </v-menu>
                                        </v-flex>
                                        <v-flex class="f-submit">
                                            <button type="submit">
                                                <v-icon class="hidden-sm-and-up">sbf-search</v-icon>
                                                <span class="hidden-xs-only">Search</span>
                                            </button>
                                        </v-flex>
                                    </v-layout>
                                </v-container>
                            </form>
                        </div>
                    </div>
                </div>
                <slot name="featuresSection"></slot>
            </v-container>
        </div>
        <section class="websites-section">
            <v-container class="websites text-xs-center limited-width">
                <h2>Content from your favorite websites</h2>
                <v-layout class="logos" row align-center justify-space-around wrap>
                    <v-flex v-for="(site,index) in props.sites" class="site" :key="index" xs6 sm4 md2>
                        <v-layout column align-center d-flex>
                            <lazy-component> <img :src="require(`./img/${site.image}`)" /></lazy-component>
                        </v-layout>
                    </v-flex>
                </v-layout>
                <p class="text-xs-right text-sm-center">… and many more.</p>
            </v-container>
        </section>
        <slot name="stripSection"></slot>
        <section class="box-section testimonials">
            <v-carousel hide-delimiters left-control-icon="sbf-arrow-right" right-control-icon="sbf-arrow-right" class="limited-width">
                <v-carousel-item v-for="(testimonial,index) in props.testimonials" src="" :key="index">
                    <div class="testimonial">
                        <p class="testimonial-text">"{{testimonial.testimonial}}"</p>
                        <div class="author">
                            <lazy-component>
                                <img :src="require(`./img/${testimonial.image}`)" />
                            </lazy-component>                            <div>
                                <span class="name">{{testimonial.name}}</span>
                                <span class="uni">{{testimonial.uni}}</span>
                            </div>
                        </div>
                    </div>
                </v-carousel-item>
            </v-carousel>
        </section>

        <footer>
            <v-container class="limited-width">
                <v-layout row wrap justify-space-between>
                    <v-flex class="box-app">
                        <div class="footer-title">GET THE SPITBALL APP</div>
                        <p>Study socially on your smartphone, laptop and tablet anytime, anywhere</p>
                        <a href="#" class="app"><lazy-component><img src="./img/app-store-icon.png" alt=""></lazy-component></a>
                    </v-flex>
                    <v-flex class="box-links">
                        <v-layout row wrap justify-space-between>
                            <v-flex md6 sm12 xs12 class="box-list">
                                <div class="title">COMPANY</div>
                                <ul>
                                    <li><a href="#">About Spitball</a></li>
                                    <li><a href="#">Jobs</a></li>
                                    <li><a href="#">Blog</a></li>
                                </ul>
                            </v-flex>
                            <v-flex md6 sm12 xs12 class="box-list">
                                <div class="title">SUPPORT</div>
                                <ul>
                                    <li><a href="#">Privacy Policy</a></li>
                                    <li><a href="#">Terms of Service</a></li>
                                    <li><a href="#">Help</a></li>
                                    <li><a href="#">Advertise with us</a></li>
                                </ul>
                            </v-flex>
                        </v-layout>
                        <v-layout class="list-social-links" justify-space-between>
                            <v-flex class="text-xs-center" v-for="icon in props.bottomIcons" :key="icon" pa-0>
                                <a href="#">
                                    <span class="demo-icon">
                                        <slot name="bottomIcon" :iconName="icon"></slot>
                                    </span>
                                </a>
                            </v-flex>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-container>
        </footer>

    </div>
</template>
