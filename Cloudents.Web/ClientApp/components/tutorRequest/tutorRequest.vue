<template>
    <transition name="fade">
        <div class="add-topic-container">
            <v-layout class="request-header" row>
                <v-flex xs12 sm12 md12>
                    <h1 class="request-tutor-header-title" >Request a Tutor</h1>
                    <button class="back-button" @click="requestNewQuestionDialogClose()">
                        <v-icon right>sbf-close</v-icon>
                    </button>
                </v-flex>
            </v-layout>
            <div class="request-textarea-container" :class="{'textArea-error': hasTextAreaError}">
                <div class="request-textarea-floating-error" v-show="hasTextAreaError">
                    {{addQuestionValidtionObj.errors["textArea"].message}}
                    <span
                            class="textArea-error-triangle"
                    ></span>
                </div>
                <div class="request-textarea-upper-part">
                    <div class="request-profile-data-container">
                        <user-avatar :userImageUrl="userImageUrl" :user-name="accountUser.name"></user-avatar>
                    </div>
                    <v-textarea
                            solo
                            no-resize
                            name="add-request-textarea"
                            :label="dictionary.askPlaceholder"
                            class="request-textarea"
                            :rows="7"
                            v-model="questionMessage"
                    ></v-textarea>
                </div>
                <div class="request-textarea-lower-part">
                    <div class="request-options-part">
                        <!--<span v-show="!isMobile"><v-icon>sbf-equazion-i</v-icon></span>-->
                        <!--<span v-show="!isMobile"><v-icon>sbf-symbols-i</v-icon></span>-->
                        <span>
              <v-icon>sbf-upload-i</v-icon>
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
                      @input-filter="inputFilter"
              ></file-upload>
            </span>
                    </div>
                    <div
                            class="request-thumbnails-part"
                            :class="{'show-tumbnails': uploadProp.uploadedFiles.length > 0}"
                    >
                        <div
                                v-for="thumbnailBox in [0,1,2,3]"
                                :key="thumbnailBox"
                                class="request-attachment-box horizontal-border vertical-border"
                        >
                            <div
                                    @click="removeImage(uploadProp.populatedThumnbailBox['box_'+thumbnailBox])"
                                    class="request-thumb-close-container"
                                    :class="{'populated': uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated}"
                            >
                                <v-icon>sbf-close</v-icon>
                            </div>
                            <div class="request-thumb-plus-container">
                                <v-icon @click="openUploadInterface()">sbf-close</v-icon>
                            </div>
                            <div class="request-thumb-img-container">
                                <img
                                        v-show="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated"
                                        :src="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].src"
                                >
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="request-subject-class-container">
                <div class="request-select left" :class="{'subject-error': hasClassError}">
                    <v-select
                            :menu-props="{contentClass:'question-select-list'}"
                            height="40"
                            v-model="questionClass"
                            single-line
                            :items="getSelectedClasses"
                            :label="dictionary.classPlaceholder"
                            :append-icon="'sbf-arrow-down'"
                            outline
                    >
                        <template slot="no-data">
                            <div class="v-select-list v-card theme--light">
                                <div role="list" class="v-list theme--light">
                                    <div role="listitem">
                                        <div class="v-list__tile theme--light">
                                            <div class="v-list__tile__content">
                                                <div class="v-list__tile__title" v-language:inner>addQuestion_no_class
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

            <div class="request-add-button-container">
                <v-btn
                        :loading="addQuestionButtonLoading"
                        class="request-add-button"
                        @click="addQuestion()"
                >
                    <span v-language:inner>addQuestion_add_button</span>
                </v-btn>
            </div>
        </div>
    </transition>
</template>


<script src="./tutorRequest.js"></script>

<style lang="less" src="./tutorRequest.less"></style>