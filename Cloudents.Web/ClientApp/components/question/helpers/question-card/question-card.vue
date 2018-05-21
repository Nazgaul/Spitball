<template>
    <v-flex xs12 class="question-card">

        <div v-if="!isAnswer">
            <div class="q-price">
                <span>$5</span>
            </div>         
            <p class="q-category">Algebra</p>  
        </div>

        <div v-else class="q-answer">
            <p class="q-text"><strong >Answer:</strong></p>
            <div class="votes">
                <v-icon  class="upvotes">sbf-chevron-down</v-icon>
                <span>3 Upvotes</span>
            </div>
            
        </div>
             
        <p class="q-text">A triangle has a perimeter of 50. If two of the side lengths are equal and the third side is five more than the equal sides, what is the length of  the third side?</p>     
        
        <div class="q-user-info" v-if="!myQuestion">
            <img src="./../../ask/img/student_ask.png" class="avatar" alt="" />
            <span><a href="/user">Irma Rogers</a> · 1 hour ago </span>
        </div>

        <div v-else class="answer">
            <a class="answer-btn" href="/askquestion">Answer</a>
        </div>

    </v-flex>
</template>

<style src="./question-card.less" lang="less"></style>
<script src="./question-card.js"></script>
