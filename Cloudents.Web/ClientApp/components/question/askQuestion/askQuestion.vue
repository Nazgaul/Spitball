<template>
    <transition name="fade">
        <div class="add-question-container ">
            <v-form  v-model="validQuestionForm" ref="questionForm">
            <v-layout class="question-header py-2" :class="[$vuetify.breakpoint.xsOnly ? 'px-4' : 'px-2']">
                <v-flex xs11 sm11 md11>
                    <span class="question-header-title">
                        <v-icon class="header-icon mr-2">sbf-person-icon</v-icon>
                        <span v-language:inner class="caption font-weight-bold">addQuestion_title</span>
                    </span>
                </v-flex>
                <v-flex xs1 sm1 md1 class="text-xs-right">
                    <button class="back-button" @click="requestAskClose()">
                        <v-icon right>sbf-close</v-icon>
                    </button>
                </v-flex>
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
                            :rows="3"
                            :rules="[rules.required, rules.maximumChars, rules.minimumChars]"
                            v-model="questionText"
                    ></v-textarea>
                </div>
                <div class="middle-part">
                    <div class="question-thumbnails-part"
                         :class="{'show-tumbnails': uploadProp.uploadedFiles.length > 0}">
                        <div
                                v-for="thumbnailBox in [0,1,2,3]"
                                :key="thumbnailBox"
                                class="question-attachment-box horizontal-border vertical-border">
                            <div
                                    @click="removeImage(uploadProp.populatedThumnbailBox['box_'+thumbnailBox])"
                                    class="question-thumb-close-container"
                                    :class="{'populated': uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated}">
                                <v-icon>sbf-close</v-icon>
                            </div>
                            <div class="question-thumb-plus-container">
                                <v-icon @click="openUploadInterface()">sbf-close</v-icon>
                            </div>
                            <div class="question-thumb-img-container">
                                <img v-show="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated"
                                     :src="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].src"
                                >
                            </div>
                        </div>
                    </div>
                </div>
                <div class="question-textarea-lower-part pt-2">
                    <div class="question-options-part">
                        <div class="question-subject-class-container">
                            <div class="question-select left">
                                <v-select
                                        :menu-props="{contentClass:'courses-select-list'}"
                                        height="32"
                                        v-model="questionCourse"
                                        :items="getSelectedClasses"
                                        :placeholder="coursePlaceholder"
                                        :rules="[rules.required]"
                                        :append-icon="'sbf-arrow-down'"
                                         outline>
                                    <template slot="no-data">
                                        <div class="v-select-list v-card theme--light">
                                            <div role="list" class="v-list theme--light">
                                                <div role="listitem">
                                                    <div class="v-list__tile theme--light">
                                                        <div class="v-list__tile__content">
                                                            <div class="v-list__tile__title" v-language:inner>
                                                                tutorRequest_no_course
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </template>
                                </v-select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="add-files px-3 mt-3 pt-1 pb-3">
                <div class="btn-upload-wrap d-flex align-center justify-center">
                    <span class="caption" v-language:inner>tutorRequest_label_add</span>
                    <span class="question-files-btn d-inline-flex align-center justify-center pl-2 pr-3 ml-3">
                <v-icon class="attach-icon mr-2">sbf-attach</v-icon>
                    <span v-language:inner>tutorRequest_btn_attachment</span>
                <file-upload
                        id="file-input"
                        :input-id="uploadProp.componentUniqueId"
                        ref="upload"
                        :drop="false"
                        v-model="uploadProp.uploadedFiles"
                        :multiple="true"
                        :maximum="uploadProp.MAX_FILES_AMOUNT"
                        :post-action="uploadProp.uploadUrl"
                        accept="image/*"
                        :extensions="uploadProp.extensions"
                        @input-file="inputFile"
                        @input-filter="inputFilter">
                </file-upload>
                </span>
                </div>
            </div>
            <v-layout class="question-add-button-container pt-12 pb-3" align-center justify-center column>
                <v-flex xs12 md12 sm12 class="text-xs-center">
                    <v-btn :loading="btnQuestionLoading"
                           class="question-add-button subheading font-weight-bold px-3"
                           @click="submitQuestion()">
                        <span v-language:inner>addQuestion_add_button</span>
                    </v-btn>
                </v-flex>
                <v-flex xs12 md12 sm12 v-if="errorMessage" class="error--text">{{errorMessage}}</v-flex>

            </v-layout>
            </v-form>
        </div>
    </transition>
</template>


<script src="./askQuestion.js"></script>

<style lang="less" src="./askQuestion.less"></style>