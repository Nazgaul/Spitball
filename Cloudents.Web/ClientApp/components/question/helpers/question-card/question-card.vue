<template>
    <v-flex v-if="cardData" xs12 class="question-card" :class="{'highlight':isCorrectAnswer||highlight}">
        <div v-if="!typeAnswer">
            <div class="q-price">
                <span>Earn ${{cardData.price}}</span> 
            </div>
            <!-- <p class="q-category">{{cardData.subject}}</p> -->
            <button v-if="myQuestion" :disabled="!cardData.answers" @click="()=>{haveAnswers?showDelete=true:deleteQuestion()}">Delete</button>
            <v-dialog  v-model="showDelete">
                {{dialogDeleteUserText}}
            </v-dialog>
        </div>
        
        <div v-else class="q-answer" >
            <user-block :user="cardData.user"></user-block>

            <button class="accept-btn" @click="markAsCorrect" :disabled="isCorrectAnswer||highlight" v-if="showApproveButton">
                <v-icon>sbf-check-circle</v-icon>
                <span>Accept</span>                
            </button>             
            
            <div class="choosen-answer" v-if="isCorrectAnswer||highlight">
                <span>Choosen Answer</span>
                <v-icon>sbf-check-circle</v-icon>
            </div>

            
            <button class="delete">
                <v-icon>sbf-trash</v-icon>        
            </button> 

        </div>


        

        <p class="q-text" :class="{'answer': typeAnswer}">{{cardData.text}}</p>

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
            <div  class="card-info general">
                <div class="new-block">
                    <div class="files">
                        <v-icon>sbf-attach</v-icon>
                        <span>1</span>
                    </div>
                    <div class="users">                        
                        <img src="../../../helpers/img/user.png" class="avatar"/>
                        <img src="../../../helpers/img/user.png" class="avatar"/>
                        <img src="../../../helpers/img/user.png" class="avatar"/>                        
                    </div>
                    <span class="user-counter">+5</span>
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
