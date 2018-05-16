<template>
    <v-content>
        <v-container>
            <v-layout row wrap class="ask_question">

                <v-flex xs12>
                    <div class="header-row">
                        <div class="student_icon">
                            <img :src="require(`./img/student_ask.png`)"/>
                        </div>
                        <span class="text-blue">Get Your Question Answered</span>
                        <button class="back-button">
                            <v-icon right>sbf-close</v-icon>
                        </button>
                    </div>
                </v-flex>


                <v-flex xs12>
                    <select v-model="category">
                        <option value="" disabled>Pick a subject</option>
                        <option v-for="item in selectItems">{{item}}</option>
                    </select>
                </v-flex>


                <v-flex xs12 class="question_area">
                    <textarea v-model="questionText" rows="9"
                              placeholder="Type in your question (Ex: Find the derivative of y = (x + 2)2 - 5x3)"></textarea>
                    <ul class="actions_text">
                        <li>
                            <button class="insert-equation">
                                <v-icon>sbf-pi-symbol</v-icon>
                            </button>
                        </li>
                        <li>
                            <button class="insert-symbol"><v-icon>sbf-omega-symbol</v-icon></button>
                        </li>
                        <li>
                            <button class="attach-file">
                                <v-icon>sbf-attach</v-icon>
                            </button>
                        </li>
                    </ul>
                    <ul class="actions_text">
                        <li>
                            <button class="icon"></button>
                        </li>
                        <li>
                            <button class="icon icon-red"></button>
                        </li>
                        <li>
                            <button class="icon icon-purple"></button>
                        </li>
                    </ul>
                </v-flex>

                <v-flex xs12>
                    <v-text-field
                            type="number"
                            name="input-1-3"
                            label="Price"
                            v-model.number="price"
                            single-line
                            full-width
                    ></v-text-field>


                </v-flex>

                <v-flex xs12 class="last-text-block">
                    <p class="text-blue text-xs-center">Price you’ll pay for an answer</p>
                    <p class="small-bold text-xs-center" style="margin-bottom: 1px;">(minimum 0.5)</p>
                    <p class="text-xs-center"><span class="text-blue" style="color:#8888d5;">Tip:</span> A fair price
                        will make the sale faster</p>
                </v-flex>


                <v-flex xs12>
                    <v-btn block color="primary" @click=ask() :disabled="!validForm" class="ask_btn">Ask</v-btn>
                </v-flex>

            </v-layout>

            <!-- <div class="btn-div">
              <button @click=ask() :disabled="!validForm">Ask</button>
            </div> -->
        </v-container>
    </v-content>
</template>

<script src="./ask-question.js"></script>
<style src="./ask-question.less" lang="less"></style>
