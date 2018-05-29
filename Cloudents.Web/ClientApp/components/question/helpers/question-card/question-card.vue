<template>
    <v-flex v-if="cardData" xs12 class="question-card">

        <div v-if="!isAnswer">
            <div class="q-price">
                <span>Earn {{cardData.price}}$</span>
            </div>
            <p class="q-category">{{cardData.subject}}</p>
        </div>

        <div v-else class="q-answer">
            <p class="q-text"><strong>Answer:</strong></p>
            <div class="votes">
                <v-icon class="upvotes">sbf-chevron-down</v-icon>
                <span>3 Upvotes</span>
            </div>

        </div>

        <p class="q-text">{{cardData.text}}</p>

        <div class="q-user-info" v-if="cardData.user">
            <user-block :user="cardData.user"></user-block>
            <div v-if="myQuestion && isAnswer">
                <label for="mark-correct">Mark as correct answer</label>
                <input id="mark-correct" type="checkbox" value="apprvedAnswer"/>
            </div>
        </div>

        <div v-if="answerBtn" class="answer">
            <router-link class="answer-btn"  :to="{path:'/question/'+cardData.id}">Answer</router-link>
        </div>

    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
