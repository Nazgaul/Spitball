<template>
    <v-flex v-if="cardData" xs12 class="question-card">
        <div v-if="!typeAnswer">
            <div class="q-price">
                <span>Earn {{cardData.price}}$</span>
            </div>
            <p class="q-category">{{cardData.subject}}</p>
            <button v-if="myQuestion" :disabled="!cardData.answers" @click="()=>{haveAnswers?showDelete=true:deleteQuestion()}">Delete</button>
            <v-dialog  v-model="showDelete">
                {{dialogDeleteUserText}}
            </v-dialog>
        </div>

        <div v-else class="q-answer">
            <p class="q-text"><strong>Answer:</strong></p>
            <div class="votes">
                <button class="upvote-btn" @click="upVote()" :disabled="submitted">
                    <v-icon class="upvotes">sbf-chevron-down</v-icon>
                </button>
                <span>{{cardData.upVote}} Upvote{{cardData.upVote === 1 ? '' :'s'}}</span>
            </div>

        </div>

        <p class="q-text">{{cardData.text}}</p>

        <div class="bottom-section">
            <div v-if="detailedView && cardData.user" class="q-user-info card-info detailed"><!--v-if="cardData.user"-->
                <user-block :user="cardData.user"></user-block>
                <div v-if="typeAnswer && showApproveButton">
                    <label for="mark-correct">Mark as correct answer</label>
                    <input id="mark-correct" type="checkbox" @click="markAsCorrect" :disabled="isCorrectAnswer"
                           :checked="isCorrectAnswer"/>
                </div>
            </div>
            <div v-else class="card-info general">
                <div class="new-block">
                    <div class="files">
                        <v-icon>sbf-attach</v-icon>
                        <span>1</span>
                    </div>
                    <ul class="users">
                        <li>
                            <img src="../../../helpers/img/user.png" class="avatar"/>
                            <v-icon>sbf-chat</v-icon>
                        </li>
                        <li>
                            <img src="../../../helpers/img/user.png" class="avatar"/>
                            <v-icon>sbf-chat</v-icon>
                        </li>
                    </ul>
                </div>

                <div class="answer" v-if="!myQuestion">
                    <button class="answer-btn">Answer</button>
                </div>
            </div>
        </div>

    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
