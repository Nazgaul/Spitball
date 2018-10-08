<template>
    <div>
        <v-flex  :class="['result-cell', 'mb-3', 'static-card-need-to-ask', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']"
                v-scroll="transformToBtn()">
            <a class="mb-5 need-to-ask-router-link" @click="goToAskQuestion">
                <div :class="['need-to-ask', $vuetify.breakpoint.smAndDown  ? 'mobile' :  'desktop', isFloatingBtn ? 'floating-need-to-ask' : '']">
                    <!--<button class="ab-test-button hidden-sm-and-up floating-button" @click="goToAskQuestion" v-show="isFloatingBtn">-->
                    <!--<v-icon class="edit-icon">sbf-edit-icon</v-icon></button>-->
                    <div class="static-center">
                        <p class="static-ab-text">
                            Stuck with your homework?
                        </p>
                    </div>
                    <div :class="['static-bottom-block', $vuetify.breakpoint.smAndDown  ? 'mobile' :  'desktop', isFloatingBtn ? 'rounded-floating-static-bottom' : '']">
                        <user-avatar slot="activator" class="hidden-sm-and-down"
                                     :userName="userName"/>
                        <span class="static-question-text hidden-sm-and-down">Ask a question…</span>
                        <v-icon class="static-icon-right hidden-sm-and-down">sbf-edit-icon</v-icon>

                        <button :class="['ab-test-button', 'hidden-sm-and-up', isFloatingBtn ? 'rounded-floating-button' : '']"
                                @click="goToAskQuestion">
                            <v-icon :class="['edit-icon', isFloatingBtn ? 'float-icon' : '']">sbf-edit-icon</v-icon>
                            <span :class="['hidden-sm-and-up', 'btn-text', isFloatingBtn ? 'rounded-floating-button-text' : '']">Add a Question</span></button>
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

        <v-flex :class="['result-cell', 'mb-3', 'static-card-what-is-hw-question', isHiddenBlock  ? 'scrolledByHeader' : '']"
                xs-12 v-scroll="hideOnMobileScroll">
            <a class="mb-5 what-is-hw-question-router-link" @click="goToAskQuestion">
                <div class="what-is-hw-question">
                    <!--<div class="top-block static-top">-->
                    <!--<span class="static-user-name">{{userName}}</span>-->
                    <!--</div>-->
                    <div class="static-center" v-if="!isHiddenBlock">
                        <p class="static-ab-text">
                            Stuck with your homework?
                        </p>
                    </div>
                    <div class="static-bottom-block">
                        <user-avatar slot="activator"
                                     :userName="userName"/>
                        <span class="static-question-text">Ask a question…</span>
                        <v-icon class="static-icon-right">sbf-edit-icon</v-icon>
                    </div>
                </div>
            </a>
        </v-flex>
    </div>
</template>

<script src="./abTestCard.js"></script>

<style scoped lang="less" src="./abTestCard.less"></style>