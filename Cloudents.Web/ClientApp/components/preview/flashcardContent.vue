<template>
    <div>
        <div class="card-content">
            <div class="side front">
                <v-btn @click="$_pinCard">{{isPinned?'unpin':'pin'}} this card</v-btn>
                <div class="data-wrapper" @click="flip(slide)">
                    <div class="img-container" v-if="slide.front.image">
                        <img :src="slide.front.image" v-once alt="font image" />
                    </div>
                    <div class="text-container" v-if="slide.front.text">
                        <p class="scrollbar" dir="auto" v-model="slide.front.text" fit-text></p>
                    </div>
                    <!--<div class="flip" ng-if="slide.style !== null">
                        <md-icon md-svg-icon="b:flip"></md-icon>
                        @FlashcardResources.InstructionFlip
                    </div>-->
                </div>
            </div>
            <div class="side back">
                <v-btn @click="$_pinCard">{{isPinned?'unpin':'pin'}} this card</v-btn>
                <div class="data-wrapper" @click="flip(slide)">
                    <div class="img-container" v-if="slide.cover.image">
                        <img :src="slide.cover.image" alt="font image" />
                    </div>
                    <div class="text-container" v-once v-if="slide.cover.text">
                        <p class="scrollbar" dir="auto" v-model="slide.cover.text" fit-text></p>
                    </div>
                    <!--<div class="flip" ng-if="slide.style !== null">
                        <md-icon md-svg-icon="b:flip"></md-icon>
                        @FlashcardResources.InstructionFlip
                    </div>-->
                </div>
            </div>
        </div>
        <!--<div @click="isToggle=!isToggle" v-bind:style="{backgroundColor: colorFront, color: colorTextFront}" v-show="!isToggle" class="animated flipInX flashcard">
            <div class="card-header" style="padding-bottom: 15px;"> {{headerFront}} </div>
            <div class="card-content center">
                <p v-bind:style="{fontSize: textSizeFront,fontWeight: 'bold'}">{{front}}</p>
                <img v-if="imgFront!=''" :src="imgFront" width="200" height="200">
            </div>
            
        </div>
        <div  v-bind:style="{backgroundColor: colorBack, color: colorTextBack}" v-show="isToggle" class="animated flipInX flashcard">
            <div class="card-header" style="padding-bottom: 15px;"> {{headerBack}}</div>
            <div class="card-content center">
                <p v-bind:style="{fontSize: textSizeBack, fontWeight: 'bold'}">{{back}}</p>
                <img v-if="imgBack!=''" :src="imgBack" width="200" height="200">
            </div>
            <div class="card-footer">{{footerBack}}</div>
        </div>
        <div class="card-footer">{{footerFront}}</div>-->
    </div>
</template>
<script>
    export default {
        methods: {
            $_pinCard() {
                let indexToCheck = this.card.index;
                console.log('pin');
                //this.isPinned ? this.pinnCards = this.pinnCards.filter((i) => i != indexToCheck) : this.pinnCards.push(indexToCheck)
                //TODO add and remove from db
            }
        },
        computed: {
            slide() { return this.card.data}
        },
        data() {
            return {
                isToggle: false,
            }
        }, props: {
            isPinned: {type:Boolean},
            card: {type:Object},
            imgFront: {
                type: String,
                default: ''
            },
            imgBack: {
                type: String,
                default: ''
            },
            front: {
                type: String,
                default: ''
            },
            back: {
                type: String,
                default: ''
            },
            textSizeFront: {
                type: String,
                default: '2em'
            },
            textSizeBack: {
                type: String,
                default: '2em'
            },
            colorTextFront: {
                type: String,
                default: 'black'
            },
            colorTextBack: {
                type: String,
                default: 'white'
            },
            colorFront: {
                type: String,
                default: 'white'
            },
            colorBack: {
                type: String,
                default: '#2ecc71'
            },
            headerFront: {
                type: String,
                default: 'Do you know?'
            },
            headerBack: {
                type: String,
                default: 'Answer'
            },
            footerFront: {
                type: String,
                default: 'Click to show Back'
            },
            footerBack: {
                type: String,
                default: 'Click to show Front'
            }
        }
    }
</script>
<style scoped>
    .center {
        text-align: center;
    }

    .flashcard {
        cursor: pointer;
        border-radius: 10px;
        margin: 20px;
        padding: 25px;
        box-shadow: 0 0px 10px rgba(0, 0, 0, 0.4);
        text-align: center;
    }

        .flashcard:hover {
            box-shadow: 0 0px 25px rgba(0, 0, 0, 0.8);
        }

    .animated {
        animation-duration: 1s;
        animation-fill-mode: both;
    }

    @keyframes flipInX {
        from {
            transform: perspective(400px) rotate3d(1, 0, 0, 90deg);
            animation-timing-function: ease-in;
            opacity: 0;
        }

        40% {
            transform: perspective(400px) rotate3d(1, 0, 0, -20deg);
            animation-timing-function: ease-in;
        }

        60% {
            transform: perspective(400px) rotate3d(1, 0, 0, 10deg);
            opacity: 1;
        }

        80% {
            transform: perspective(400px) rotate3d(1, 0, 0, -5deg);
        }

        to {
            transform: perspective(400px);
        }
    }

    .flipInX {
        backface-visibility: visible !important;
        animation-name: flipInX;
    }
</style>