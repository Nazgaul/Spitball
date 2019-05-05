<template>
    <transition name="fade">
        <div class="add-request-container">
            <v-layout class="request-header px-2 py-2">
                <v-flex xs11 sm11 md11>
                    <span class="request-tutor-header-title">
                        <v-icon class="header-icon mr-2">sbf-person-icon</v-icon>
                        <span v-language:inner class="caption font-weight-bold">Request a Tutor</span>
                    </span>
                </v-flex>
                <v-flex xs1 sm1 md1 class="text-xs-right">
                    <button class="back-button" @click="tutorRequestDialogClose()">
                        <v-icon right>sbf-close</v-icon>
                    </button>
                </v-flex>
            </v-layout>
            <div class="request-textarea-container">
                <div class="request-textarea-upper-part">
                    <div class="request-profile-data-container">
                        <user-avatar :userImageUrl="userImageUrl" :user-name="accountUser.name"></user-avatar>
                    </div>
                    <v-textarea
                            solo
                            no-resize
                            name="add-request-textarea"
                            :label="'placeholder'"
                            class="request-textarea elevation-0"
                            :rows="3"
                            v-model="tutorRequestText"
                    ></v-textarea>
                </div>
                <div class="middle-part">
                    <div class="request-thumbnails-part"
                         :class="{'show-tumbnails': uploadProp.uploadedFiles.length > 0}">
                        <div
                                v-for="thumbnailBox in [0,1,2,3]"
                                :key="thumbnailBox"
                                class="request-attachment-box horizontal-border vertical-border">
                            <div
                                    @click="removeImage(uploadProp.populatedThumnbailBox['box_'+thumbnailBox])"
                                    class="request-thumb-close-container"
                                    :class="{'populated': uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated}">
                                <v-icon>sbf-close</v-icon>
                            </div>
                            <div class="request-thumb-plus-container">
                                <v-icon @click="openUploadInterface()">sbf-close</v-icon>
                            </div>
                            <div class="request-thumb-img-container">
                                <img v-show="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated"
                                     :src="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].src"
                                >
                            </div>
                        </div>
                    </div>
                </div>
                <div class="request-textarea-lower-part pt-2">
                    <div class="request-options-part">
                        <div class="request-subject-class-container">
                            <div class="request-select left">
                                <v-select
                                        :menu-props="{contentClass:'question-select-list'}"
                                        height="32"
                                        v-model="tutorCourse"
                                        single-line
                                        :items="getSelectedClasses"
                                        :label="'sdfsdf'"
                                        :append-icon="'sbf-arrow-down'"
                                        outline>
                                    <template slot="no-data">
                                        <div class="v-select-list v-card theme--light">
                                            <div role="list" class="v-list theme--light">
                                                <div role="listitem">
                                                    <div class="v-list__tile theme--light">
                                                        <div class="v-list__tile__content">
                                                            <div class="v-list__tile__title" v-language:inner>
                                                                addQuestion_no_class
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
                <span class="caption">Add</span>
                <span class="request-files-btn d-inline-flex align-center justify-center px-2 ml-3">
                <v-icon class="attach-icon mr-2">sbf-attach</v-icon>
                    <span>Attachment</span>
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
            <v-layout class="request-add-button-container pt-12 pb-3" align-center justify-center>
                <v-flex xs12 md12 sm12 class="text-xs-center">
                    <v-btn
                            class="request-add-button subheading font-weight-bold"
                            @click="requestTutor()"
                    >
                        <span>Submit Request</span>
                    </v-btn>
                </v-flex>

            </v-layout>
        </div>
    </transition>
</template>


<script src="./tutorRequest.js"></script>

<style lang="less" src="./tutorRequest.less"></style>