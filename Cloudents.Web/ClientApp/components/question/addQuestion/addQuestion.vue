<template>
  <transition name="fade">
    <div class="add-question-container">
      <div class="question-header">
        <h1 class="question-header-title" v-language:inner>addQuestion_title</h1>
        <button class="back-button" @click="requestNewQuestionDialogClose()">
          <v-icon right>sbf-close</v-icon>
        </button>
      </div>
      <div class="question-textarea-container" :class="{'textArea-error': hasTextAreaError}">
        <div class="question-textarea-floating-error" v-show="hasTextAreaError">
          {{addQuestionValidtionObj.errors["textArea"].message}}
          <span
            class="textArea-error-triangle"
          ></span>
        </div>
        <div class="question-textarea-upper-part">
          <div class="question-profile-data-container">
            <user-avatar :userImageUrl="userImageUrl" :user-name="accountUser.name"></user-avatar>
          </div>
          <v-textarea
            solo
            no-resize
            name="add-question-textarea"
            :label="dictionary.askPlaceholder"
            class="question-textarea"
            :rows="7"
            v-model="questionMessage"
          ></v-textarea>
        </div>
        <div class="question-textarea-lower-part">
          <div class="question-options-part">
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
            class="question-thumbnails-part"
            :class="{'show-tumbnails': uploadProp.uploadedFiles.length > 0}"
          >
            <div
              v-for="thumbnailBox in [0,1,2,3]"
              :key="thumbnailBox"
              class="question-attachment-box horizontal-border vertical-border"
            >
              <div
                @click="removeImage(uploadProp.populatedThumnbailBox['box_'+thumbnailBox])"
                class="question-thumb-close-container"
                :class="{'populated': uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated}"
              >
                <v-icon>sbf-close</v-icon>
              </div>
              <div class="question-thumb-plus-container">
                <v-icon @click="openUploadInterface()">sbf-close</v-icon>
              </div>
              <div class="question-thumb-img-container">
                <img
                  v-show="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated"
                  :src="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].src"
                >
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="question-subject-class-container">
        <div class="question-select-floating-error" v-show="hasClassError">
          {{addQuestionValidtionObj.errors["class"].message}}
          <span class="select-error-triangle"></span>
        </div>
        <div class="question-select left" :class="{'subject-error': hasClassError}">
          
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
                        <div class="v-list__tile__title" v-language:inner>addQuestion_no_class</div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </template>
          </v-select>
        </div>
        <div class="question-select right">
          <v-select
            :menu-props="{contentClass:'question-select-list'}"
            height="40"
            v-model="questionSubjct"
            single-line
            item-value="id"
            item-text="subject"
            :items="subjectList"
            :label="dictionary.selectSubjectPlaceholder"
            :append-icon="'sbf-arrow-down'"
            outline
          ></v-select>
        </div>
      </div>
      <div class="question-component-container"  v-show="hasExternalError">
        <div class="question-component-floating-error">
          {{currentComponentselected.returnedObj.message}}
          <span class="component-error-triangle"></span>
        </div>
      </div>
      <div class="question-add-button-container">
        <v-btn
          :loading="addQuestionButtonLoading"
          class="question-add-button"
          @click="addQuestion()"
        >
          <span v-language:inner>addQuestion_add_button</span>
        </v-btn>
      </div>
    </div>
  </transition>
</template>

<script src="./addQuestion.js"></script>
<style src="./addQuestion.less" lang="less"></style>
