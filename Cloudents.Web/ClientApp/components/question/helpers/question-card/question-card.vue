<template>
    <v-flex v-if="cardData&&!isDeleted"  class="question-card" :class="{'highlight':flaggedAsCorrect}">
        <div class="top-block">
            <user-block :user="cardData.user" v-if="cardData.user" :name="cardData.subject||'Answer'">
                <template> · <span class="timeago" :datetime="cardData.dateTime||cardData.create"></span><span
                        v-if="typeAnswer"
                        class="q-answer">
                    <!--<h3>{{cardData}}</h3>-->
                    <button class="accept-btn right" @click="markAsCorrect"
                            v-if="showApproveButton && !flaggedAsCorrect && !hasAnswer">
                        <v-icon>sbf-check-circle</v-icon>
                        <span>Accept</span>
                    </button>

                    <span class="choosen-answer right" v-if="flaggedAsCorrect">
                        <v-icon>sbf-check-circle</v-icon></span>


                </span></template>
            </user-block>
            <div v-if="!typeAnswer">
                <div class="q-price pr-3">
                    <span>Earn ${{cardData.price | dollarVal}}</span>
                </div>
                <!-- <p class="q-category">{{cardData.subject}}</p> -->
            </div>
        </div>

        <p class="q-text" :class="{'answer': typeAnswer}">{{cardData.text}}</p>

        <!-- v-if="cardData.files.length" -->
        <div class="gallery" v-if="gallery&&gallery.length">
            <v-carousel left-control-icon="sbf-arrow-right left" right-control-icon="sbf-arrow-right right"
                        interval="600000" cycle full-screen
                        hide-delimiters :hide-controls="gallery.length===1">
                <v-carousel-item v-for="(item,i) in gallery" v-bind:src="item" :key="i"></v-carousel-item>
            </v-carousel>
        </div>


        <div class="bottom-section">
            <!-- <div v-if="detailedView && cardData.user" class="q-user-info card-info detailed">
                <user-block :user="cardData.user"></user-block>
                <div v-if="typeAnswer && showApproveButton">
                    <label for="mark-correct">Mark as correct answer</label>
                    <input id="mark-correct" type="checkbox" @click="markAsCorrect" :disabled="isCorrectAnswer"
                           :checked="isCorrectAnswer"/>
                </div>
            </div> -->
            <!-- v-else -->
            <div class="card-info general" v-if="!typeAnswer">
                <div class="new-block">
                    <div class="files" v-if="cardData.filesNum">
                        <template>
                            <v-icon>sbf-attach</v-icon>
                            <span>{{cardData.filesNum}}</span>
                        </template>
                    </div>
                    <div class="users">
                        <template v-for="i in Math.min(3,cardData.answersNum)">
                            <div class="avatar">
                                <v-icon>sbf-comment-icon</v-icon>
                            </div>
                        </template>
                    </div>
                    <span class="user-counter" v-if="cardData.answersNum>3">+{{cardData.answersNum-3}}</span>
                </div>

                <!--<div class="answer" v-if="!detailedView">-->
                <!--<button class="answer-btn">Answer</button>-->
                <!--</div>-->
            </div>
        </div>
        <button class="delete-btn" v-if="detailedView && canDelete" @click="deleteQuestion()">Delete</button>
    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
