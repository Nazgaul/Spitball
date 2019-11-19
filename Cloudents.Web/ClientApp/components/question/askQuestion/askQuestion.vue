<template>
    <transition name="fade">
        <div class="add-question-container">
            <v-form  v-model="validQuestionForm" ref="questionForm">
                <v-layout class="question-header">
                    <div class="mx-auto" v-language:inner="'addQuestion_title'"></div>
                </v-layout>

                <div class="question-textarea-container">
                    <div class="question-textarea-upper-part">
                        <div class="question-profile-data-container">
                            <user-avatar :userImageUrl="userImageUrl" :user-name="accountUser.name"></user-avatar>
                        </div>
                        <v-textarea
                                solo
                                no-resize
                                name="add-question-textarea"
                                :label="topicPlaceholder"
                                class="question-textarea elevation-0"
                                :rows="7"
                                :rules="[rules.required, rules.maximumChars, rules.minimumChars]"
                                v-model="questionText" :value="questionText"
                        ></v-textarea>
                    </div>
                </div>

                <div class="question-textarea-courses">
                    <fieldset class="question-fieldset px-2">
                        <legend v-language:inner="'addQuestion_class_placeholder'"/>
                        <v-combobox 
                            class="text-truncate pa-0"
                            flat
                            hide-no-data
                            :append-icon="''"
                            v-model="questionCourse"
                            :items="getSelectedClasses"
                            :rules="[rules.required]">
                        </v-combobox>
                    </fieldset>
                </div>
                
                <v-layout class="question-add-button-container pt-6 pb-4" :class="{'question-mobile': $vuetify.breakpoint.xsOnly}" align-center justify-center column>
                    <v-flex xs12 class="text-center">
                        <v-btn 
                            class="question-add-button font-weight-bold px-3 button_cancel" 
                            text 
                            @click="closeAddQuestionDialog">
                                <span v-language:inner="'addQuestion_cancel_button'"></span>
                        </v-btn>
                        <v-btn 
                            :loading="btnQuestionLoading"
                            text
                            class="question-add-button font-weight-bold px-3"
                            @click="submitQuestion">
                                <span v-language:inner="'addQuestion_add_button'"></span>
                        </v-btn>
                    </v-flex>
                    <v-flex xs12 v-if="errorMessage" class="error--text">{{errorMessage}}</v-flex>
                </v-layout>
            </v-form>
        </div>
    </transition>
</template>


<script src="./askQuestion.js"></script>

<style lang="less" src="./askQuestion.less"></style>