<template>
    <!--<div :class="{'both':showBoth}">-->
    <div class="card-player">
        isPinn:{{isPined}}
        <div class="card-template" :class="showFront ? 'front' : (showBoth ? 'both' : 'back')">
            {{showFrontSide}}
            <div class="flip-container">
                <div class="flipper">
                    <div class="card-content">
                        <div class="side front">
                            <v-btn @click="$_pinCard">{{isPined?'unpin':'pin'}} this card</v-btn>
                            <div class="data-wrapper" @click="$_flip(slide)">
                                <div class="img-container" v-if="slide.front.image">
                                    <img :src="slide.front.image" v-once alt="font image" />
                                </div>
                                <div class="text-container" v-if="slide.front.text">
                                    <p class="scrollbar" dir="auto" v-text="slide.front.text" fit-text></p>
                                </div>
                                <div class="flip" v-if="!showBoth">
                                    click to flip
                                </div>
                            </div>
                        </div>
                        <div class="side back">
                            <v-btn @click="$_pinCard">{{isPined?'unpin':'pin'}} this card</v-btn>
                            <div class="data-wrapper" @click="$_flip(slide)">
                                <div class="img-container" v-if="slide.cover.image">
                                    <img :src="slide.cover.image" alt="font image" />
                                </div>
                                <div class="text-container" v-once v-if="slide.cover.text">
                                    <p class="scrollbar" dir="auto" v-text="slide.cover.text" fit-text></p>
                                </div>
                                <div class="flip" v-if="!showBoth">
                                    click to flip
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex'
    export default {
        methods: {
            ...mapActions(['updatePinnedCards']),
            $_pinCard() {
                let indexToCheck = this.card.index;
                let currentPinned = new Set(this.pinnedCards[this.$attrs.id]);
                this.isPined ? currentPinned.delete(indexToCheck) : currentPinned.add(indexToCheck);
                let updatedPinned = { [this.$attrs.id]: [...currentPinned] };
                this.updatePinnedCards(updatedPinned);
            },
            $_flip() {
                if (!this.showBoth) this.showFront = !this.showFront;
            }
        },
        computed: {
            ...mapGetters(['pinnedCards']),
            isPined() {
                return new Set(this.pinnedCards[this.$attrs.id]).has(this.card.index)
            },
            slide() {
                this.showFront = this.showFrontSide;
                return this.card.data
            },
            showBoth() {
                return this.showFrontSide == -1;
            }
        },
        data() {
            return {
                showFront: true
            }
        },
        props: {
            card: { type: Object },
            showFrontSide: { default: true }
        }
    }
</script>