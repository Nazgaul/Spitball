<template>
    <div>
         <!-- !!!!!   card AAA   !!!!!-->
        <v-flex  :class="['result-cell', 'mb-3', 'static-card-need-to-ask', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']"
                v-scroll="transformToBtn()">
            <a class="mb-5 need-to-ask-router-link" @click="goToAskQuestion(2)">
                <div :class="['need-to-ask', $vuetify.breakpoint.smAndDown  ? 'mobile' :  'desktop', isFloatingBtn ? 'floating-need-to-ask' : '']">
                    <div class="static-center">
                        <p class="static-ab-text" v-language:inner>abTest_stuckWithHomework
                        </p>
                    </div>
                    <div :class="['static-bottom-block', $vuetify.breakpoint.smAndDown  ? 'mobile' :  'desktop', isFloatingBtn ? 'rounded-floating-static-bottom' : '']">
                        <user-avatar slot="activator" class="hidden-sm-and-down"
                                     :userName="userName"/>
                        <span class="static-question-text hidden-sm-and-down" v-language:inner>abTest_askQuestion</span>
                        <v-icon class="static-icon-right hidden-sm-and-down">sbf-edit-icon</v-icon>
                        <button :class="['ab-test-button', 'hidden-sm-and-up', isFloatingBtn ? 'rounded-floating-button' : '']"
                                @click="goToAskQuestion(3)">
                            <v-icon :class="['edit-icon', isFloatingBtn ? 'float-icon' : '']">sbf-edit-icon</v-icon>
                            <span :class="['hidden-sm-and-up', 'btn-text', isFloatingBtn ? 'rounded-floating-button-text' : '']" v-language:inner>abTest_addQuestion</span>
                        </button>
                    </div>
                </div>
            </a>
        </v-flex>
        <v-flex :class="['result-cell', 'mb-3', 'static-card-what-is-hw-question', isHiddenBlock  ? 'scrolledByHeader' : '']"
                xs-12 v-scroll="hideOnMobileScroll">
            <a class="mb-5 what-is-hw-question-router-link" @click="goToAskQuestion(isHiddenBlock ? 4 : 2)">
                <div class="what-is-hw-question">
                    <div class="static-center" v-if="!isHiddenBlock">
                        <p class="static-ab-text" v-language:inner>abTest_stuckWithHomework
                        </p>
                    </div>
                    <div class="static-bottom-block">
                        <user-avatar slot="activator"
                                     :userName="userName"/>
                        <span class="static-question-text" v-language:inner>abTest_askQuestion</span>
                        <v-icon class="static-icon-right">sbf-edit-icon</v-icon>
                    </div>
                </div>
            </a>
        </v-flex>
    </div>
</template>

<script src="./abTestCard.js"></script>

<style scoped lang="less" src="./abTestCard.less"></style>