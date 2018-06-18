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


                <extended-text-area uploadUrl="/api/upload/ask" v-model="textAreaValue" @addFile="addFile"
                                    @removeFile="removeFile"></extended-text-area>
                <v-flex xs6>
                    <select v-model="subject">
                        <option value="" disabled hidden>Pick a subject</option>
                        <option v-for="item in subjectList" :value="item">{{item.subject}}</option>
                    </select>
                </v-flex>




                <!-- <v-flex xs12> -->
                <v-flex xs12 :class="[currentSum>=0 ? 'text-blue' : 'text-red', 'my-points','subheading']">You have {{currentSum}} points</v-flex>
                <!-- </v-flex> -->

                <v-flex xs12>
                    <ul class="points-list">
                        <li :class="`pts-${pricey}`" v-for="(pricey,index) in pricesList" :key="index">
                            <input :id="`${pricey}pts`" class="automatic-amount" type="radio" name="price" :value="pricey" v-model="selectedPrice">
                            <label :for="`${pricey}pts`">{{pricey}} pts</label>
                        </li>
                        <li class="other">
                            <input type="number" placeholder="Other amount" @focus="selectOtherAmount()" v-model="price"/>
                            <v-icon right>sbf-hand-coin</v-icon>
                        </li>
                    </ul>
                </v-flex>

                <v-flex xs12 class="last-text-block">
                    <p class="text-xs-center"><span class="text-blue" style="color:#8888d5;">Tip:</span>&nbsp;A fair
                        price will make the sale</p>
                </v-flex>


                <v-flex class="submit-btn-wrap" xs12>
                    <div v-if="errorMessage.length" class="error-message">{{errorMessage}}</div>
                    <v-btn block color="primary" @click="submitQuestion()" :disabled="!validForm||submitted"
                           class="ask_btn">Ask
                    </v-btn>
                </v-flex>

            </v-layout>

            <!-- <div class="btn-div">
              <button @click=ask() :disabled="!validForm">Ask</button>
            </div> -->
        </v-container>
    </div>
</template>
<script src="./newQuestion.js"></script>
<style src="./newQuestion.less" lang="less"></style>
