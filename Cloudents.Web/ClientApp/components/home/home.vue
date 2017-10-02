<template v-once>
    <!--<div class="page">
        <form v-on:submit.prevent="search">
            <input v-model.trim="msg" :placeholder="placeholder" />
            <input type="submit" value="search" />
        </form>
        <vertical-collection :changeCallback="changeSection"></vertical-collection>
    </div>-->
    <div class="wrapper">
        <div class="box-header">

            <div class="container">
                <header>
                    <div class="layout row">
                        <div class="flex sm3">
                            <a href="#" class="logo">
                                <img class="hidden-sm-and-down" src="img/logo-spitball.png" alt="">
                                <img class="hidden-md-and-up" src="img/spitball-logo.png" alt="">
                            </a>
                        </div>
                        <div class="flex sm9">
                            <nav class="animenu">
                                <button class="animenu__toggle">
                                    <span class="animenu__toggle__bar"></span>
                                    <span class="animenu__toggle__bar"></span>
                                    <span class="animenu__toggle__bar"></span>
                                </button>
                                <ul class="animenu__nav right">
                                    <li><a href="#">Spitball Guide</a></li>
                                    <li><a href="#">Key Features</a></li>
                                    <li><a href="#">Shared Documents</a></li>
                                    <li><a href="#">Mobile App</a></li>
                                </ul>
                            </nav>
                        </div>
                    </div>
                </header>

                <div class="box-small-container">
                    <div class="box-inner-header">

                        <div class="title">Study Help & More for College Students</div>
                        <div class="sub-title">Work smart, play smart with our class material, study tools, textbooks, local food recs and more!</div>

                        <div class="box-search">
                            <form id="labnol" method="get" action="https://www.google.com/search">
                                <div class="speech">
                                    <input type="text" name="q" id="transcript" placeholder="Ask me anything..">
                                    <button type="button" class="demo-icon-mic"><img src="img/svg/mic.svg" alt=""></button>
                                    <button type="submit"><img src="img/svg/search-icon.svg" alt=""> Send</button>
                                </div>
                            </form>

                        </div>

                    </div>
                    <div class="box-nav-circle">
                        <ul class="scroll">
                            <li class="box-1">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/ask-icon.svg" alt=""></span></div>
                                    Ask
                                </a>
                            </li>
                            <li class="box-2">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/class-material-icon.svg" alt=""></span></div>
                                    Class Material
                                </a>
                            </li>
                            <li class="box-3">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/flashcard-icon.svg" alt=""></span></div>
                                    Flashcards
                                </a>
                            </li>
                            <li class="box-4">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/tutor-icons.svg" alt=""></span></div>
                                    Tutors
                                </a>
                            </li>
                            <li class="box-5">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/jobs-icon.svg" alt=""></span></div>
                                    Jobs
                                </a>
                            </li>
                            <li class="box-6">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/textbooks-icon.svg" alt=""></span></div>
                                    Textbooks
                                </a>
                            </li>
                            <li class="box-7">
                                <a href="#">
                                    <div class="hover"><span><img src="img/svg/food-icon.svg" alt=""></span></div>
                                    Food
                                </a>
                            </li>
                        </ul>
                    </div>


                </div>


            </div>

        </div>

        <section class="box-section vertical-pink">
            <div class="container box-class-materials">
                <div class="layout row box-small-container">
                    <div class="flex md8 sm12 xs12">
                        <div class="title"><span class="demo-icon"><img src="img/svg/class-material-icon-purple.svg" alt=""></span> Class Materials</div>

                        <p>Spitball curates content for you from the best sites on the web. The documents populate based on student ratings and are filtered by your school, classes, and preferences.</p>
                    </div>


                </div>
                <div class="layout row box-small-container">
                    <div class="flex md12">
                        <div class="img">
                            <img class="hidden-sm-and-down" src="img/class-materiall-screen.png" alt="">
                            <img class="hidden-md-and-up" src="img/class-material-mobil-image.png" alt="">
                        </div>
                    </div>
                </div>

            </div>
        </section>

        <section class="box-section">
            <div class="container up-box">
                <div class="layout row wrap">
                    <div class="flex md6 sm12 xs12 box-tutors">
                        <div class="title"><span class="demo-icon"><img src="img/svg/tutor-icon-green.svg" alt=""></span> Tutors</div>

                        <p>Spitball has teamed up with the most trusted tutoring services. All of our online and in-person tutors are highly qualified experts with educations from the best universities around the world. </p>

                        <div class="img"><img src="img/group-copy-5.png" alt=""></div>
                        <div class="img"><img src="img/group-2-copy.png" alt=""></div>
                    </div>
                    <div class="flex md6 sm12 xs12 box-jobs">
                        <div class="title"><span class="demo-icon"><img src="img/svg/jobs-icon-orange.svg" alt=""></span> Jobs and Internships</div>

                        <p>Easily find and apply to paid internships, part-time jobs and entry-level opportunities at thousands of Fortune 500 companies and startups. </p>

                        <div class="img">
                            <img src="img/group-copy-4.png" alt="">
                            <img src="img/group-copy-6.png" alt="">
                        </div>
                    </div>
                </div>
            </div>
            <div class="layoyt row box-substrate">
                <div class="flex md6 vertical-green"></div>
                <div class="flex md6 vertical-orange"></div>
            </div>
        </section>

        <section class="box-section vertical-purple">
            <div class="container box-textbooks">
                <div class="layout row box-small-container">
                    <div class="flex md8 sm12 xs12">
                        <div class="title"><span class="demo-icon"><img src="img/svg/textbooks-icon-purple.svg" alt=""></span> Textbooks</div>

                        <p>Spitball finds you textbooks at the best prices by simultaneously searching multiple websites. Compare prices to buy, rent, and sell back your books.</p>
                    </div>

                </div>
                <div class="layout row box-small-container">
                    <div class="flex md12 sm12 xs12">
                        <div class="img">
                            <img class="hidden-sm-and-down" src="img/textbook-screen.png" alt="">
                            <img class="hidden-md-and-up" src="img/textbook-mobile-screen.png" alt="">
                        </div>
                    </div>
                </div>

            </div>
        </section>

        <section class="box-section box-testimonials">
            <div class="container">
                <div class="row box-small-container">
                    <p>"As a sophomore, I found myself looking for a new way to communicate and study with my classmates. After searching online, I was shocked that there was no real solution that fit my lifestyle (nothing I could easily access on my phone or computer). Then someone invited me to Spitball... it saved me."</p>
                    <img src="img/user-mage-place-holder.png" alt="">
                    <div class="author">Donna Floyd <span>Sophomore at UC Davis</span></div>
                </div>

            </div>
        </section>
        <section class="box-section box-websites">
            <div class="container">
                <div class="title">Content from your favorite websites</div>
                <div class="layout row wrap">
                    <div class="flex xs6 sm6 md2"><img src="img/quizlet.png" alt=""></div>
                    <div class="flex xs6 sm6 md2"><img src="img/chegg.png" alt=""></div>
                    <div class="flex xs6 sm6 md2"><img src="img/study-blue.png" alt=""></div>
                    <div class="flex xs6 sm6 md2"><img src="img/wyzant.png" alt=""></div>
                    <div class="flex xs6 sm6 md2"><img src="img/course-hero.png" alt=""></div>
                    <div class="flex xs6 sm6 md2"><img src="img/wayup.png" alt=""></div>
                </div>

            </div>
        </section>

        <footer>
            <div class="container grid-list-xl">
                <div class="layout row wrap">
                    <div class="flex md6 sm12 xs12 box-app">
                        <div class="title">GET THE SPITBALL APP</div>
                        <p>Study socially on your smartphone, laptop and tablet anytime, anywhere</p>

                        <a href="#" class="app"><img src="img/app-store-icon.png" alt=""></a>
                    </div>
                    <div class="flex md3 sm6 xs6 box-list">
                        <div class="title">COMPANY</div>
                        <ul>
                            <li><a href="#">About Spitball</a></li>
                            <li><a href="#">Jobs</a></li>
                            <li><a href="#">Blog</a></li>
                        </ul>
                    </div>
                    <div class="flex md3 sm6 xs6 box-list">
                        <div class="title">SUPPORT</div>
                        <ul>
                            <li><a href="#">Privacy Policy</a></li>
                            <li><a href="#">Terms of Service</a></li>
                            <li><a href="#">Help</a></li>
                            <li><a href="#">Advertise with us</a></li>
                        </ul>
                    </div>
                </div>
                <div class="layout row">
                    <div class="flex md6 sm8 xs10 offset-md6">
                        <ul class="list-social-links">
                            <li><a href="#"><span class="demo-icon"><img src="img/svg/facebook-icon.svg" alt=""></span></a></li>
                            <li><a href="#"><span class="demo-icon"><img src="img/svg/twitter-icon.svg" alt=""></span></a></li>
                            <li><a href="#"><span class="demo-icon"><img src="img/svg/google-icon.svg" alt=""></span></a></li>
                            <li><a href="#"><span class="demo-icon"><img src="img/svg/youtube-icon.svg" alt=""></span></a></li>
                            <li><a href="#"><span class="demo-icon"><img src="img/svg/instagram-icon.svg" alt=""></span></a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </footer>

    </div>

</template>

<style src="./home.less" lang="less" scoped></style>
<script src="./home.js"></script>
