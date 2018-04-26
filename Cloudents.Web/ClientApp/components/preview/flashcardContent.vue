<template>
    <!--<div :class="{'both':showBoth}">-->
    <div class="card-player">
        <div class="card-template" :class="showBoth ? 'both' : (showFront ? 'front' : 'back')">
            <div class="flip-container">
                <div class="flipper">
                    <div class="card-content">
                        <div class="side front">
                            <button class="pin-btn" @click="$_pinCard" :class="{'pinned':isPined}">
                                <pin-icon></pin-icon>
                            </button>
                            <div class="data-wrapper" role="button" @click="$_flip(slide)">
                                <div class="img-container" v-if="slide.front.image">
                                    <img :src="slide.front.image" v-once alt="font image" />
                                </div>
                                <fit-text  v-if="slide.front.text" :input-text="slide.front.text" :preview-options="previewOptions"></fit-text>
                                <!--<div class="text-container" v-if="slide.front.text">-->
                                    <!--<p class="scrollbar" dir="auto" v-text="slide.front.text" fit-text></p>-->
                                <!--</div>-->
                                <v-layout class="flip" v-if="!showBoth" row align-center>
                                    <flip-icon></flip-icon>
                                    <span>Click to flip</span>
                                </v-layout>
                            </div>
                        </div>
                        <div class="side back">
                            <button class="pin-btn" @click="$_pinCard" :class="{'pinned':isPined}">
                                <pin-icon></pin-icon>
                            </button>
                            <div class="data-wrapper" @click="$_flip(slide)">
                                <div class="img-container" v-if="slide.cover.image">
                                    <img :src="slide.cover.image" alt="font image" />
                                </div>
                                <fit-text  v-if="slide.cover.text" :input-text="slide.cover.text" :preview-options="previewOptions"></fit-text>

                                <!--<div class="text-container" v-if="slide.cover.text">-->
                                    <!--<p class="scrollbar" dir="auto" v-text="slide.cover.text" fit-text></p>-->
                                <!--</div>-->
                                <div class="flip" v-if="!showBoth">
                                    <flip-icon></flip-icon>
                                    Click to flip
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
    import FitText from './../helpers/fitText.vue';
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
                if (!this.showBoth){ this.showFront = !this.showFront;
                    this.previewOptions.safeWidth= this.$el.clientWidth;
                    this.$emit('update:side',this.showFront)
                }
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
                // this.previewOptionsCover.previewVertOffset=this.previewOptions.safeWidth;
                // this.previewOptionsCover.safeWidth=this.previewOptions.safeWidth/2;

                return this.showFrontSide === -1;
            }
        },
        watch:{
            showFrontSide(val){
                // if(val===-1) {
                //     this.previewOptions.safeWidth = this.previewOptions.safeWidth / 2;
                // }else{safeWidth:elm.clientWidth}
                this.previewOptions.safeWidth=(val===-1)?this.previewOptions.safeWidth / 2:this.$el.clientWidth;
            }
        },
        mounted(){
            let elm=this.$el;
            this.previewOptions={...this.previewOptions,safeWidth:elm.clientWidth,previewHeight:elm.clientHeight};
            // this.$nextTick(()=>{
            //     this.previewOptionsCover=this.previewOptions;
            // })
        },
        data() {
            return {
                showFront: true,
                previewOptions: {
                    previewVertOffset: 0,
                    originalFontSize: 24
                }
            }
        },
        props: {
            card: { type: Object },
            showFrontSide: { default: true }
        },
        components: {
            FitText,
            pinIcon: () => import("./svg/pin-icon.svg"),
            flipIcon: () => import("./svg/flip-icon.svg")        }
    }
</script>