<template>
    <v-flex v-if="cardData && !isDeleted " class="question-card" :class="{'highlight':flaggedAsCorrect}">
        <div v-if="!typeAnswer">
            <!-- question Card -->
            <div class="top-block" >
            <user-block :cardData="cardData" :user="cardData.user" v-if="cardData.user" :name="cardData.subject">
                <template> · <span class="timeago" :datetime="cardData.dateTime||cardData.create"></span><span
                        v-if="typeAnswer"
                        class="q-answer">
                    <button class="accept-btn right" @click="markAsCorrect"
                            v-if="showApproveButton && !flaggedAsCorrect && !hasAnswer">
                        <v-icon>sbf-check-circle</v-icon>
                        <span>Accept</span>
                    </button>

                    <span class="choosen-answer right" v-if="flaggedAsCorrect">
                        <v-icon>sbf-check-circle</v-icon></span>
                </span></template>
            </user-block>
            <div v-if="cardData.price">
                <div class="q-price pr-3">
                    <span>Earn ${{cardData.price | dollarVal}}</span>
                </div>
                <!-- <p class="q-category">{{cardData.subject}}</p> -->
            </div>
        </div>

        <p class="q-text" :class="{'answer': typeAnswer}">{{cardData.text}}</p>

        <!-- v-if="cardData.files.length" -->
        <div class="gallery" v-if="gallery&&gallery.length">
            <v-carousel prev-icon="sbf-arrow-right left" next-icon="sbf-arrow-right right"
                        interval="600000" cycle full-screen
                        hide-delimiters :hide-controls="gallery.length===1">
                <v-carousel-item v-for="(item,i) in gallery" v-bind:src="item" :key="i"
                                 @click.native="showBigImage(item)"></v-carousel-item>
            </v-carousel>
        </div>

        <div class="bottom-section">
            <div class="card-info general" v-if="!typeAnswer">
                <div class="new-block">
                    <div class="files" v-if="cardData.filesNum">
                        <template>
                            <v-icon>sbf-attach</v-icon>
                            <span>{{cardData.filesNum}}</span>
                        </template>
                    </div>
                    <div class="users">
                        <!--<template v-for="i in Math.min(3, cardData.answers)">-->
                        <template v-for="i in cardData.answersNum">
                            <div class="avatar">
                                <v-icon>sbf-comment-icon</v-icon>
                            </div>
                        </template>
                    </div>
                    <span class="user-counter" v-if="cardData.answersNum>3">+{{cardData.answersNum-3}}</span>
                </div>
                <!--show only if in suggestion popup-->
                <div class="answer" v-if="suggestion">
                <button class="answer-btn">Answer</button>
                </div>
            </div>
        </div>
        
        </div>


        <div v-else class="question-card-answer transparent">
            <!-- answer Card -->
            <div class="full-width-flex">
                <user-block :cardData="cardData" :user="cardData.user"></user-block>
                <div class="full-width-flex" :class="{'column-direction': gallery && gallery.length}">
                    <div class="full-width-flex calc-Margin answer-block">
                        <div class="triangle"></div>
                        <div class="text-container">
                            <div class="text">
                                <span class="user-date">Answer ·</span>
                                <span class="timeago" :datetime="cardData.dateTime||cardData.create"></span><span
                                    v-if="typeAnswer"
                                    class="q-answer">
                                <button class="accept-btn right" @click="markAsCorrect" v-if="showApproveButton && !flaggedAsCorrect && !hasAnswer">
                                    <v-icon>sbf-check-circle</v-icon>
                                    <span>Accept</span>
                                </button>

                                <span class="choosen-answer right" v-if="flaggedAsCorrect">
                                    <v-icon>sbf-check-circle</v-icon></span>
                                </span>
                            </div>

                            <p class="q-text" :class="{'answer': typeAnswer}">{{cardData.text}}</p>
                        </div>
                    </div>

                    <div class="gallery fixed-margin" v-if="gallery && gallery.length">
                        <v-carousel prev-icon="sbf-arrow-right left" next-icon="sbf-arrow-right right"
                                    interval="600000" cycle full-screen
                                    hide-delimiters :hide-controls="gallery.length===1">
                            <v-carousel-item v-for="(item,i) in gallery" v-bind:src="item" :key="i"
                                            @click.native="showBigImage(item)"></v-carousel-item>
                        </v-carousel>
                    </div>
                </div>
            </div>
            
        </div>


        
        <button :class="{'delete-btn': !typeAnswer, 'delete-btn-answer': typeAnswer}" v-if="detailedView && canDelete" @click="deleteQuestion()">Delete</button>

        <!-- TODO strange behaviour check why is being added tab index-1 to DOM-->
        <v-dialog v-model="showDialog"  max-width="720px"
                  transition="scale-transition" content-class="zoom-image">
            <img :src="selectedImage" alt="" height="auto" width="100%" class="zoomed-image">
        </v-dialog>
    </v-flex>
</template>
<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
