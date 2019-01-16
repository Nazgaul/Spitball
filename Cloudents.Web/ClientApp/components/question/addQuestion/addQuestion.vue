<template>
  <transition name="fade">
    <div class="add-question-container">
      <div class="question-header">
        <h1 class="question-header-title">Add a question</h1>
        <button class="back-button" @click="requestNewQuestionDialogClose()">
          <v-icon right>sbf-close</v-icon>
        </button>
      </div>
      <div class="question-textarea-container">
        <div class="question-textarea-upper-part">
          <div class="question-profile-data-container">
            <user-avatar :user-name="accountUser.name"></user-avatar>
          </div>
          <v-textarea
            solo
            no-resize
            name="add-question-textarea"
            label="Ask your question here…"
            class="question-textarea"
            :rows="7"
          ></v-textarea>
        </div>
        <div class="question-textarea-lower-part">
          <div class="question-options-part">
            <span><v-icon>sbf-close</v-icon></span>
            <span><v-icon>sbf-close</v-icon></span>
            <span><v-icon>sbf-close</v-icon></span>
          </div>
          <div class="question-thumbnails-part" :class="{'show-tumbnails': uploadProp.showTumbnails}">
            <div class="question-attachment-box horizontal-border vertical-border">
              <div class="question-thumb-close-container" :class="{'populated': uploadProp.populatedThumnbailBox.boxA}">
                <v-icon>sbf-close</v-icon>
              </div>
            </div>
            <div class="question-attachment-box horizontal-border vertical-border">
              <div class="question-thumb-close-container" :class="{'populated': uploadProp.populatedThumnbailBox.boxB}">
                <v-icon>sbf-close</v-icon>
              </div>
            </div>
            <div class="question-attachment-box horizontal-border vertical-border">
              <div class="question-thumb-close-container" :class="{'populated': uploadProp.populatedThumnbailBox.boxC}">
                <v-icon>sbf-close</v-icon>
              </div>
            </div>
            <div class="question-attachment-box horizontal-border vertical-border">
              <div class="question-thumb-close-container" :class="{'populated': uploadProp.populatedThumnbailBox.boxD}">
                <v-icon>sbf-close</v-icon>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="question-subject-class-container">
        <div class="question-select">
          <v-select height="40" single-line item-value="id" item-text="subject" :items="subjectList" label="Select the subject" :append-icon="''" outline></v-select>
        </div>
        <div class="question-select">
          <v-select height="40" single-line :items="getSelectedClasses" label="For what class?" :append-icon="''" outline></v-select>
        </div>
      </div>
      <div class="question-component-container">
        <component :is="`question-${currentComponentselected}`"></component>
      </div>
    </div>
  </transition>
</template>

<script src="./addQuestion.js"></script>
<style src="./addQuestion.less" lang="less"></style>
