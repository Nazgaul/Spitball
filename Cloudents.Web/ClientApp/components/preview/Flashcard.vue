<template>
    <div class="flashcard-page">
        <div class="flashcard-wrapper">
            <flashcard-header :name="item.name" :showProgress="showCards||isEnded" :isEnded="isEnded" v-model="slideFront"
                              :showCards="showCards" :currentIndex="currentIndex" :cardsSize="showList.length" :pinnedCount="currentPinn.size">
                <div v-if="showCards" slot="actions" class="card-sides">
                    <button @click="slideFront=1" class="front" :class="{'active':slideFront==1}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div class="side-name" xs12>Front</div>
                        </v-layout>
                    </button>
                    <button flat @click="slideFront=0" class="back" :class="{'active':slideFront==0}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div class="side-name" xs12>Back</div>
                        </v-layout>
                    </button>
                    <button flat @click="slideFront=-1" class="both" :class="{'active':slideFront==-1}">
                        <v-layout column>
                            <div xs12 class="flashcard-icon"></div>
                            <div class="side-name" xs12>Both</div>
                        </v-layout>
                    </button>
                </div>
            </flashcard-header>
            <div class="cards-info" v-if="!showCards">
                <div class="content card-template">
                    <h1 v-text="item.name"></h1>
                    <h4>Created by Yifat the Queen</h4>
                    <p class="subtitle" v-if="item.cards">{{item.cards.length}} Cards</p>
                    <div class="action-buttons">
                        <button class="action-btn elevation-1" @click="$_startFlashcards">{{beginText}}</button>
                        <button class="action-btn pinned-cards elevation-1" v-if="currentPinn.size" @click="$_startPinnsFlashcards">take {{currentPinn.size}} pinns cards</button>
                    </div>
                    <div class="shuffle">
                        <shuffle-icon></shuffle-icon>
                        <div>Shuffle cards</div>
                        <v-switch v-model="shuffle"  color="color-green" hide-details></v-switch>
                    </div>
                </div>
            </div>
            <div v-else>
                <flashcard-content v-if="currentCard" v-bind="$attrs" :showFrontSide="slideFront" :card="showList[currentIndex]">
                </flashcard-content>
                <nav class="buttons">
                    <button class="prev" @click="currentIndex--" :disabled="currentIndex==0">
                        <v-icon>sbf-chevron-down</v-icon>
                    </button>
                    <button class="next" @click="currentIndex++">
                        <v-icon>sbf-chevron-down</v-icon>
                    </button>
                </nav>

                <v-menu class="keyboard">
                    <button color="primary" dark slot="activator">
                        <keyboard-icon></keyboard-icon>
                    </button>
                    <v-list class="keyboard-menu">
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&larr;</span><span class="info">Previous</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&rarr;</span><span class="info">Next</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&uarr;</span><span class="info">FlipUp</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&darr;</span><span class="info">FlipDown</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">P</span><span class="info">Pinned</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">S</span><span class="info">Shuffle</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">F</span><span class="info">Front</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">K</span><span class="info">Back</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">B</span><span class="info">Both</span>
                            </v-list-tile-title>
                        </v-list-tile>
                    </v-list>
                </v-menu>


            </div>
        </div>
    </div>

</template>
<script type="module" src="./flashcard.js">
</script>
<style src="./flashcard.less" lang="less"></style>