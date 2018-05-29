<template>
    <div class="flashcard-page">
        <div class="flashcard-wrapper">
            <flashcard-header :name="item.name" :showProgress="showCards||isEnded" :isEnded="isEnded" v-model="slideFront"
                              :showCards="showCards" :currentIndex="currentIndex" :cardsSize="showList.length" :pinnedCount="currentPinn.size" :height="$vuetify.breakpoint.smAndUp ? 10 : 20">
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
                <v-menu slot="mobile-actions" class="settings-menu hidden-sm-and-up">
                    <button class="sides" color="primary" dark slot="activator">
                        <settings-icon></settings-icon>
                    </button>
                    <v-list class="sides">
                        <v-list-tile @click="slideFront=1">
                            <v-list-tile-title>
                                front
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="slideFront=0">
                            <v-list-tile-title>
                                back
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="slideFront=-1">
                            <v-list-tile-title>
                                both
                            </v-list-tile-title>
                        </v-list-tile>
                    </v-list>
                </v-menu>
            </flashcard-header>
            <div class="cards-info" v-if="!showCards">
                <div class="content card-template">
                    <h1 v-text="item.name"></h1>
                    <p class="subtitle" v-if="item.cards">{{item.cards.length}} Cards</p>
                    <div class="action-buttons">
                        <button class="action-btn elevation-1" @click="$_startFlashcards">{{beginText}}</button>
                        <button class="action-btn pinned-cards elevation-1" v-if="currentPinn.size" @click="$_startPinnsFlashcards">Study {{currentPinn.size}} pin</button>
                    </div>
                    <div class="shuffle">
                        <shuffle-icon></shuffle-icon>
                        <div>Shuffle cards</div>
                        <v-switch v-model="shuffle"  color="color-green" hide-details></v-switch>
                    </div>
                </div>
            </div>
            <div v-else>
                <flashcard-content v-if="currentCard" v-bind="$attrs" :showFrontSide="slideFront" :card="showList[currentIndex]" :side.sync="slideFront">
                </flashcard-content>
                <nav class="buttons">
                    <button class="prev" @click="currentIndex--" :disabled="currentIndex==0">
                        <v-icon>sbf-chevron-down</v-icon>
                    </button>
                    <button class="next" @click="currentIndex++">
                        <v-icon>sbf-chevron-down</v-icon>
                    </button>
                </nav>

                <v-menu class="keyboard hidden-xs-only" top offset-y>
                    <button color="primary" dark slot="activator">
                        <keyboard-icon></keyboard-icon>
                    </button>
                    <v-list class="keyboard-menu">
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&larr;</span><span class="key-info">Previous</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&rarr;</span><span class="key-info">Next</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&uarr;</span><span class="key-info">FlipUp</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">&darr;</span><span class="key-info">FlipDown</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">P</span><span class="key-info">Pin</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">S</span><span class="key-info">Shuffle</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">F</span><span class="key-info">Front</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">K</span><span class="key-info">Back</span>
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile>
                            <v-list-tile-title>
                                <span class="key">B</span><span class="key-info">Both</span>
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