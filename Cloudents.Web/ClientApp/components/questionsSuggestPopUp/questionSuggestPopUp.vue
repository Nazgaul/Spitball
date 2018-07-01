<template>
    <v-card>
        <button class="close-btn" @click="showDialog = false">
            <v-icon>sbf-close</v-icon>
        </button>

        <h2>Great Job!</h2>
        <p>
            Let’s wait for eidan2134 to see if accepts your answer. In the meantime, below is another question that you
            should be able
            to with.
        </p>
        <v-carousel left-control-icon="sbf-arrow-right left" right-control-icon="sbf-arrow-right right"
                    cycle full-screen
                    hide-delimiters>
            <v-carousel-item v-for="(item,i) in cardList" src="" :key="i">
                <question-card :cardData="item" class="user-question mb-3" detailed-view></question-card>
            </v-carousel-item>
        </v-carousel>
    </v-card>
</template>

<script src="./questionSuggestPopUp.js"></script>

<style src="./questionSuggestPopUp.less" lang="less"></style>
