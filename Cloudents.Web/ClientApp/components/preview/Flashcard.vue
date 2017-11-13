<template>
    <div>
        <flashcard-header :name="item.name" :showCards="showCards" v-model="slideFront"
                          :currentIndex="currentIndex" :cardsSize="showList.length"></flashcard-header>
    <div class="white" v-if="!showCards">
        <h5 v-text="item.name"></h5>
        <span>Created by {{item.author}}</span>
        <p v-if="item.cards">{{item.cards.length}} Cards</p>
        <v-btn v-if="currentPinn.size" @click="$_startPinnsFlashcards">take {{currentPinn.size}} pinns cards</v-btn>
        <v-btn @click="$_startFlashcards">{{beginText}}</v-btn>
        <v-switch label="Shuffle cards"
                  v-model="shuffle"
                  color="light-green accent-3"></v-switch>
    </div>
    <div v-else>
        <v-btn @click="currentIndex--" :disabled="currentIndex==0">Back</v-btn>
        <v-btn @click="currentIndex++">Next</v-btn>

        <flashcard-content v-if="currentCard" v-bind="$attrs" :showFrontSide="slideFront" :card="showList[currentIndex]">
        </flashcard-content>
    </div>
    </div>
    
</template>
<script type="module" src="./flashcard.js"></script>