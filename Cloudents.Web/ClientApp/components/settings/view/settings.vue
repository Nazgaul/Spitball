<template>
    <div class="settings-page">
        <v-container class="general-cell">
            <v-layout row wrap>

                <v-flex xs12>
                    <div class="header-row">      
                        <div class="icon-block" >
                            <v-icon right>sbf-user</v-icon>
                            <span class="text-blue">Settings</span>
                        </div>                  
                        
                        <button class="back-button">
                            <v-icon right>sbf-close</v-icon>
                        </button>
                    </div>
                </v-flex>


                <v-flex xs12>
                    <div class="upload-avatar">
                        <img class="avatar" src="../../helpers/img/user.png"/>
                        <div class="text">
                            <span class="text-blue">Tap the photo to change it</span>
                            <p>Images must be in PNG or JPG format and under 5mb</p>
                        </div>
                    </div>
                </v-flex>

                <v-flex xs12>
                    <uni-search-input class="uni-field" :placeholder="'Your input'" v-model="test" :prependIcon="'sbf-users'"></uni-search-input>
                </v-flex>

                <v-flex xs12 class="university">
                    <uni-search-input class="uni-field" :placeholder="'Your university'" v-model="university" :prependIcon="'sbf-user'"></uni-search-input>
                </v-flex>

                <v-flex xs12 >
                    <v-btn block color="primary" @click="submitQuestion()" :disabled="!validForm" class="save" >Save</v-btn>
                </v-flex>

            </v-layout>            
        </v-container>
    </div>
</template>
<script src="./settings.js"></script>
<style src="./settings.less" lang="less"></style>
