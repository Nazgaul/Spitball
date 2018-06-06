<template>
    <div class="ask_question">
        <v-container class="general-cell">
            <v-layout row wrap>

                <v-flex xs12>
                    <div class="header-row">
                        <div class="student_icon">
                            <img :src="require(`./img/student_ask.png`)"/>
                        </div>
                        <span class="text-blue">Get Your Question Answered</span>
                        <button class="back-button" @click="$router.go(-1)">
                            <v-icon right>sbf-close</v-icon>
                        </button>
                    </div>
                </v-flex>


                <v-flex xs12>
                    <select v-model="subject">
                        <option value="" disabled hidden>Pick a subject</option>
                        <option v-for="item in subjectList" :value="item">{{item.subject}}</option>
                    </select>
                </v-flex>


                <extended-text-area uploadUrl="/api/upload/ask" v-model="textAreaValue" @addFile="addFile" @removeFile="removeFile"></extended-text-area>

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


                <v-flex class="submit-btn-wrap" xs12>
                    <div v-if="errorMessage" class="error-messages">{{errorMessage}}</div>
                    <v-btn block color="primary" @click.once="submitQuestion()" :disabled="!validForm||submitted" class="ask_btn">Ask</v-btn>
                </v-flex>

            </v-layout>

            <!-- <div class="btn-div">
              <button @click=ask() :disabled="!validForm">Ask</button>
            </div> -->
        </v-container>
    </div>
</template>
<script src="./askQuestion.js"></script>
<style src="./askQuestion.less" lang="less"></style>
