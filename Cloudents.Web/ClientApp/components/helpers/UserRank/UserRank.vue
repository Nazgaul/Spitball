<template>
    <div class="rank-container">
        <div :class="[`level-${rank}`]">
            <span>{{rankName}}</span>
        </div>
    </div>
</template>

<script>
import reputationService from '../../../services/reputationService.js'
import { LanguageService } from '../../../services/language/languageService.js'
export default {
    data(){
        return {
            rankNames: [
                LanguageService.getValueByKey('userRank_begginer'),
                LanguageService.getValueByKey('userRank_professional'),
                LanguageService.getValueByKey('userRank_worldClass'),
                LanguageService.getValueByKey('userRank_spitballer')
            ]
        }
    },
    props:{
        score:{
            required: true
        }
    },
    computed:{
        rank(){
            return reputationService.calculateRankByScore(this.score);
        },
        rankName(){
            return this.rankNames[this.rank];
        }
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
    .rank-container{
            font-family: @fontFiraSans;
            font-size: 12px;
            font-style: italic;
            line-height: 1;
            letter-spacing: -0.2px;
            text-align: center;
            color: rgba(0, 0, 0, 0.54);
            @media(max-width: @screen-xs) {
                margin-top: unset;
            }
        .level-0 {
            span{
                background-color: #ebebeb;
                border-radius: 10px;
                padding: 2px 12px;
            }
        }
        .level-1 {
            span{
                background-color: #acacac;
                color: #ffffff;
                border-radius: 10px;
                padding: 2px 12px;
            }
        }
        .level-2 {
            span{
                background-color: #939393;
                color: #ffffff;
                border-radius: 10px;
                padding: 2px 12px;
            }
        }
        .level-3 {
            span{
                background-color: #5c5c5c;
                color: #ffffff;
                border-radius: 10px;
                padding: 2px 12px;
            }
        }
        
    }
</style>
