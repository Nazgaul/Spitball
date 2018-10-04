<template>
    <div>
    <v-flex class="result-cell mb-3 static-card-need-to-ask" xs-12>
        <a class="mb-5 need-to-ask-router-link"  @click="goToAskQuestion">
            <div class="need-to-ask">
                <div class="top-block static-top">
                    <user-avatar slot="activator"
                                 :user-name="userName"/>
                    <span class="static-ab-text">What do you need to ask?</span>
                </div>
                <div class="static-center">
                </div>
                <div class="static-bottom-block">
                    <button class="static-btn btn-subject">
                        <v-icon>sbf-books</v-icon>
                        Pick a subject
                    </button>
                    <button class="static-btn btn-price">
                        <v-icon>sbf-hand-coin</v-icon>
                        Place your price
                    </button>
                </div>
            </div>
        </a>
    </v-flex>

    <v-flex class="result-cell mb-3 static-card-what-is-question" xs-12>
        <a class="mb-5 what-is-question-router-link" @click="goToAskQuestion">
            <div class="what-is-question">
                <div class="top-block static-top">
                    <p class="static-ab-text">
                        What is<br/> your <b>homework<br/> question?</b>
                    </p>
                </div>
                <div class="static-bottom-block">
                    <askQuestionBtn class="static-ask-question"></askQuestionBtn>
                </div>
            </div>
        </a>
    </v-flex>

    <v-flex class="result-cell mb-3 static-card-what-is-hw-question" xs-12 >
        <a class="mb-5 what-is-hw-question-router-link" @click="goToAskQuestion">
            <div class="what-is-hw-question">
                <div class="top-block static-top">
                    <user-avatar slot="activator"
                                 :userName="userName"/>
                    <span class="static-user-name">{{userName}}</span>
                </div>
                <div class="static-center">
                    <p class="static-ab-text">
                        What is your HW question?
                    </p>
                </div>
            </div>
        </a>
    </v-flex>
    </div>
</template>

<script src="./abTestCard.js"></script>

<style scoped lang="less" src="./abTestCard.less"></style>