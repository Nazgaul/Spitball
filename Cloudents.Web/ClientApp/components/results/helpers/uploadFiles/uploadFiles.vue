<template>
    <v-flex xs12>
        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
        <sb-dialog :showDialog="showUploadDialog" :transitionAnimation="transitionAnimation" :popUpType="'uploadDialog'" :fullWidth="true"
                   :isPersistent="true"
                   :content-class="'upload-dialog'">
          <v-icon class="close-upload-btn-icon"@click="showUploadDialog = false" >sbf-close</v-icon>
            <v-card :class="['sb-steps-wrap', isFirstStep ? 'px-2' : 'px-2' ]">
                <v-stepper v-model="currentStep" class="sb-stepper">
                    <v-stepper-header class="sb-stepper-header" v-show="currentStep===1">
                        <template>
                            <h2 class="sb-step-title">Ready, Set, Sale!</h2>
                            <h4 class="sb-step-subtitle">Make money of your study documents.</h4>
                        </template>
                    </v-stepper-header>
                    <v-stepper-items class="sb-stepper-item">

                        <v-stepper-content :class="['sb-stepper-content', `step-${n}`]"  v-for="n in steps"
                                           :key="`${n}-content`"
                                           :step="n">
                            <!--upload steps rendering-->
                            <component :is="`upload-step_${n}`" :callBackmethods="callBackmethods"></component>
                        </v-stepper-content>


                        <div class="bottom-upload-controls" v-show="currentStep > 1">
                            <v-progress-linear :height="'3px'" v-show="currentStep >1" :color="'#4452fc'" v-model="stepsProgress"
                                               class="sb-steps-progress ma-0" :active="true"></v-progress-linear>
                            <!--<div id="result"></div>-->
                            <div class="step-controls">
                                <div class="upload upload-result-file">
                                    <div class="file-item">
                                        <v-icon v-if="!progressShow">sbf-terms</v-icon>
                                        <div v-else class="dot-flashing" ></div>
                                        <span class="upload-file-name ml-5 mr-3">{{getFileData.name}}</span>
                                        <v-icon class="sb-close">sbf-close</v-icon>
                                    </div>
                                </div>
                                <v-btn round v-if="currentStep > 1 && currentStep !==8" flat class="sb-back-flat-btn" @click="previousStep(step)">
                                    <v-icon left class="arrow-back">sbf-arrow-upward</v-icon>
                                    <span>Back</span>
                                </v-btn>
                                <v-btn v-show="currentStep !==7 && currentStep !==8" round class="next-btn" @click="nextStep(step)" :disabled="isDisabled">Next</v-btn>
                                <v-btn v-show="currentStep ===7 && currentStep !==8" round class="next-btn sell" @click="sendDocumentData(step)" :disabled="isDisabled">
                                    SELL MY DOCUMENT
                                    <v-icon right class="credit-card">sbf-credit-card</v-icon>
                                </v-btn>
                                <v-btn v-show="currentStep ===8" flat class="sb-back-flat-btn" @click="showUploadDialog = false">Close
                                </v-btn>
                                <v-btn v-show="currentStep ===8" round outline class="another-doc" @click="changeStep(1)">Upload another document
                                    <v-icon right class="cloud-upload">sbf-upload-cloud</v-icon>
                                </v-btn>
                            </div>
                        </div>
                    </v-stepper-items>
                </v-stepper>
            </v-card>
        </sb-dialog>
    </v-flex>
</template>
<script src="./uploadFiles.js">

</script>
<style lang="less" src="./uploadFiles.less">

</style>