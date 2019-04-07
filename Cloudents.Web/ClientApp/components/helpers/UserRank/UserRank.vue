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
            width: 75px;
            margin-top: 8px;
            font-family: @fontFiraSans;
            font-size: 12px;
            font-weight: normal;
            font-style: italic;
            font-stretch: normal;
            line-height: 1;
            letter-spacing: -0.2px;
            text-align: center;
            color: rgba(0, 0, 0, 0.54);
            @media(max-width: @screen-xs) {
                margin-top: unset;
            }
        .level-0 {
            background-color: #ebebeb;
            border-radius: 8px;
            height: 16px;
            padding-top: 2px;
        }
        .level-1 {
            background-color: #acacac;
            border-radius: 8px;
            height: 16px;
            color: #ffffff;
            padding-top: 2px;
        }
        .level-2 {
            background-color: #939393;
            border-radius: 8px;
            height: 16px;
            color: #ffffff;
            padding-top: 2px;
        }
        .level-3 {
            background-color: #5c5c5c;
            border-radius: 8px;
            height: 16px;
            color: #ffffff;
            padding-top: 2px;
        }
    }
</style>
