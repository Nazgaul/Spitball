<template>
    <v-card>
        <div class="dialog-wrapp">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text">
                <h2 class="text-md-left" v-language:inner>questionSuggestPopUp_Great_Job</h2>
                <p>
                    <span v-language:inner>questionSuggestPopUp_Lets_wait_for</span><span class="user-name"> {{ user.name}}</span> <span v-language:inner>questionSuggestPopUp_accepts_your_answer</span>
                </p>
                <p>
                   <span v-language:inner>questionSuggestPopUp_below_question_should_help</span> 
                </p>
            </div>
            <v-carousel :interval="interval" v-if="cardList"
                        delimiter-icon="sbf-circle-solid"
                        prev-icon="sbf-arrow-left-carousel"
                        next-icon="sbf-arrow-right-carousel">

                <v-carousel-item  v-for="(item,i) in cardList" src="" :key="i"  @click.native="answerMore(item.id)">
                        <question-card :fromCarousel="true" :cardData="item" :typeAnswer="typeAnswer" :suggestion="true" class="user-question mb-1 mt-1" detailed-view ></question-card>
                </v-carousel-item>
            </v-carousel>
        </div>
    </v-card>

</template>

<script src="./questionSuggestPopUp.js"></script>

<style src="./questionSuggestPopUp.less" lang="less"></style>
