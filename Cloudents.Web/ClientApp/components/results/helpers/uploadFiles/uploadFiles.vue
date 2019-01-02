<template>
    <v-flex xs12>
            <v-icon class="close-upload-btn-icon" @click="confirmCloseOpen()">sbf-close</v-icon>
        <v-card :class="['sb-steps-wrap', isFirstStep ? 'px-2' : '0' ]">
            <v-stepper v-model="currentStep" class="sb-stepper">
                <v-stepper-header class="sb-stepper-header" v-show="currentStep===1">
                    <template>
                        <h2 class="sb-step-title" v-language:inner>upload_files_header_title</h2>
                        <!--<h4 class="sb-step-subtitle" v-language:inner>upload_files_header_subtitle</h4>-->
                    </template>
                </v-stepper-header>
                <v-stepper-items class="sb-stepper-item">
                    <v-stepper-content :class="['sb-stepper-content', `step-${n}`, n !==1 ? 'paddingTopSm': '']"
                                       v-for="n in steps"
                                       :key="`${n}-content`"
                                       :step="n">
                        <!--upload steps rendering-->
                        <component :is="`upload-step_${n}`" :docReferral="docReferral" :curStep="n" :callBackmethods="callBackmethods"></component>
                    </v-stepper-content>
                    <div class="bottom-upload-controls" v-show="currentStep > 1 && currentStep !==8">
                        <v-progress-linear
                                :height="'3px'"
                                v-show="currentStep >1"
                                :color="'#4452fc'"
                                v-model="stepsProgress"
                                class="sb-steps-progress ma-0"
                                :active="true">
                        </v-progress-linear>
                        <div :class="['step-controls', isLastStepAndMobile ? 'mobile-controls-last' : '' ]">
                            <div class="upload upload-result-file">
                                <div class="file-item">
                                    <v-icon v-if="!progressShow">sbf-document-note</v-icon>
                                    <div v-else class="load-container">
                                        <div class="dot-flashing"></div>
                                    </div>
                                    <span class="text-truncate upload-file-name mr-3">{{getFileData.name}}</span>
                                    <!--<v-icon class="sb-close">sbf-close</v-icon>-->
                                </div>
                            </div>
                            <!--Do not remove pseudo el for bnt centering-->
                            <div style="width: 96px; height: 36px; visibility: hidden; margin: 6px 8px;" v-if="currentStep === 2 && $vuetify.breakpoint.smAndUp"></div>
                            <v-btn round v-if="currentStep > 2 && currentStep !==8" flat class="sb-back-flat-btn"
                                   @click="previousStep(step)">
                                <v-icon left class="arrow-back">sbf-arrow-upward</v-icon>
                                <span v-language:inner>upload_files_btn_back</span>
                            </v-btn>
                            <v-btn v-show="currentStep !==7 && currentStep !==8" round class="next-btn"
                                   @click="nextStep(step)" :disabled="isDisabled">
                                <span v-language:inner>upload_files_btn_next</span>
                            </v-btn>

                            <v-btn v-show="currentStep ===7" round class="next-btn sell"
                                   @click="sendDocumentData(7)" :disabled="isDisabled">
                                <span v-language:inner>upload_files_btn_sell</span>

                                <v-icon right class="credit-card">sbf-credit-card</v-icon>
                            </v-btn>
                            <v-btn v-show="currentStep ===8" flat
                                   :class="['sb-back-flat-btn', $vuetify.breakpoint.smAndDown ? 'sb-close-mobile' : '']"
                                   @click="closeUpload()">
                                <span v-language:inner>upload_files_btn_close</span>
                            </v-btn>
                            <v-btn v-show="currentStep ===8" round outline class="another-doc" @click="changeStep(1)">
                                <span v-show="$vuetify.breakpoint.smAndDown" v-language:inner>upload_files_btn_anotherUpload_mobile</span>
                                <span v-show="$vuetify.breakpoint.smAndUp" v-language:inner>upload_files_btn_anotherUpload</span>

                                <v-icon right class="cloud-upload">sbf-upload-cloud</v-icon>
                            </v-btn>
                        </div>
                    </div>
                </v-stepper-items>
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
                    <!--<v-icon class="warning-icon mr-2">sbf-warning</v-icon>-->
                    <span v-language:inner>upload_confirm_stop_title</span>
                </v-card-title>
                <v-card-text class="confirm-text"><span v-language:inner>upload_confirm_text</span></v-card-text>
                <v-card-actions class="card-actions">
                    <v-btn round class="close-upload" @click.native="closeUpload()"><span v-language:inner>upload_confirm_btn_close</span>
                    </v-btn>
                    <v-btn round class="cancel" @click.native="confirmationDialog = false"><span v-language:inner>upload_confirm_btn_cancel</span>
                    </v-btn>
                </v-card-actions>
            </v-card>

        </sb-dialog>
    </v-flex>
</template>
<script src="./uploadFiles.js">

</script>
<style lang="less" src="./uploadFiles.less">

</style>