<!--<template>-->
<!--<v-layout row justify-space-between>-->
<!--<v-flex xs6>-->
<!--<div v-if="showCards">{{name}} </div>-->
<!--<v-progress-linear v-if="showCards" height="10" color="info" :value="((currentIndex+1)/cardsSize)*100"></v-progress-linear>-->
<!--<v-layout v-if="showCards" row justify-space-between> <v-flex xs2>progress</v-flex><v-flex xs2 class="text-xs-right"><span>{{(currentIndex+1)}}/{{cardsSize}}</span></v-flex></v-layout>-->
<!--</v-flex><v-flex xs4><close-action></close-action>-->
<!--<div v-if="showCards">-->
<!--<v-btn @click="$emit('input',1)" :value="1" :class="{'active':value==1}">Front</v-btn>-->
<!--<v-btn @click="$emit('input',0)" :value="0" :class="{'active':value==0}">Back</v-btn>-->
<!--<v-btn @click="$emit('input',-1)" :value="-1" :class="{'active':value==-1}">Both</v-btn>-->
<!--</div>-->
<!--</v-flex>-->
<!--</v-layout>-->
<!--</template>-->
<!--<script>-->
<!--import closeAction from './itemActions.vue'-->
<!--export default {-->
<!--props: { value: {}, showCards: { type: Boolean }, name: { type: String }, currentIndex: { type: Number }, cardsSize: { type: Number } },-->
<!--components: { closeAction}-->
<!--}-->
<!--</script>-->
<template functional>
    <v-layout class="flashcard-header" row justify-space-between>
        <v-flex class="progress-wrapper">

            <div class="flashcard-title">
                <h4 v-if="props.showProgress">{{props.name}} </h4>
            </div>
            <v-progress-linear v-if="props.showProgress" height="10" color="info" :value="((props.isEnded?props.currentIndex:props.currentIndex+1)/(props.isEnded?props.currentIndex:props.cardsSize))*100"></v-progress-linear>
            <v-layout v-if="props.showProgress" class="progress-text" hidden-xs-only row justify-space-between>
                <div>progress</div>
                <div class="text-xs-right">
                    <span>{{(props.currentIndex+1)}}/{{props.cardsSize}}</span>
                </div>
            </v-layout>
        </v-flex>
        <close-action class="close-btn"></close-action>
        <slot name="actions"></slot>
        <div class="pinned-counter" v-if="props.isEnded">
            <div class="num">{{props.pinnedCount}}</div>
            <div class="icon">
                <pin-icon></pin-icon>
                <div>pinned</div>
            </div>
        </div>
    </v-layout>
</template>
<style src="./flashcardHeader.less" lang="less"></style>