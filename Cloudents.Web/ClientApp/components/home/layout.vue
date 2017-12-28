<template functional>
    <div class="wrapper h-p">
        <div class="box-header">
            <div class="box-small-container">
                <div class="box-inner-header">
                    <h2 class="box-title">Simplify School</h2>
                    <h4>Curating all of the resources you need. For <b>Free.</b></h4>
                    <div class="hidden-sm-and-down">
                        <slot name="search"></slot>
                    </div>
                </div>
            </div>
            <slot name="featuresSection"></slot>
        </div>
        <section class="websites-section">
            <v-container class="websites text-xs-center limited-width">
                <h2>Our Partners</h2>
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
            <v-carousel hide-delimiters left-control-icon="sbf-arrow-right" interval="6000000" right-control-icon="sbf-arrow-right" class="limited-width">
                <v-carousel-item v-for="(testimonial,index) in props.testimonials" src="" :key="index">
                    <div class="testimonial">
                        <p class="testimonial-text">"{{testimonial.testimonial}}"</p>
                        <div class="author">
                            <lazy-component>
                                <div class="testimonial-image"></div>
                                <!--<img :src="require(`./img/${testimonial.image}`)" />-->
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
                        <p>Simplify School. Find class notes, textbooks, deals, tutors and more anytime, anywhere.</p>
                        <a href="#" class="app"><lazy-component><img src="./img/app-store-icon.png" alt=""></lazy-component></a>
                    </v-flex>
                    <v-flex class="box-links">
                        <v-layout class="about-links" row wrap justify-space-between>
                            <v-flex class="box-list">
                                <div class="list-title">COMPANY</div>
                                <ul>
                                    <li><a href="#">How Spitball Works</a></li>
                                    <li><a href="#">Work For Us</a></li>
                                    <li><a href="#">Blog</a></li>
                                    <li><a href="#">Contact Us</a></li>
                                </ul>
                            </v-flex>
                            <v-flex class="box-list">
                                <div class="list-title">SUPPORT</div>
                                <ul>
                                    <li><a href="#">Privacy Policy</a></li>
                                    <li><a href="#">Terms of Service</a></li>
                                    <li><a href="#">FAQ</a></li>
                                    <li><a href="#">Become a Partner</a></li>
                                </ul>
                            </v-flex>
                        </v-layout>
                        <v-layout class="list-social-links" justify-space-between>
                            <v-flex v-for="icon in props.bottomIcons" :key="icon" pa-0>
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
