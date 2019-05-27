<template>
    <div class="canvas-container" id="canvasDiv">
        <div id="canvas-wrapper" class="canvas-wrapper" style="position:relative; overflow: auto;" :style="`width:${windowWidth}px;height:${windowHeight}px;`">
            <canvas id="canvas" :class="{'select-object': canvasData.objDetected}"></canvas>

        
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
        </svg>

        <div class="text-helper-container" v-if="helperShow && selectedOptionString === enumOptions.text">
            <input type="text" placeholder="Enter Some Text"
                   v-model="helperStyle.text"
                   :class="[helperClass, helperStyle.id]"
                   :style="{'color': helperStyle.color, 'top':helperStyle.top, 'left':helperStyle.left}"/>
        </div>
        <div class="equation-helper-container"
             :style="{'color': helperStyle.color, 'top':helperStyle.top, 'left':helperStyle.left}"
             v-if="helperShow && selectedOptionString === enumOptions.equation">
             <div>
                 <equation-mapper :injectToTextArea="injectToTextArea"></equation-mapper>
             </div>
            <div class="equation-text-area">
                <textarea id="textArea-tutoring" :class="[helperClass, helperStyle.id]"
                      v-model="helperStyle.text"
                      cols="50"
                      rows="6"></textarea>
                <vue-mathjax :class="[helperClass, helperStyle.id]"
                         v-show="!!helperStyle.text"
                         :formula="`$$${helperStyle.text}$$`"
                         class="math-jax"></vue-mathjax>
            </div>
            
        </div>
        </div>
        <div class="canvas-tabs">
            <div @click="changeTab(tab)"
                 class="canvas-tab"
                 v-for="(tab) in canvasTabs"
                 :key="tab.id"
                 :class="{'canvas-tabs-active': tab.id === getCurrentSelectedTab.id}">
                <button :id="tab.id">{{tab.name}}</button>
                <!-- <v-icon @click.stop="showTabOption(tab.id)">sbf-3-dot</v-icon>
                <div class="canvas-tab-option" :class="{'canvas-tab-option-active': tabEditId === tab.id}">
                    <div>
                        <div @click="renameTab(tab)">
                            <v-icon>sbf-close</v-icon>
                            <span>Rename</span>
                        </div>
                        <div @click="deleteTab(tab)">
                            <v-icon>sbf-close</v-icon>
                            <span>Delete</span>
                        </div>
                    </div>
                </div> -->
            </div>
             
             
        </div>
        <!--<div>-->
            <!--<ul>-->
                <!--<li v-for="(shape, index) in canvasData.dragData" :key="index">{{shape.type}}</li>-->
            <!--</ul>-->
        <!--</div>-->
    </div>
</template>

<script src="./whiteBoard.js"></script>


<style lang="less">
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
        .helper {
            position: fixed;
            pointer-events: none;
            top: 0;
            left: 0;
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
            .text-helper {
                position: fixed;
                border: 1px solid #7b7b7b;
                padding: 6px;
                outline: none;
                border-radius: 4px;
                font-family: "Open Sans", sans-serif;
                font-size: 14px;
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
        .canvas-tabs{
            position: fixed;
            bottom: 5px;
            width: 100%;
            height: 46px;
            background-color: #f0f0f7;
            display: flex;
            border-top: 1px solid #e1e1ef;
            .canvas-tab{
                display: flex;
                background-color: rgba(67, 66, 93, 0.12);
                padding: 0 20px;    
                border-right: 1px solid #c9c9c9;
                min-width: 100px;
                justify-content: center;
                position: relative;
                .canvas-tab-option{
                    position: absolute;
                    width: 127px;
                    height: 102px;
                    box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
                    bottom: 30px;
                    right: -80px;
                    display: none;
                    background-color: #ffffff !important;
                    &.canvas-tab-option-active{
                        display: unset;
                        z-index: 1;
                        border-radius: 4px;
                    }
                }
                &.canvas-tabs-active{
                    background-color: #FFF;
                    box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
                }
                button{
                    outline: none;
                }
                i{
                    font-size: 14px;
                    color: rgba(67, 66, 93, 0.25);
                }
            }
            
        }
    }
</style>
