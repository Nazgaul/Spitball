<template>
    <v-container class="tutor-card-container" @click="updateTutorDialog(true)">
        <v-layout class="tutor-card-upper-area">
        </v-layout>
        
        <v-layout column align-center justify-space-between class="tutor-card-middle-area">
            <v-flex class="tutor-button-container">
                <span class="tutor-face-container">
                    <v-icon>sbf-face-female-icon</v-icon>
                    <v-icon>sbf-face-icon</v-icon>
                </span>
                <button class="tutor-button" @click="updateTutorDialog(true)" v-language:inner>studyRoom_become_tutor</button>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-space-between class="tutor-card-bottom-area">
           <v-flex class="text-card" v-language:inner>studyRoom_help_others</v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import utilitiesService from "../../../services/utilities/utilitiesService";
import {mapActions} from 'vuex'
export default {
    data(){
        return {
        }
    },
    methods:{
        ...mapActions(['updateTutorDialog']),
    },
    computed:{
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutor-card-container{
        background-color: #ffca54;
        border-radius: 4px;
        box-shadow: 0 3px 14px 0 rgba(0, 0, 0, 0.36);
        padding: 0;
        margin: 16px 16px 16px 0;
        min-width: 164px;
        max-width: 164px;
        display: flex;
        flex-direction: column;
        min-height: 232px;
        @media (max-width: @screen-xs) {
            margin:5px;
        }
        .tutor-card-upper-area{
            flex-grow: 1;
            background-color: #fad88d;
            color:#a5a4bf;
            border-radius: 4px 4px 0 0;
            text-align: center;
            border-bottom-left-radius: 100%;
            border-bottom-right-radius: 100%;
            &.study-card-active{
                background-color: rgba(66, 224, 113, 0.16);
                color: #34ca61;
            }
        }
        .tutor-card-middle-area{
            flex-grow: 1;
            position: relative;
            margin:10px 0;
            color: #5d5d5d;
            font-size:12px;
            padding: 20px 0;
            .tutor-face-container{
                margin: 15px auto;
                i{
                    margin: 0 2px;
                    font-size:31px;
                    color:@global-purple;
                }
            }
            .tutor-button-container{
                    position: absolute;
                    bottom: -35px;
                    display: flex;
                    flex-direction: column;
                .tutor-button{
                    background-color: @global-purple;
                    color: white;
                    border-radius: 22.5px;
                    padding: 14px 24px;
                    font-size: 11px;
                    outline: none;
                }
            }
        }
        .tutor-card-bottom-area{
            flex-grow: 2;
            background-color: #fad88d;
            font-size: 11px;
            color: rgba(0, 0, 0, 0.54);
            padding: 8px 12px;
            border-top-left-radius: 100%;
            border-top-right-radius: 100%;
            .text-card{
                text-align: center;
                padding: 10px 5px 0 5px;
                color: rgba(0, 0, 0, 0.54);
                font-size: 13px;
                line-height: 1.46;
            }
        }
    }
</style>
