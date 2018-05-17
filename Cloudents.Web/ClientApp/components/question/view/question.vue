<template>
    <div>
        <v-content>
            <v-container>
                <v-layout row wrap class="answer-question">
                    <v-flex xs7>
                        <v-flex class="user-question">   
                            <question-card></question-card> 
                        </v-flex>                    
                        <question-text-area v-model="questionText"></question-text-area>
                        <v-btn block color="primary" @click=answer() :disabled="!validForm" class="add_answer">Add your answer</v-btn>
                    </v-flex>  

                    <v-flex offset-xs1 xs4>
                            <mini-chat></mini-chat>             
                    </v-flex>
                    

                </v-layout>
            </v-container>
        </v-content>
    </div>
</template>

<style src="./question.less" lang="less"></style>
<script src="./question.js"></script>
