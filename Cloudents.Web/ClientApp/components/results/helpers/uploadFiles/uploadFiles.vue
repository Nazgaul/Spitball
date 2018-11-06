<template>
    <v-flex xs12>
          <v-icon class="close-upload-btn-icon"@click="closeDialog()" >sbf-close</v-icon>
            <v-card :class="['sb-steps-wrap', isFirstStep ? 'px-2' : '0' ]">
                <v-stepper v-model="currentStep" class="sb-stepper">
                    <v-stepper-header class="sb-stepper-header" v-show="currentStep===1">
                        <template>
                            <h2 class="sb-step-title"  v-language:inner>upload_files_header_title</h2>
                            <h4 class="sb-step-subtitle" v-language:inner>upload_files_header_subtitle</h4>
                        </template>
                    </v-stepper-header>
                    <v-stepper-items class="sb-stepper-item">

                        <v-stepper-content :class="['sb-stepper-content', `step-${n}`, n !==1 ? 'paddingTopSm': '']"  v-for="n in steps"
                                           :key="`${n}-content`"
                                           :step="n">
                            <!--upload steps rendering-->
                            <component :is="`upload-step_${n}`" :callBackmethods="callBackmethods"></component>
                        </v-stepper-content>
                        <div class="bottom-upload-controls" v-show="currentStep > 1">
                            <v-progress-linear
                                    :height="'3px'"
                                    v-show="currentStep >1"
                                    :color="'#4452fc'"
                                    v-model="stepsProgress"
                                    class="sb-steps-progress ma-0"
                                    :active="true">

                            </v-progress-linear>
                            <div class="step-controls">
                                <div class="upload upload-result-file">
                                    <div class="file-item">
                                        <v-icon v-if="!progressShow">sbf-terms</v-icon>
                                        <div v-else class="dot-flashing" ></div>
                                        <span class="upload-file-name ml-3 mr-3">{{getFileData.name}}</span>
                                        <v-icon class="sb-close">sbf-close</v-icon>
                                    </div>
                                </div>
                                <v-btn round v-if="currentStep > 2 && currentStep !==7" flat class="sb-back-flat-btn" @click="previousStep(step)">
                                    <v-icon left class="arrow-back">sbf-arrow-upward</v-icon>
                                    <span>Back</span>
                                </v-btn>
                                <v-btn v-show="currentStep !==6 && currentStep !==7" round class="next-btn" @click="nextStep(step)" :disabled="isDisabled">
                                <span v-language:inner>upload_files_btn_next</span>
                                </v-btn>

                                <v-btn v-show="currentStep ===6 && currentStep !==7" round class="next-btn sell" @click="sendDocumentData(step)" :disabled="isDisabled">
                                    <span v-language:inner>upload_files_btn_sell</span>

                                    <v-icon right class="credit-card">sbf-credit-card</v-icon>
                                </v-btn>
                                <v-btn v-show="currentStep ===7" flat class="sb-back-flat-btn" @click="closeDialog()">
                                    <span v-language:inner>upload_files_btn_close</span>
                                </v-btn>
                                <v-btn v-show="currentStep ===7" round outline class="another-doc" @click="changeStep(1)">
                                    <span v-language:inner>upload_files_btn_anotherUpload</span>
                                    <v-icon right class="cloud-upload">sbf-upload-cloud</v-icon>
                                </v-btn>
                            </div>
                        </div>
                    </v-stepper-items>
                </v-stepper>
            </v-card>
    </v-flex>
</template>
<script src="./uploadFiles.js">

</script>
<style lang="less" src="./uploadFiles.less">

</style>