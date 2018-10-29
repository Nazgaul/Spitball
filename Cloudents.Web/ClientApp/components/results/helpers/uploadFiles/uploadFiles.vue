<template>
    <v-flex xs12>

        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
        <sb-dialog :showDialog="showUploadDialog" :popUpType="'uploadDialog'" :fullWidth="true"
                   :transition="'slide-y-transition'"
                   :content-class="'upload-dialog'">
            <v-card :class="['sb-steps-wrap', isFirstStep ? 'px-2' : 'px-0' ]">
                <!--<v-progress-linear v-model="progressValue" :active="progressShow"></v-progress-linear>-->
                <v-stepper v-model="e1" class="sb-stepper">
                    <v-stepper-header class="sb-stepper-header" v-show="e1===1">
                        <template>
                            <h2 class="sb-step-title">Ready, Set, Sale!</h2>
                            <h4 class="sb-step-subtitle">Make money of your study documents.</h4>
                        </template>
                    </v-stepper-header>
                    <v-stepper-items class="sb-stepper-item">
                        <!--step 1-->
                        <v-stepper-content class="sb-stepper-content step-one"
                                           :key="`${1}-content`"
                                           :step="1">
                            <v-card class="sb-step-card">
                                <!--dropshadow for drag and drop-->
                                <div class="upload-row-1">
                                    <v-icon>sbf-upload-cloud</v-icon>
                                    <h3 class="text-blue upload-cloud-text">Upload a Document</h3>
                                </div>
                                <div class="upload-row-2 paddingTopSm">
                                    <div class="btn-holder">
                                        <v-btn fab class="upload-option-btn" @click="DbFilesList()"
                                               :disabled="!dbReady">
                                            <v-icon>sbf-upload-dropbox</v-icon>
                                        </v-btn>
                                        <span class="btn-label">DropBox</span>
                                    </div>
                                    <div class="btn-holder">
                                        <!--<div class="desktop-upload-btn">-->
                                        <v-btn fab class="upload-option-btn">
                                            <v-icon>sbf-upload-desktop</v-icon>
                                            <file-upload
                                                    class="upload-input"
                                                    ref="upload"
                                                    :drop="true"
                                                    v-model="files"
                                                    post-action="/api/upload/file"
                                                    chunk-enabled
                                                    :extensions="['doc', 'pdf', 'png', 'jpg']"
                                                    :maximum="1"
                                                    @input-file="inputFile"
                                                    @input-filter="inputFilter"
                                                    :chunk="{
                              action: '/api/upload/file',
                              minSize: 1048576,
                              maxActive: 3,
                              maxRetries: 5,}">
                                            </file-upload>
                                        </v-btn>
                                        <span class="btn-label">Your Dekstop</span>
                                    </div>
                                </div>
                                <div class="upload-row-3">
                                    <div :class="['btn-holder', $refs.upload && $refs.upload.dropActive ? 'drop-active' : '' ]">
                                        <v-icon>sbf-upload-drag</v-icon>
                                        <span class="btn-label">Or just drop your file here</span>
                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>

                        <!--step 2-->
                        <v-stepper-content class="sb-stepper-content step-two mt-5"
                                           :key="`${2}-content`"
                                           :step="2">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <h3 class="upload-cloud-text sb-title">Awesome! get your document ready for
                                        sale</h3>
                                    <h4 class="sb-subtitle mt-2">Filling details will increase this doc. chances to get
                                        sold</h4>
                                </div>
                                <div class="upload-row-2 paddingTopSm">
                                    <div class="btn-holder">
                                        <label :for="'school'" class="steps-form-label school mb-2">
                                            <v-icon class="mr-1">sbf-university</v-icon>
                                            <span>School</span></label>
                                        <sb-input :bottomError="true"
                                                  v-model="schoolName" placeholder="Your School" name="school"
                                                  type="text"
                                                  :autofocus="true" @keyup.enter.native="">
                                        </sb-input>
                                    </div>
                                    <div class="btn-holder">
                                    </div>
                                </div>
                                <div class="upload-row-3">
                                    <label :for="'class-chip'" class="steps-form-label mb-2">
                                        <v-icon class="mr-1">sbf-classes</v-icon>
                                        <span>Select a Relevant Class</span>
                                    </label>
                                    <div class="chip-wrap">
                                        <v-chip name="sbf-class-chip" class="sbf-class-chip mb-2" outline
                                                v-for="(singleClass, index) in classesList"
                                                @click="updateClass(singleClass)"
                                                :selected="selectedClass ===singleClass"
                                                :key="index">{{singleClass}}
                                        </v-chip>
                                    </div>
                                </div>

                            </v-card>
                        </v-stepper-content>


                        <!--step 3-->
                        <v-stepper-content class="sb-stepper-content step-three mt-5"
                                           :key="`${3}-content`"
                                           :step="3">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <h3 class="sb-title">Choose document type, it would be easier to
                                        find it</h3>
                                </div>
                                <div class="upload-row-2">
                                    <div :class="['sb-doc-type', singleType.title === selectedDoctype.title ? 'selected': '']"
                                         v-for="(singleType, index) in documentTypes"
                                         :key="singleType.id"
                                         @click="updateDocumentType(singleType)">
                                        <v-icon class="sb-doc-icon mt-2">{{singleType.icon}}</v-icon>
                                        <span class="sb-doc-title mb-2">
                                           {{singleType.title}}
                                       </span>
                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>

                        <!--step 4-->
                        <v-stepper-content class="sb-stepper-content step-four mt-5"
                                           :key="`${4}-content`"
                                           :step="4">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <h3 class="upload-cloud-text sb-title">Create a title or use the one we created for
                                        you</h3>
                                </div>
                                <div class="upload-row-2 paddingTopSm">
                                    <div class="btn-holder">
                                        <label :for="'school'" class="steps-form-label school mb-2">
                                            <!--<v-icon class="mr-1">sbf-university</v-icon>-->
                                            <span>Document Title</span></label>
                                        <sb-input :bottomError="true"
                                                  v-model="documentTitle" placeholder="title" name="document title"
                                                  type="text"
                                                  :autofocus="true">
                                        </sb-input>
                                    </div>
                                    <div class="btn-holder ml-0 ">

                                    </div>
                                </div>
                                <div class="upload-row-3">
                                    <label :for="'school'" class="steps-form-label school mb-2">
                                        <!--<v-icon class="mr-1">sbf-university</v-icon>-->
                                        <span>Professor Name</span></label>
                                    <sb-input :bottomError="true"
                                              v-model="proffesorName" placeholder="proffesor name" name="proffesor"
                                              type="text"
                                              :autofocus="true">
                                    </sb-input>

                                </div>
                            </v-card>
                        </v-stepper-content>


                        <!--step 5-->
                        <v-stepper-content class="sb-stepper-content step-five mt-5"
                                           :key="`${5}-content`"
                                           :step="5">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <h3 class="upload-cloud-text sb-title">Add any tag you think is relevant for this
                                        doc.</h3>
                                    <h4 class="sb-subtitle mt-2">It will help other to find you in more than one
                                        way</h4>
                                </div>
                                <div class="upload-row-2 paddingTopSm">
                                    <div class="btn-holder">
                                        <label :for="'school'" class="steps-form-label school mb-2">
                                            <v-icon class="mr-1">sbf-tag-icon</v-icon>
                                            <span>Tags</span></label>
                                        <v-combobox class="sb-combo"
                                                    v-model="selectedTags"
                                                    height="'48px'"
                                                    append-icon=""
                                                    prepend-icon=""
                                                    placeholder="Type a tag name"
                                                    color="'#979797'"
                                                    multiple
                                                    chips
                                                    solo
                                                    :allow-overflow="false">
                                            <template slot="selection" slot-scope="data" class="sb-selection">
                                                <v-chip class="sb-chip-tag">
                                                   <span class="chip-button px-2">
                                                       {{!!data.item ? data.item : ''}}
                                                   </span>
                                                    <v-icon class="chip-close ml-3" @click="removeTag(data.item)">
                                                        sbf-close
                                                    </v-icon>
                                                </v-chip>
                                            </template>
                                        </v-combobox>
                                    </div>
                                    <div class="btn-holder">
                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>

                        <!--step 6-->
                        <v-stepper-content class="sb-stepper-content step-six mt-5"
                                           :key="`${6}-content`"
                                           :step="6">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <h3 class="upload-cloud-text sb-title">Set your price</h3>
                                    <h4 class="sb-subtitle mt-2">In spitball platform You can buy and sell with SBL’s.
                                        <a href="" class="sbl-faq">What are SBL’s? </a></h4>
                                </div>
                                <div class="upload-row-2 paddingTopSm">
                                    <div class="btn-holder">
                                        <label :for="'school'" class="steps-form-label school mb-2">
                                            <v-icon class="mr-1">sbf-tag-icon</v-icon>
                                            <span>Price</span></label>
                                        <div class="wrap-row-price">
                                            <div class="price-wrap">
                                                <sbl-currency v-model="uploadPrice"
                                                              class="sb-input-upload-price"></sbl-currency>
                                                <div class="sbl-suffix">SBL</div>

                                                <span class="balance-label" v-show="uploadPrice">Equals to {{uploadPrice | dollarVal}}$</span>

                                            </div>
                                            <div class="sb-current-balance">
                                                <v-icon class="sb-wallet-icon">sbf-wallet-new</v-icon>
                                                <span name="account-balance" class="sb-account-amount">{{accountUser.balance | currencyLocalyFilter}}</span>
                                                <span class="balance-label">Your balance</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="btn-holder">

                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>
                        <!--step 7-->
                        <v-stepper-content class="sb-stepper-content step-seven mt-5"
                                           :key="`${7}-content`"
                                           :step="7">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <v-icon class="five">sbf-five</v-icon>
                                    <h3 class="sb-title">You Rock! you finished sooo fast! </h3>
                                    <h4 class="sb-subtitle mt-2">Please make sure all the details are just right before sale</h4>
                                </div>
                                <div class="upload-row-2 final-row " style="padding-top: 32px;" @click="changeStep(2)">
                                    <div class="final-item school" >
                                        <div class="edit">
                                            <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                            <span>Edit</span>
                                        </div>
                                        <div class="final-item-wrap">
                                            <v-icon class="final-icon">sbf-university</v-icon>
                                            <span class="item-name">School</span>
                                        </div>
                                        <div>
                                            <p class="school-name">{{schoolName}}</p>
                                        </div>

                                    </div>
                                    <div class="final-item class-selected">
                                        <div class="edit">
                                            <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                            <span>Edit</span>
                                        </div>
                                        <div class="final-item-wrap">
                                            <v-icon class="final-icon">sbf-classes-new</v-icon>
                                            <span class="item-name">Class</span>
                                        </div>
                                        <span class="class-name">{{selectedClass}}</span>
                                    </div>
                                </div>
                                <div class="upload-row-3 final-row">
                                    <div class="final-item doc-type-selected"  @click="changeStep(3)">
                                        <div class="edit">
                                            <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                            <span>Edit</span>
                                        </div>
                                        <div class="final-item-wrap doc-type-wrap">
                                            <v-icon class="final-icon doc-type">{{selectedDoctype.icon}}</v-icon>
                                            <span class="item-name doc-type-name">{{selectedDoctype.title}}</span>
                                        </div>

                                    </div>
                                    <div class="final-item tags-selected" @click="changeStep(5)">
                                        <div class="edit">
                                            <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                            <span>Edit</span>
                                        </div>
                                        <div class="final-item-wrap mb-1">
                                            <v-icon class="final-icon tags">sbf-tag-icon</v-icon>
                                            <span class="item-name">Tags</span>

                                        </div>
                                        <div class="sb-combo final-tags">                                       
                                            <v-chip class="sb-chip-tag" v-for="tag in selectedTags">
                                                   <span class="chip-button px-1">
                                                       {{tag}}
                                                   </span>                                             
                                            </v-chip>
                                        </div>
                                    </div>

                                </div>
                                <div class="upload-row-4 final-row">
                                    <div class="legal-wrap">
                                    <input type="checkbox" class="legal-input" id="legal-ownership" v-model="legalCheck" name="legalyOwn" @change="updateLegal()"/>
                                    <label for="legal-ownership" class="ml-3 legal-ownership">I legally own this document, and all its legal rights</label>
                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>
                        <!--step 7-->
                        <v-stepper-content class="sb-stepper-content step-eight mt-5"
                                           :key="`${8}-content`"
                                           :step="8">
                            <v-card class="mb-5 sb-step-card">
                                <div class="upload-row-1">
                                    <v-icon class="five">sbf-spread-loud</v-icon>
                                    <h3 class="sb-title">Spread the Word</h3>
                                </div>
                                <div class="upload-row-2 paddingTopSm referral-row">
                                    <referral-dialog :popUpType="''" ></referral-dialog>
                                </div>


                            </v-card>
                        </v-stepper-content>

                        <div class="bottom-upload-controls" v-show="e1 > 1">
                            <v-progress-linear :height="'3px'" v-show="e1 >1" :color="'#4452fc'" v-model="stepsProgress"
                                               class="sb-steps-progress ma-0" :active="true"></v-progress-linear>
                            <!--<v-btn block color="primary"-->

                            <!--class="ask_btn">{{files.length >= 1 ? 'D Upload more' : 'Dropbox'}}-->
                            <!--</v-btn>-->
                            <!--<div id="result"></div>-->
                            <div class="step-controls">
                                <div class="upload upload-result-file">
                                    <div class="file-item" v-for="file in files">
                                        <v-icon>sbf-terms</v-icon>
                                        <span class="upload-file-name ml-4 mr-3">{{file.name}}</span>
                                        <v-icon class="sb-close">sbf-close</v-icon>
                                        <!-- - Error: {{file.error}}, Success: {{file.success}}-->
                                    </div>
                                    <!--<span v-show="$refs.upload && $refs.upload.uploaded">All files have been uploaded</span>-->
                                </div>
                                <v-btn round v-if="e1 > 1" flat class="sb-back-flat-btn" @click="previousStep(step)">
                                    <v-icon left class="arrow-back">sbf-arrow-upward</v-icon>
                                    <span>Back</span>
                                </v-btn>
                                <v-btn v-show="e1 !==7" round class="next-btn" @click="nextStep(step)" :disabled="isDisabled">Next</v-btn>
                                <v-btn v-show="e1 ===7" round class="next-btn sell" @click="sendDocumentData(step)" :disabled="isDisabled">SELL MY DOCUMENT
                                    <v-icon class="credit-card">sbf-credit-card</v-icon>
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