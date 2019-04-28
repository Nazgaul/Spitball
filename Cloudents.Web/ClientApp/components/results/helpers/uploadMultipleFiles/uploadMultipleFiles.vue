<template>
    <v-flex xs12>
        <v-icon class="close-upload-btn-icon" @click="confirmCloseOpen()">sbf-close</v-icon>
        <v-card elevation="0" class="sb-steps-wrap">
            <v-stepper v-model="currentStep" class="sb-stepper">
                <v-stepper-header class="sb-stepper-header px-4">
                    <template>
                        <h2 v-show="$vuetify.breakpoint.smAndUp" class="sb-step-title" v-language:inner>
                            upload_multiple_files_header_title</h2>
                        <div v-show="!$vuetify.breakpoint.smAndUp"
                             style="width: 100%; display: flex; align-items: center; justify-content: center;">
                            <v-icon class="col-blue mr-4">sbf-upload-cloud</v-icon>
                            <span class="upload-subtitle col-blue"
                                  v-language:inner>upload_multiple_label_icon_text</span>
                        </div>
                    </template>
                </v-stepper-header>
                <v-stepper-items class="sb-stepper-item">
                    <v-stepper-content :class="['sb-stepper-content', `step-${n}`]"
                                       v-for="n in steps"
                                       :key="`${n}-content`"
                                       :step="n">
                        <v-layout justify-center column wrap align-center v-if="firstStep"
                                  :class="[{'mobile-view-layout mt-0': $vuetify.breakpoint.xsOnly}, 'mt-4']">
                            <v-flex xs12 sm6 row class="justify-center align-center mb-3 grow-1"
                                    v-show="$vuetify.breakpoint.smAndUp">
                                <v-icon class="col-blue mr-4">sbf-upload-cloud</v-icon>
                                <span class="upload-subtitle col-blue"
                                      v-language:inner>upload_multiple_label_icon_text</span>
                            </v-flex>
                            <v-flex xs12 sm6 d-flex :class="[{'px-2': $vuetify.breakpoint.xsOnly}]"
                                    class="justify-center align-center">
                                <v-select
                                        class="course-select custom-select elevation-0"
                                        :items="classesList"
                                        :placeholder="selectCoursePlaceholder"
                                        v-model="courseSelected"
                                        @input="updateSelectedCourse()"
                                        solo
                                        :append-icon="'sbf-arrow-down'"
                                ></v-select>
                            </v-flex>
                        </v-layout>
                        <upload-files-start
                                :class="[courseSelected && firstStep ? 'visibilityVisible' : 'visibilityHidden', {'slim': !firstStep}]"
                                :curStep="1"
                                :callBackmethods="callBackmethods"></upload-files-start>
                        <transition name="slide">
                            <uploadStep_2 v-show="n===2" :curStep="2" :callBackmethods="callBackmethods"></uploadStep_2>
                        </transition>
                        <transition name="slide">
                            <uploadStep_3
                                    v-if="n===3"
                                    :curStep="3"
                                    :fileSnackbar="fileSnackbar"
                                    :referralLinks="docReferral"
                                    :callBackmethods="callBackmethods"
                                    :showError="showError"
                                    :errorText="errorText"></uploadStep_3>
                        </transition>
                    </v-stepper-content>
                </v-stepper-items>
                <v-stepper-header v-show="courseSelected && !lastStep" class="sb-stepper-header footer px-2"
                                  :class="{'slim': firstStep || lastStep || isEdge}">
                    <v-flex xs12 sm12 md12 class="text-xs-center" v-show="!firstStep && !lastStep">
                        <span class="caption mb-1 legal-text" v-language:inner>upload_multiple_legal_text</span>
                    </v-flex>
                    <v-flex xs12 sm12 md12 v-show="!firstStep && !lastStep" class="text-xs-center">
                        <v-btn :loading="loading" class="upload-btn d-inline-flex" :disabled="!isLoaded || disableBtn"
                               @click="sendDocumentData()">
                            <span v-show="isLoaded" class="sb-btn-text" v-language:inner>upload_multiple_btn_upload</span>
                            <span v-show="!isLoaded"  class="sb-btn-text" v-language:inner>upload_multiple_btn_uploading</span>
                        </v-btn>
                    </v-flex>

                </v-stepper-header>
            </v-stepper>
        </v-card>
        <sb-dialog :showDialog="confirmationDialog"
                   :popUpType="'confirmationDialog'"
                   :isPersistent="true"
                   :activateOverlay="true"
                   :content-class="'confirmation-dialog'">
            <v-card class="confirm-card">
                <span class="warning-icon">!</span>
                <v-card-title class="confirm-headline">
                    <span v-language:inner>upload_multiple_confirm_stop_title</span>
                </v-card-title>
                <v-card-text class="confirm-text"><span v-language:inner>upload_multiple_confirm_text</span>
                </v-card-text>
                <v-card-actions class="card-actions">
                    <v-btn round class="close-upload" @click.native="closeUpload()"><span v-language:inner>upload_multiple_confirm_btn_close</span>
                    </v-btn>
                    <v-btn round class="cancel" @click.native="confirmationDialog = false"><span v-language:inner>upload_multiple_confirm_btn_cancel</span>
                    </v-btn>
                </v-card-actions>
            </v-card>

        </sb-dialog>
    </v-flex>
</template>
<script src="./uploadMultipleFiles.js"></script>

<style lang="less" src="./uploadMultipleFiles.less"></style>