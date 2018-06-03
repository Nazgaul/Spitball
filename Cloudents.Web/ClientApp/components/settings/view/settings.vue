<template>
    <div class="settings-page">
        <v-container class="general-cell">
            <v-layout row wrap>
                <div class="flex main-content">
                    <v-flex xs12>
                        <div class="header-row">      
                            <div class="icon-block" >
                                <v-icon right>sbf-settings</v-icon>
                                <span class="text-blue">Settings</span>
                            </div>                  
                            
                            <button class="back-button" @click="$emit('input',false)">
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

                    <v-flex xs12 class="username">
                    <v-icon right>sbf-users</v-icon>
                    <input type="text" placeholder="User Name" v-model="username">
                    </v-flex>

                    <v-flex xs12 class="university">
                        <uni-search-input class="uni-field" :placeholder="'Your university'" v-model="university" :prependIcon="'sbf-user'"></uni-search-input>
                    </v-flex>
                </div>

                <v-flex xs12 >
                    <v-btn block color="primary" @click="submitQuestion()" :disabled="!validForm" class="save" >Save</v-btn>  
                </v-flex>

            </v-layout>            
        </v-container>
    </div>
</template>
<script src="./settings.js"></script>
<style src="./settings.less" lang="less"></style>
