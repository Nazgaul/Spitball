<template>
    <v-flex v-if="cardData" xs12 class="question-card" :class="{'highlight':flaggedAsCorrect}">
        <div v-if="!typeAnswer">
            <div class="q-price">
                <span>Earn ${{cardData.price}}</span>
            </div>
            <!-- <p class="q-category">{{cardData.subject}}</p> -->
        </div>
        <user-block :user="cardData.user" v-if="cardData.user"></user-block>

        <div v-else class="q-answer">

            <button class="accept-btn" @click="markAsCorrect" v-if="showApproveButton & !flaggedAsCorrect">
                <v-icon>sbf-check-circle</v-icon>
                <span>Accept</span>
            </button>

            <div class="choosen-answer" v-if="flaggedAsCorrect">
                <v-icon>sbf-check-circle</v-icon>
            </div>


        </div>
        <button class="delete" v-if="detailedView && canDelete"
                @click="showDeleteDialog = true">
            <v-icon>sbf-trash</v-icon>
        </button>

        <p class="q-text" :class="{'answer': typeAnswer}">{{cardData.text}}</p>

        <!-- v-if="cardData.files.length" -->
        <div class="gallery" v-if="gallery&&gallery.length">
            <v-carousel left-control-icon="sbf-arrow-right left" right-control-icon="sbf-arrow-right" interval="600000" hide-delimiters>
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
            <div class="card-info general" v-if="!detailedView">
                <div class="new-block">
                    <div class="files">
                        <template v-if="cardData.files">
                        <v-icon>sbf-attach</v-icon>
                        <span>{{cardData.files}}</span>
                        </template>
                    </div>
                    <div class="users">
                        <template v-for="i in Math.min(3,cardData.answers)">
                        <div class="avatar">
                            <v-icon>sbf-comment-icon</v-icon>
                        </div>
                        </template>
                    </div>
                    <span class="user-counter" v-if="cardData.answers>3">+{{cardData.answers-3}}</span>
                </div>

                <div class="answer">
                    <button class="answer-btn">Answer</button>
                </div>
            </div>
        </div>

        <v-dialog v-model="showDeleteDialog">
            <v-card>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to delete?</h1>
                    <button @click="deleteQuestion()">Yes</button>
                    <button @click="showDeleteDialog = false">No</button>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
