<template>
    <v-flex xs12>
        <v-icon class="close-upload-btn-icon" @click="confirmCloseOpen()">sbf-close</v-icon>
        <v-card elevation="0" class="sb-steps-wrap">
            <v-stepper v-model="currentStep" class="sb-stepper">
                <v-stepper-header class="sb-stepper-header px-4">
                    <template>
                        <h2 class="sb-step-title" v-language:inner>upload_multiple_files_header_title</h2>
                    </template>
                </v-stepper-header>
                <v-stepper-items class="sb-stepper-item">
                    <v-stepper-content :class="['sb-stepper-content', `step-${n}`]"
                                       v-for="n in 2"
                                       :key="`${n}-content`"
                                       :step="n">
                        <v-layout justify-center column wrap align-center v-if="firstStep" class="mt-4">
                            <v-flex xs12 sm6 d-flex row class="justify-center align-center mb-3 grow-1">
                                <v-icon class="col-blue mr-4">sbf-upload-cloud</v-icon>
                                <span class="upload-subtitle col-blue" v-language:inner>upload_multiple_label_icon_text</span>
                            </v-flex>
                            <v-flex xs12 sm6 d-flex class="justify-center align-center">
                                <v-select
                                        class="course-select custom-select elevation-0"
                                        :items="classesList"
                                        placeholder="Please select course"
                                        v-model="courseSelected"
                                        @input="updateSelectedCourse()"
                                        solo
                                        :append-icon="'sbf-arrow-down'"
                                ></v-select>
                            </v-flex>
                        </v-layout>
                        <transition name="slide">
                            <upload-files-start
                                    v-show="courseSelected && firstStep"
                                    :curStep="n"
                                    :callBackmethods="callBackmethods"></upload-files-start>
                        </transition>
                        <uploadStep_2 v-show="n===2"  :curStep="n"  :callBackmethods="callBackmethods"></uploadStep_2>
                        <ulpoadStep_3 v-show="n===3"  :curStep="n"  :callBackmethods="callBackmethods"></ulpoadStep_3>
                        <!--<component-->
                                <!--v-if="!firstStep"-->
                                <!--:is="`upload-step_${n}`"-->
                                <!--:curStep="n"-->
                                <!--:callBackmethods="callBackmethods"></component>-->
                    </v-stepper-content>
                </v-stepper-items>
                <v-stepper-header v-show="courseSelected" class="sb-stepper-header footer px-2">
                        <v-flex v-show="!firstStep">
                            <v-btn class="upload-btn" :disabled="!isLoaded" @click="sendDocumentData()">
                                <span class="sb-btn-text" v-language:inner>upload_multiple_btn_upload</span>
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
                <v-card-text class="confirm-text"><span v-language:inner>upload_multiple_confirm_text</span></v-card-text>
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