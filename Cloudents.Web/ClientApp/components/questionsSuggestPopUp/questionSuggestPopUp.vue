<template>
    <v-card>
        <div class="dialog-wrapp">
            <button class="close-btn text-md-right" @click.prevent="requestDialogClose()">
                <v-icon>sbf-close</v-icon>
            </button>
            <div class="ml-1 wrap-text">
                <h2 class="text-md-left">Great Job!</h2>
                <p>
                    Let’s wait for <span class="user-name">{{user.name}}</span> to see if accepts your answer.
                </p>
                <p>
                    In the meantime, below is another question that you
                    should be able
                    to with.
                </p>
            </div>
            <v-carousel :interval="interval"
                        @click="requestDialogClose()"
                        delimiter-icon="sbf-circle-solid"
                        prev-icon="sbf-arrow-left-carousel"
                        next-icon="sbf-arrow-right-carousel">

                <v-carousel-item v-for="(item,i) in cardList" src="" :key="i"  @click.native="answerMore(item.id)">
                    <router-link :item="item" :to="{path:'/question/'+item.id}"  class="mb-5">
                        <question-card :typeAnswer="false" :cardData="item" class="user-question mb-1 mt-1" detailed-view></question-card>
                    <!--<v-btn @click="answerMore(item.id)">test</v-btn>-->
                    </router-link>
                </v-carousel-item>
            </v-carousel>
        </div>
    </v-card>

</template>

<script src="./questionSuggestPopUp.js"></script>

<style src="./questionSuggestPopUp.less" lang="less"></style>
