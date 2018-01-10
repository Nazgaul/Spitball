<template>
    <div class="flashcard-page">
        <div class="flashcard-wrapper">
            <flashcard-header :name="item.name" :showCards="showCards" v-model="slideFront"
                              :currentIndex="currentIndex" :cardsSize="showList.length">
                <div v-if="showCards" slot="actions" class="card-sides">
                    <button @click="slideFront=1" class="front" :class="{'active':slideFront==1}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div xs12>Front</div>
                        </v-layout>
                    </button>
                    <button flat @click="slideFront=0" class="back" :class="{'active':slideFront==0}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div xs12>Back</div>
                        </v-layout>
                    </button>
                    <button flat @click="slideFront=-1" class="both" :class="{'active':slideFront==-1}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div xs12>Both</div>
                        </v-layout>
                    </button>
                </div>
            </flashcard-header>
            <div class="white" v-if="!showCards">
                <h5 v-text="item.name"></h5>
                <span>Created by Yifat the Queen</span>
                <p v-if="item.cards">{{item.cards.length}} Cards</p>
                <v-btn v-if="currentPinn.size" @click="$_startPinnsFlashcards">take {{currentPinn.size}} pinns cards</v-btn>
                <v-btn @click="$_startFlashcards">{{beginText}}</v-btn>
                <v-switch label="Shuffle cards"
                          v-model="shuffle"
                          color="light-green accent-3"></v-switch>
            </div>
            <div v-else>
                <flashcard-content v-if="currentCard" v-bind="$attrs" :showFrontSide="slideFront" :card="showList[currentIndex]">
                </flashcard-content>
                <div class="buttons">
                    <v-btn @click="currentIndex--" :disabled="currentIndex==0">Back</v-btn>
                    <v-btn @click="currentIndex++">Next</v-btn>
                </div>
            </div>
        </div>
    </div>

</template>
<script type="module" src="./flashcard.js">
</script>
<style src="./flashcard.less" lang="less"></style>