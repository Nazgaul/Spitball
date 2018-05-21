<template>
    <v-flex xs12 class="question-card">

        <div v-if="!isAnswer">
            <div class="q-price">
                <span>Earn {{cardData.price}}$</span>
            </div>         
            <p class="q-category">{{cardData.subject}}</p>
        </div>

        <div v-else class="q-answer">
            <p class="q-text"><strong >Answer:</strong></p>
            <div class="votes">
                <v-icon  class="upvotes">sbf-chevron-down</v-icon>
                <span>3 Upvotes</span>
            </div>
            
        </div>
             
        <p class="q-text">{{cardData.text}}</p>
        
        <div class="q-user-info" v-if="!myQuestion && cardData.user">
            <img src="./../../ask/img/student_ask.png" class="avatar" alt="" />
            <span><a href="/user">{{cardData.user.name}}</a> · 1 hour ago </span>
        </div>

        <div v-else class="answer">
            <a class="answer-btn" href="/askquestion"></a>
        </div>

    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
