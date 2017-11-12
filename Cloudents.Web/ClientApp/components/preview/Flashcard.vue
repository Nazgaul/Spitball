<template>
    <div>
        <flashcard-header :name="item.name" :showCards="showCards" 
                          :currentIndex="currentIndex" :cardsSize="showList.length"></flashcard-header>
    <div class="white" v-if="!showCards">
        <h5 v-text="item.name"></h5>
        <span>Created by {{item.author}}</span>
        <p v-if="item.cards">{{item.cards.length}} Cards</p>
        <v-btn v-if="pinnCards" @click="$_startPinnsFlashcards">take {{pinnCards.length}} pinns cards</v-btn>
        <v-btn @click="$_startFlashcards">{{beginText}}</v-btn>
        <v-switch label="Shuffle cards"
                  v-model="shuffle"
                  color="light-green accent-3"></v-switch>
    </div>
    <div v-else>
        <v-btn @click="currentIndex--" :disabled="currentIndex==0">Back</v-btn>
        <v-btn @click="currentIndex++">Next</v-btn>
        <!--The Original index:{{showList[currentIndex]}}-->
        Is Pin:{{isPinned}}
        <!--<flashcard-content></flashcard-content>-->
        <!--<transition>
            <div v-if="items.length > 0">
                 ... 
            </div>
            <div v-else>Sorry, no items found.</div>
        </transition>-->
        <v-btn @click="$_pinCard">{{isPinned?'unpin':'pin'}} this card</v-btn>

        <vue-flashcard v-if="currentCard" :front="currentCard.front.text"
                       :imgFront="currentCard.front.image" :back="currentCard.cover.text" :imgBack="currentCard.cover.image">
        </vue-flashcard>
    </div>
    </div>
    
</template>
<script type="module" src="./flashcard.js"></script>