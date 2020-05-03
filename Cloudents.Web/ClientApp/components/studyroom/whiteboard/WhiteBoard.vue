<template>
    <div class="canvas-container" id="canvasDiv">
        <div id="canvas-wrapper" class="canvas-wrapper" style="position:relative; overflow: auto;" :style="`width:${windowWidth}px;height:${windowHeight}px;`">
            <canvas id="canvas" :class="{'select-object': canvasData.objDetected}"></canvas>
            <!-- <whiteBoardLayers v-if="false" :canvasData="canvasData"></whiteBoardLayers> -->
             <v-skeleton-loader v-if="getImgLoader" class="loader-img-canvas"
                max-width="300" min-width="300"
                type="image"></v-skeleton-loader>
            <!-- <v-progress-circular v-if="getImgLoader" class="loader-img-canvas light-blue" 
                indeterminate
                :rotate="3" :size="100" :width="3" color="info"></v-progress-circular> -->
        <svg class="helper" width="100%" height="100%" v-if="helperShow">
            <rect v-if="selectedOptionString === enumOptions.rectangle || selectedOptionString === enumOptions.select"
                  :x="helperStyle.x"
                  :y="helperStyle.y"
                  :width="helperStyle.width"
                  :height="helperStyle.height"
                  :class="helperClass"
                  :style="{'stroke': helperStyle.stroke}"/>            
            
            <line v-if="selectedOptionString === enumOptions.line"
                  :x1="helperStyle.x1"
                  :y1="helperStyle.y1"
                  :x2="helperStyle.x2"
                  :y2="helperStyle.y2"
                  :class="helperClass"
                  :style="{'stroke': helperStyle.stroke}"/>

            <ellipse v-if="selectedOptionString === enumOptions.circle"
                     :cx="helperStyle.cx"
                     :cy="helperStyle.cy"
                     :rx="helperStyle.rx"
                     :ry="helperStyle.ry"
                     :class="helperClass"
                     :style="{'stroke': helperStyle.stroke}"/>

            <rect v-if="selectedOptionString === enumOptions.select && showAnchors"
                  :x="(Number(helperStyle.xRaw) - 4) + 'px'"
                  :y="(Number(helperStyle.yRaw) - 4) + 'px'"
                  :width="8"
                  :height="8"
                  class="anchor anchor-top-left"/>
                  {{helperStyle.widthRaw}}
            <rect v-if="selectedOptionString === enumOptions.select && showAnchors"
                  :x="(Number(helperStyle.xRaw) + Number(helperStyle.widthRaw) -4) + 'px'"
                  :y="Number(helperStyle.yRaw - 4) + 'px'"
                  :width="8"
                  :height="8"
                  class="anchor anchor-top-right"/>
            <rect v-if="selectedOptionString === enumOptions.select && showAnchors"
                  :x="Number(helperStyle.xRaw - 4) + 'px'"
                  :y="(Number(helperStyle.yRaw) + Number(helperStyle.heightRaw) - 4) + 'px'"
                  :width="8"
                  :height="8"
                  class="anchor anchor-bottom-left"/>
            <rect v-if="selectedOptionString === enumOptions.select && showAnchors"
                  :x="(Number(helperStyle.xRaw) + Number(helperStyle.widthRaw) - 4) + 'px'"
                  :y="(Number(helperStyle.yRaw) + Number( helperStyle.heightRaw) - 4) + 'px'"
                  :width="8"
                  :height="8"
                  class="anchor anchor-bottom-right"/>
        </svg>
        <div v-if="getShowBoxHelper && dragData.length === 0" class="welcome-helper-top">
            <div class="top-helper">
                <pencilSVG class="icon-helper"/>
                <span v-language:inner="'studyRoom_boxs_top'"/>
            </div>
        </div>
        <div v-if="getShowBoxHelper && dragData.length === 0" class="welcome-helper-bottom">
            <div class="bottom-helper">
                <div class="bottom-helper-cont">
                    <uploadSVG class="icon-helper"/>
                    <div>
                        <p v-language:inner="'studyRoom_boxs_bottom_add'"/>
                        <span>
                            <span v-language:inner="'studyRoom_boxs_bottom_or'"/> 
                            <span @click="uploadImage" class="underlined" v-language:inner="'studyRoom_boxs_bottom_link'"/>
                        </span>
                    </div>

                </div>
            </div>
        </div>
        <div :style="{'top':helperStyle.top, 'left':helperStyle.left, 'flex-direction': isRtl ?  'row-reverse': ''}" class="text-helper-container" v-if="helperShow && selectedOptionString === enumOptions.text">
                <div style="width:240px;height:40px">
                    <input type="text" v-language:placeholder="'tutor_enter_text'"
                    v-model="helperStyle.text"
                    :class="[helperClass, helperStyle.id]"
                    :style="{'color': helperStyle.color, 'direction':isRtl ? 'rtl' : 'ltr' }"/>
                </div>
                <div style="width: 100px;height: 55px;">
                    <v-select
                        :items="textScales"
                        :label="sizeText"
                        append-icon='sbf-arrow-down'
                        v-model="fontSize"
                        item-text='text'
                        item-value='value'
                        right
                    ></v-select>
                </div>
            
        </div>
        <div class="equation-helper-container"
             :style="{'color': helperStyle.color, 'top':`${equationSizeY}px`, 'left':`${equationSizeX}px`}"
             v-if="helperShow && selectedOptionString === enumOptions.equation">
             <div>
                 <equation-mapper :injectToTextArea="injectToTextArea"></equation-mapper>
             </div>
            <div class="equation-text-area" style="justify-content: space-between;">
                <textarea id="textArea-tutoring" :class="[helperClass, helperStyle.id]"
                      v-model="helperStyle.text"
                      cols="50"
                      rows="6"></textarea>
                <vue-mathjax :class="[helperClass, helperStyle.id]"
                         v-show="!!helperStyle.text"
                         :formula="`$$${helperStyle.text}$$`"
                         class="math-jax"></vue-mathjax>
                <div style="align-self: flex-end;">
                    <v-btn @click="finishEquation" class="white--text" rounded color="#514f7d" v-language:inner="'studyRoom_equation_btn'"/>
                </div>
            </div>
            
        </div>
        <div class="iink-helper-container"
             :style="{'color': helperStyle.color, 'top':`${equationSizeY}px`, 'left':`${equationSizeX}px`}"
             v-if="helperShow && selectedOptionString === enumOptions.iink">
             <div>
                 <iink-drawer :helperClass="helperClass" :helperObj="helperStyle"></iink-drawer>
             </div>
            <div class="equation-text-area" style="justify-content: space-between;">
                <div style="align-self: flex-end;">
                    <v-btn @click="finishEquation" class="white--text" rounded color="#514f7d" v-language:inner="'studyRoom_equation_btn'"/>
                </div>
            </div>
        </div>
        </div>
    </div>
</template>

<script src="./whiteBoard.js"></script>


<style lang="less">
@import '../../../styles/colors.less';
    .canvas-container {
        .canvas-wrapper{
            /*rtl:ignore*/
            direction:ltr;
        }
        .formula-text{
                top: 25px;
                position: absolute;
                left: 100px;
            }
            .math-jax{
                margin-top: -84px;
                border: none;
            }
        #canvas {
            display: block;
            &.select-object {
                cursor: pointer;
            }
        }
        .loader-img-canvas{
            position: absolute;
            top: 20%;
            left: 0;
            right: 0;
            margin: 0 auto;
        }
        .helper {
            position: fixed;
            pointer-events: none;
            top: 0;
            left: 0;
            .anchor{
                fill:#4c59ff;
            }
            .rectangular-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
            .line-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
            .ellipse-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
        }
        .text-helper-container {
            position:fixed;
            display:flex;
            background: #FFF;
            height: 75px;
            width: 400px;
            justify-content: space-around;
            align-items: center;
            border-radius: 8px;
            .text-helper {
                position: fixed;
                border: 1px solid #7b7b7b;
                padding: 6px;
                outline: none;
                border-radius: 4px;
                font-family: "Open Sans", sans-serif;
                font-size: 20px;
            }
        }
        .welcome-helper-top{
            position: absolute;
            top: 13%;
            left: 42.5%;
            .top-helper{
                // box-shadow: 0px 4px 13px 0 rgba(0, 0, 0, 0.35);
                // background: white;
                border: 2px #90949c dashed;
                border-radius: 6px;
                width: 280px;
                height: 112px;
                display: flex;
                flex-direction: column;
                justify-content: space-evenly;
                align-items: center;
                padding: 10px;
                .icon-helper{
                    margin-top: 6px;
                    margin-left: 21px;
                    width: 42px;
                    height: 42px;
                    color: #90949c;
                }
                span{
                    padding-top: 10px;
                    font-size: 18px;
                    letter-spacing: -0.56px;
                    color: @color-main;
                }
            }
        }
        .welcome-helper-bottom{
            position: absolute;
            top: 46%;
            left: 38%;
            .bottom-helper{
                border-radius: 6px;
                // box-shadow: 0px 4px 13px 0 rgba(0, 0, 0, 0.35);
                // background: white;
                width: 448px;
                height: 180px;
                display: flex;
                flex-direction: column;
                justify-content: space-evenly;
                align-items: center;
                padding: 12px;
                .bottom-helper-cont{
                    width: 100%;
                    height: 100%;
                    border: 2px #90949c dashed;
                    border-radius: 6px;
                    display: flex;
                    flex-direction: column;
                    justify-content: space-evenly;
                    align-items: center;
                    text-align: center;
                    font-size: 18px;
                    line-height: 1.44;
                    letter-spacing: normal;
                    color: @color-main;     
                    .icon-helper{
                        width: 44px;
                        height: 42px;
                        color: #90949c;
                        margin: 11px 0;
                    }
                    .underlined{
                        cursor: pointer;
                        text-decoration: underline;
                    }
                    p{
                        margin: 0;
                    }
                }
            }
        }
        .equation-helper-container {
                position: fixed;
                background: #fff;
                padding: 10px;
                border: 1px solid #e1e1ef;
                border-radius: 4px;
                /*flex-direction: column;*/
                display: flex;
                flex-direction: row;
                .equation-text-area{
                    display: flex;
                    flex-direction: column;
                    .equation-helper {
                        border: 2px solid #c4c4ca;
                        padding: 6px;
                        outline: none;
                        border-radius: 4px;
                        font-family: "Open Sans", sans-serif;
                        font-size: 14px;
                        background: #fff;
                    }
                    .math-jax{
                        margin-right: auto;
                        margin-top: 10px;
                        border: none;
                    }
                }
            
        }
        .iink-helper-container {
                position: fixed;
                background: #fff;
                padding: 10px;
                border: 1px solid #e1e1ef;
                border-radius: 4px;
                flex-direction: column;
                display: flex;
                .equation-text-area{
                    display: flex;
                    flex-direction: column;
                    .equation-helper {
                        border: 2px solid #c4c4ca;
                        padding: 6px;
                        outline: none;
                        border-radius: 4px;
                        font-family: "Open Sans", sans-serif;
                        font-size: 14px;
                        background: #fff;
                    }
                    .math-jax{
                        margin-right: auto;
                        margin-top: 10px;
                        border: none;
                    }
                }
            
        }
    }
</style>