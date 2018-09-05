<template functional>
    <v-layout class="flashcard-header" row justify-space-between>
        <v-flex class="progress-wrapper">

            <div class="flashcard-title">
                <h4 v-if="props.showProgress">{{props.name}} </h4>
            </div>
            <div class="progress-settings" v-if="props.showProgress">
                <slot name="mobile-actions"></slot>
                <v-progress-linear :height="props.height" color="info" class="progress-line" :value="((props.isEnded?props.currentIndex:props.currentIndex+1)/(props.isEnded?props.currentIndex:props.cardsSize))*100"></v-progress-linear>
            </div>
            <v-layout v-if="props.showProgress" class="progress-text" row justify-space-between>
                <div class="hidden-xs-only" v-language:inner>flashcardheader_progress</div>
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
                <div v-language:inner>flashcardheader_pinned</div>
            </div>
        </div>
    </v-layout>
</template>
<style src="./flashcardHeader.less" lang="less"></style>