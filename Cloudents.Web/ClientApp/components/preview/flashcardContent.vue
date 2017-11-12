<template>
    <v-card>
        <div @click="isFront=!isFront">
            <transition mode="out-in" name="component-fade">
                <div v-if="isFront">
                    This is front
                </div>
                <div v-else >This is back</div>
            </transition>
            <!--<div @click="isToggle=!isToggle">
            <div class="card-header" style="padding-bottom: 15px;"> {{headerFront}} </div>
            <div class="card-content center">
                <p v-bind:style="{fontSize: textSizeFront,fontWeight: 'bold'}">{{front}}</p>
                <img v-if="imgFront!=''" :src="imgFront" width="200" height="200">
            </div>
            <div class="card-footer">{{footerFront}}</div>
        </div>
        <div @click="isToggle=!isToggle" v-show="isToggle" class="animated flipInX flashcard">
            <div class="card-header" style="padding-bottom: 15px;"> {{headerBack}}</div>
            <div class="card-content center">
                <p v-bind:style="{fontSize: textSizeBack, fontWeight: 'bold'}">{{back}}</p>
                <img v-if="imgBack!=''" :src="imgBack" width="200" height="200">
            </div>
        </div>-->
            <div class="card-footer">click to flip</div>
        </div>
    </v-card>
</template>
<script>
    export default {
        data() {
            return {
                isFront: true
            }
        },
        props: ['currentSide']
    }
</script>
<style>
    .component-fade-enter-active, .component-fade-leave-active {
        transition: opacity .7s ease;
    }

    .component-fade-enter, .component-fade-leave-to
    /* .component-fade-leave-active below version 2.1.8 */ {
        opacity: 0;
    }
</style>