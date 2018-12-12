<template>
<!-- ********************** THIS IS THE ANSWER CARD ********************** -->
    <v-flex v-if="cardData && !isDeleted " class="question-card" :class="[`sbf-card-${!!cardData.color ? cardData.color.toLowerCase() : 'undefined' }`, {'highlight':flaggedAsCorrect}]">

        <div class="question-card-answer transparent">
            <!-- answer Card -->
            <div class="full-width-flex">
                <div>
                    <user-block class="q-user-block" :cardData="cardData" :user="cardData.user"></user-block>
                    <div class="answer-body-container">
                        <span class="answer-raputation upvote-arrow" @click.prevent="upvoteAnswer()">
                            <v-icon>sbf-arrow-up</v-icon>
                        </span>
                        <span class="answer-raputation answer-score">{{cardData.votes}}</span>
                        <span class="answer-raputation downvote-arrow" @click.prevent="downvoteAnswer()">
                            <v-icon>sbf-arrow-down</v-icon>
                        </span>
                    </div>
                </div>
                
                <div class="full-width-flex" :class="{'column-direction': gallery && gallery.length}">
                    <div class="full-width-flex calc-Margin answer-block">
                        <div class="triangle"></div>
                        <div class="text-container">
                            <div class="text">
                                <span class="user-date" v-language:inner>questionCard_Answer</span>
                                <user-rank style="margin-top: 1px;" :score="cardData.user.score"></user-rank>
                                <span class="timeago"
                                      :datetime="cardData.dateTime||cardData.create"></span><span
                                    v-if="typeAnswer"
                                    class="q-answer">
                                <button class="accept-btn right" @click="markAsCorrect"
                                        v-if="showApproveButton && !flaggedAsCorrect && !hasAnswer">
                                    <v-icon>sbf-check-circle</v-icon>
                                    <span v-language:inner>questionCard_Accept</span>
                                </button>

                                <span class="choosen-answer right" v-if="flaggedAsCorrect">
                                    <v-icon>sbf-check-circle</v-icon></span>
                                </span>
                            </div>

                            <p class="q-text" :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`, {'answer': typeAnswer}]">{{cardData.text}}</p>
                        </div>
                    </div>
                    <div class="gallery fixed-margin" v-if="gallery && gallery.length">
                        <v-carousel  :prev-icon="isRtl ? 'sbf-arrow-right rigth' : 'sbf-arrow-right left'"
                                     :next-icon="isRtl ?  'sbf-arrow-right left': 'sbf-arrow-right right'"
                                    interval="600000" cycle full-screen
                                    hide-delimiters :hide-controls="gallery.length===1">
                            <v-carousel-item v-for="(item,i) in gallery" v-bind:src="item" :key="i"
                                             @click.native="showBigImage(item)"></v-carousel-item>
                        </v-carousel>
                    </div>
                </div>
            </div>
            <button :class="{'delete-btn': !typeAnswer, 'delete-btn-answer': typeAnswer}"
                    v-if="detailedView && canDelete" @click="deleteQuestion()" v-language:inner>questionCard_Delete
            </button>
            <!-- TODO strange behaviour check why is being added tab index-1 to DOM-->
            <v-dialog v-model="showDialog" max-width="720px"
                      transition="scale-transition" content-class="zoom-image">
                <img :src="selectedImage" alt="" height="auto" width="100%" class="zoomed-image">
            </v-dialog>
        </div>
    </v-flex>
</template>
<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
