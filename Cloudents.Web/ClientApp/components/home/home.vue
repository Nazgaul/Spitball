<template v-once>
    <div class="wrapper">
        <v-navigation-drawer temporary
                             light
                             v-model="drawer"
                             overflow
                             absolute>
            <v-list>
                <template v-for="action in links">
                    <v-list-tile append>
                        <v-list-tile-content>
                            <v-list-tile-title>{{ action.name }}</v-list-tile-title>
                        </v-list-tile-content>
                    </v-list-tile>
                </template>
            </v-list>
        </v-navigation-drawer>
        <div class="box-header">
            <v-container fluid @click="showOptions=false">
                <header>
                    <v-toolbar flat style="background-color:transparent;">
                        <v-toolbar-side-icon @click.stop="drawer = !drawer" class="hidden-md-and-up"><menu-icon></menu-icon></v-toolbar-side-icon>
                        <v-spacer class="hidden-sm-and-down"></v-spacer>
                        <div class="hidden-sm-and-down" v-for="action in links" :key="action.name">
                            <a href="#">{{ action.name }}</a>
                        </div>
                    </v-toolbar>
                </header>

                <div class="box-small-container" @click.stop="">
                    <div class="box-inner-header">
                        <img  src="./img/logo-spitball.png"  width="227" height="54">

                        <div class="box-search mb-3">
                            <form id="labnol" method="get" v-on:submit.prevent="search">
                                <div class="speech">
                                    <input autocomplete="off" type="text" required name="q" id="transcript" v-model.trim="msg" @focus="$_focus()"  :placeholder="placeholder">
                                    <button type="button" class="demo-icon-mic"><mic-icon></mic-icon></button>
                                    <button type="submit"><search-icon></search-icon> Send</button>
                                </div>
                            </form>
                        </div>
                        <div v-show="showOptions">
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
                        </div>
                    </div>
                </div>
            </v-container>
        </div>

        <section class="box-section vertical-pink">
            <v-container class="box-class-materials">
                <v-layout row class="box-small-container">
                    <v-flex md8 sm12 xs12>
                        <div class="title"><span class="demo-icon"><class-material-icon></class-material-icon></span> Class Materials</div>
                        <p>Spitball curates content for you from the best sites on the web. The documents populate based on student ratings and are filtered by your school, classes, and preferences.</p>
                    </v-flex>
                </v-layout>
                <v-layout row class="box-small-container">
                    <v-flex md12>
                        <div class="img">
                            <img class="hidden-sm-and-down" src="./img/class-materiall-screen.png" alt="">
                            <img class="hidden-md-and-up" src="./img/class-material-mobil-image.png" alt="">
                        </div>
                    </v-flex>
                </v-layout>
            </v-container>
        </section>

        <section class="box-section">
            <v-container class="up-box">
                <v-layout row wrap>
                    <v-flex md6 sm12 xs12 class="box-tutors">
                        <div class="title"><span class="demo-icon"><tutor-icon></tutor-icon></span> Tutors</div>

                        <p>Spitball has teamed up with the most trusted tutoring services. All of our online and in-person tutors are highly qualified experts with educations from the best universities around the world. </p>

                        <div class="img"><img src="./img/group-copy-5.png" alt=""></div>
                        <div class="img"><img src="./img/group-2-copy.png" alt=""></div>
                    </v-flex>
                    <v-flex md6 sm12 xs12 class="box-jobs">
                        <div class="title"><span class="demo-icon"><job-icon></job-icon></span> Jobs and Internships</div>

                        <p>Easily find and apply to paid internships, part-time jobs and entry-level opportunities at thousands of Fortune 500 companies and startups. </p>

                        <div class="img">
                            <img src="./img/group-copy-4.png" alt="">
                            <img src="./img/group-copy-6.png" alt="">
                        </div>
                    </v-flex>
                </v-layout>
            </v-container>
            <v-layout row class="box-substrate">
                <v-flex md6 class="vertical-green"></v-flex>
                <v-flex md6 class="vertical-orange"></v-flex>
            </v-layout>
        </section>

        <section class="box-section vertical-purple">
            <v-container class="box-textbooks">
                <v-layout row class="box-small-container">
                    <v-flex md8 sm12 xs12>
                        <div class="title"><span class="demo-icon"><text-book-icon></text-book-icon></span> Textbooks</div>
                        <p>Spitball finds you textbooks at the best prices by simultaneously searching multiple websites. Compare prices to buy, rent, and sell back your books.</p>
                    </v-flex>

                </v-layout>
                <v-layout row class="box-small-container">
                    <v-flex md12 sm12 xs12>
                        <div class="img">
                            <img class="hidden-sm-and-down" src="./img/textbook-screen.png" alt="">
                            <img class="hidden-md-and-up" src="./img/textbook-mobile-screen.png" alt="">
                        </div>
                    </v-flex>
                </v-layout>

            </v-container>
        </section>

        <section class="box-section box-testimonials">
            <v-container>
                <div class="box-small-container">
                    <p>"As a sophomore, I found myself looking for a new way to communicate and study with my classmates. After searching online, I was shocked that there was no real solution that fit my lifestyle (nothing I could easily access on my phone or computer). Then someone invited me to Spitball... it saved me."</p>
                    <img src="./img/user-mage-place-holder.png" alt="">
                    <div class="author">Donna Floyd <span>Sophomore at UC Davis</span></div>
                </div>

            </v-container>
        </section>
        <section class="box-section box-websites">
            <v-container>
                <div class="title">Content from your favorite websites</div>
                <v-layout row wrap>
                    <v-flex xs6 sm6 md2><img src="./img/quizlet.png" alt=""></v-flex>
                    <v-flex xs6 sm6 md2><img src="./img/chegg.png" alt=""></v-flex>
                    <v-flex xs6 sm6 md2><img src="./img/study-blue.png" alt=""></v-flex>
                    <v-flex xs6 sm6 md2><img src="./img/wyzant.png" alt=""></v-flex>
                    <v-flex xs6 sm6 md2><img src="./img/course-hero.png" alt=""></v-flex>
                    <v-flex xs6 sm6 md2><img src="./img/wayup.png" alt=""></v-flex>
                </v-layout>
            </v-container>
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
