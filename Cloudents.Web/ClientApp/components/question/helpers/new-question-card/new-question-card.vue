<template>
  <div class="question-container">
    <router-link :to="{path:'/question/'+cardData.id}">
      <div class="question-header-container">
        <div class="question-header-large-sagment">
          <div class="avatar-area">
            <user-avatar :user-name="cardData.user.name" :user-id="cardData.user.id"/>
          </div>
          <div class="rank-date-container">
            <div class="rank-area">
              <user-rank :rank="randomRank"></user-rank>
            </div>
            <div class="date-area">{{uploadDate}}</div>
          </div>

          <div class="sold-area" v-if="isSold">
            <div class="sold-container">
              <span>SOLD</span>
              <v-icon>sbf-curved-arrow</v-icon>
            </div>
          </div>
        </div>
        <div class="question-header-small-sagment">
          <div class="price-area" :class="{'sold': isSold}">
            {{cardPrice}}
            <span>SBL</span>
          </div>
          <div class="menu-area">
            <v-menu bottom left>
              <v-btn :depressed="true" @click.prevent slot="activator" icon>
                <v-icon>sbf-3-dot</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile v-for="(action, i) in actions" :key="i">
                  <v-list-tile-title>{{ action.title }}</v-list-tile-title>
                </v-list-tile>
              </v-list>
            </v-menu>
          </div>
        </div>
      </div>
      <div class="question-body-container">
        <div class="question-left-body-container">
          <span class="question-raputation upvote-arrow">
            <v-icon>sbf-arrow-right</v-icon>
          </span>
          <span class="question-raputation question-score">{{questionReputation}}</span>
          <span class="question-raputation downvote-arrow">
            <v-icon>sbf-arrow-right</v-icon>
          </span>
        </div>
        <div class="question-right-body-container">
          <div class="question-body-header-container">
            <span class="question-subject">{{cardData.subject}}</span>
          </div>
          <div
            class="question-body-content-container"
            :class="[`align-switch-${cardData.isRtl ? isRtl ? 'l' : 'r' : isRtl ? 'r' : 'l'}`]"
          >
            <span>{{cardData.text}}</span>
          </div>
        </div>
      </div>
      <div class="question-footer-container">
        <div class="answer-display-container">
          <div>
            <v-icon
              v-for="(answer, index) in answersNumber"
              :key="index"
              class="answer-icon"
            >sbf-comment-icon</v-icon>
          </div>
          <div v-if="answersDeltaNumber > 0" class="answers-delta-number-container">
            <span>{{answersDeltaNumber}}</span>
          </div>
          <div
            v-if="cardData.watchingNow > 0 && !isSold"
            class="answer-currently-watching-container"
            :class="{'space': answersNumber > 0}"
          >
            <span>Someone is answering now…</span>
          </div>
        </div>
        <div class="answers-info-container">
          <div v-if="cardData.filesNum > 0" class="answers-attachments-container">
            <v-icon>sbf-attach</v-icon>
            <span>{{cardData.filesNum}}</span>
          </div>
          <div class="answers-viewers-container">
            <v-icon class>sbf-views</v-icon>
            <span>{{randomViews}}</span>
          </div>
        </div>
      </div>
      <div v-if="!isSold" class="question-bottom-section">
        <div class="question-input-container">
          <input class="question-input" placeholder="Answer..." type="text">
        </div>
      </div>
    </router-link>
  </div>
</template>

<script src="./new-question-card.js"></script>
<style lang="less" src="./new-question-card.less"></style>
