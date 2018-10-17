<template>
    <transition name="fade">
        <div class="ask_question">
            <v-container class="general-cell">
                <v-layout row wrap>
                    <v-flex xs12>
                        <div class="header-row">
                            <div class="student_icon">
                                <img :src="require(`./img/student_ask.png`)"/>
                            </div>
                            <span class="text-blue" v-language:inner>newQuestion_Get_Your_Question_Answered</span>
                            <button class="back-button" @click="requestNewQuestionDialogClose()">
                                <v-icon right>sbf-close</v-icon>
                            </button>
                        </div>
                    </v-flex>
                    <extended-text-area uploadUrl="/api/upload/ask"
                                        v-model="textAreaValue"
                                        @addFile="addFile"
                                        :actionType="actionType"
                                        :error="errorTextArea"
                                        @removeFile="removeFile"

                    >
                    </extended-text-area>

                    <v-flex xs6 :class="{'has-error':!subject && errorMessageSubject}" class="inputBorder">
                        <select v-model="subject">
                            <option value="" disabled hidden v-language:inner>newQuestion_Pick_a_subject</option>
                            <option v-for="item in subjectList" :value="item">{{item.subject}}</option>
                        </select>
                    </v-flex>
                    <v-flex xs6 v-if="!subject" class="input-error">
                        <span>{{errorMessageSubject}}</span>
                    </v-flex>

                    <v-flex xs12 v-if="currentSum"
                            :class="[currentSum>=0 ? 'text-blue' : 'text-red', 'my-points','subheading']"><span
                            v-language:inner>newQuestion_You_have</span>
                        {{currentSum | fixedPoints}} <span v-language:inner>newQuestion_SBL</span>
                    </v-flex>
                    <v-flex xs12 v-if="(price > 100)" :class="[price < 100 ? 'text-blue' : 'text-red']" v-language:inner>
                        newQuestion_max_SBL
                    </v-flex>
                    <v-flex xs12 v-else-if="price < 1 && price" :class="[price > 1 ? 'text-blue' : 'text-red']"
                            v-language:inner>
                        newQuestion_min_SBL
                    </v-flex>
                    <!-- </v-flex> -->

                    <v-flex xs12 class="mb-0">
                        <div class="points-list">
                            <div class="points-line">
                                <div class="point-btn" :class="`pts-${pricey}`" v-for="(pricey,index) in pricesList"
                                     v-if="index<3" :key="index">
                                    <input :id="`${pricey}pts`" class="automatic-amount" type="radio" name="price"
                                           :value="pricey" v-model="selectedPrice">
                                    <label :for="`${pricey}pts`">{{pricey}} SBL</label>
                                </div>
                            </div>
                            <div class="points-line">
                                <div class="point-btn" :class="`pts-${pricey}`" v-for="(pricey,index) in pricesList"
                                     v-if="index>=3" :key="index">
                                    <input :id="`${pricey}pts`" class="automatic-amount" type="radio" name="price"
                                           :value="pricey" v-model="selectedPrice">
                                    <label :for="`${pricey}pts`">{{pricey}} <span
                                            v-language:inner>newQuestion_SBL</span></label>
                                </div>
                                <div class="point-btn other inputBorder">
                                    <input type="number" placeholder="newQuestion_otherAmount_placeholder"
                                           v-language:placeholder
                                           @focus="selectOtherAmount()" step="1"
                                           min="1" max="100"
                                           :class="[price ? 'has-value' : '']"
                                           v-model="price"/>
                                    <v-icon right>sbf-hand-coin</v-icon>
                                </div>
                            </div>
                        </div>
                    </v-flex>

                    <v-flex xs12 class="error-block">
                        <div v-if="errorSelectPrice.length && !selectedPrice && !price" class="error-message">
                            {{errorSelectPrice}}
                        </div>

                    </v-flex>

                    <v-flex xs12 class="last-text-block">
                        <p class="text-xs-center"><span class="text-blue" style="color:#8888d5;" v-language:inner>newQuestion_Tip</span>&nbsp;
                            <span v-language:inner>newQuestion_fair_prise</span>
                        </p>
                    </v-flex>
                    <v-flex class="submit-btn-wrap" xs12>
                        <div v-if="currentSum < 0" class="error-message" v-language:inner>newQuestion_sufficient_SBL
                        </div>
                        <v-btn block color="primary" @click="submitQuestion()" :disabled="submitted"
                               :loading="loading"
                               class="ask_btn"><span v-language:inner>newQuestion_Ask</span></v-btn>
                    </v-flex>

                </v-layout>
            </v-container>
        </div>
    </transition>
</template>
<script src="./newQuestion.js"></script>
<style src="./newQuestion.less" lang="less"></style>
