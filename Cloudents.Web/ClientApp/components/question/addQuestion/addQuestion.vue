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
            <user-avatar :user-name="'TESTNAME'" :user-id="156"></user-avatar>
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
            <span>X</span>
            <span>Y</span>
            <span>Z</span>
          </div>
          <div class="question-thumbnails-part"></div>
        </div>
      </div>
      <div class="question-subject-class-container">
        <div>
          <v-select height="40" single-line :items="['a','b','c','d','e','f','g','h']" label="Select the subject" :append-icon="''" outline></v-select>
        </div>
        <div>
          <v-select height="40" single-line :items="['a','b','c','d','e','f','g','h']" label="For what class?" :append-icon="''" outline></v-select>
        </div>
      </div>
    </div>
  </transition>
</template>

<script src="./addQuestion.js"></script>
<style src="./addQuestion.less" lang="less"></style>
