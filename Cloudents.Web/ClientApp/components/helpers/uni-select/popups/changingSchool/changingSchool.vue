<template>
    <div class="changing-school-container" :class="{'active': showPopup}">
        <div class="header-container">
            <div class="warning-triangle"><span>!</span></div>
            <div class="icons">
                <span><v-icon class="blue">sbf-school</v-icon></span>
                <span><v-icon class="blue small">sbf-curved-arrow</v-icon></span>
                <span><v-icon>sbf-school</v-icon></span>
            </div>
            <div class="title">
                <span>Changing Schools?</span>
            </div>
        </div>
        <div class="bottom-container">
            <div class="description">
                <span class="blueText">{{schoolName}}</span>
                <span>{{schoolDesc}}</span>
            </div>
            <div class="buttons-container">
                <button @click="revertAction()">Keep Current School</button>
                <button class="transparent" @click="continueAction()">YES, Change School</button>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data(){
        return{
            schoolName: "New Jersey Institue of Technology,",
            schoolDesc: "\n and all of the selected classes \n will be deleted",
            showPopup: false,
        }
    },
    methods:{
        revertAction(){
            this.showPopup = false;
        },
        continueAction(){
            this.showPopup = false;
        },
    }
}
</script>

<style lang="less">
    .changing-school-container{
        display: none;
        width: 297px;
        height: 338px;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(0, 0, 0, 0.39);
        background-color: #ffffff;
        z-index: 10;
        position: absolute;
        top: 38px;
        &.active{
            display: block;
        }
        .header-container{
            position: relative;
            height: 125px;
            background-color: #f7f7f7;
            display:flex;
            flex-direction: column;
            justify-content: center;
            border-top-left-radius: 4px;
            border-top-right-radius: 4px;
            .warning-triangle{
                position: absolute;
                right:0;
                top:0;
                width: 0;
                height: 0;
                border-style: solid;
                border-width: 0 46px 43px 0;
                border-color: transparent #ffeb3b transparent transparent !important;
                span{
                    position: absolute;
                    right: -37px;
                    top: 4px;
                    font-weight: 600;
                    font-family: 'Fira Sans', sans-serif;
                    font-size: 18px;
                    font-weight: normal;
                    font-style: normal;
                    font-stretch: normal;
                    line-height: normal;
                    letter-spacing: normal;
                    text-align: center;
                    color: rgba(0, 0, 0, 0.87);
                }
            }
            .icons{
                display:flex;
                margin: 14px auto;
                text-align: center;
                vertical-align: middle;
                span{
                    margin:3px;
                    i{
                        opacity: 0.35;
                        font-size: 33px;
                        &.blue{
                            color: #4452FC;
                        }
                        &.small{
                            font-size:15px;
                            margin-top: 10px;
                        }
                    }
                }
            }
            .title{
                margin: 0px auto;
                font-family: 'Fira Sans', sans-serif;
                font-size: 18px;
                font-weight: normal;
                font-style: normal;
                font-stretch: normal;
                line-height: 1.5;
                letter-spacing: normal;
                text-align: center;
                color: rgba(0, 0, 0, 0.87);
            }
        }
        .bottom-container{
            margin: 0px auto;
            text-align: center;
            .description{
                height:66px;
                white-space: pre-wrap;
                .blueText{
                    color: #4452fc;
                    font-weight: 600;
                }
            }
            .buttons-container{
                display: flex;
                flex-direction: column;
                margin-top: 20px;
                button{
                    width: 162px;
                    height: 48px;
                    margin: 4px auto;
                    background-color: rgba(68, 82, 252, 0.09);
                    border-radius: 4px;
                    outline: none;
                    &.transparent{
                        background-color: transparent !important;
                        border: solid 1px #d8d6d6 !important;
                        color: rgba(0, 0, 0, 0.54);
                    }
                }
            }
        }
    }
</style>
