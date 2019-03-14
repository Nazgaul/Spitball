<template>
    <div class="canvas-container" :style="`max-width:${canvasWidth}px;`" id="canvasDiv">
        <div class="nav-container elevation-2">
            <!--Select-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.pan}"
                            class="nav-action" @click="setOptionType(enumOptions.pan)">
                        <v-icon>sbf-pan</v-icon>
                    </button>
                </template>
                <span >Pan</span>
            </v-tooltip>

            <!--Select-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.select}"
                            class="nav-action" @click="setOptionType(enumOptions.select)">
                        <v-icon>sbf-mouse-pointer</v-icon>
                    </button>
                </template>
                <span >Select</span>
            </v-tooltip>

            <!--Text-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="{'active-tool': selectedOptionString === enumOptions.text}"
                             class="nav-action" @click="setOptionType(enumOptions.text)">
                        <v-icon>sbf-text-icon</v-icon>
                    </button>
                </template>
                <span >Text</span>
            </v-tooltip>
            <!--Equation-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="{'active-tool': selectedOptionString === enumOptions.equation}"
                             class="nav-action" @click="setOptionType(enumOptions.equation)">
                        <v-icon>sbf-equation-icon</v-icon>
                    </button>
                </template>
                <span>Equation</span>
            </v-tooltip>
            <!--Color Picker-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': showPickColorInterface}" class="nav-action"
                            @click="showColorPicker">
                        <v-icon class="selected-color"  :style="{ color: canvasData.color.hex}">sbf-color-picked</v-icon>
                        <!--<span class="selected-color" :style="{ backgroundColor: canvasData.color.hex}"></span>-->
                    </button>
                </template>
                <span >Color Picker</span>
            </v-tooltip>
            <slider-picker class="color-picker" :palette="predefinedColors" v-show="showPickColorInterface"
                           v-model="canvasData.color"/>
            <!--<button class="nav-action" @click="clearCanvas">clear</button>-->

            <!--Draw-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.draw}"
                            class="nav-action" @click="setOptionType(enumOptions.draw)">
                        <v-icon>sbf-pencil-empty</v-icon>
                    </button>
                </template>
                <span >Pen</span>
            </v-tooltip>

            <!--Line-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.line}"
                            class="nav-action" @click="setOptionType(enumOptions.line)">
                        <v-icon>sbf-connect-line</v-icon>
                    </button>
                </template>
                <span >Line</span>
            </v-tooltip>

            <!--Circle-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.circle}"
                            class="nav-action" @click="setOptionType(enumOptions.circle)">
                        <v-icon>sbf-elipse-stroke</v-icon>
                    </button>
                </template>
                <span>Circle</span>
            </v-tooltip>

            <!--Square-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.rectangle}"
                            class="nav-action" @click="setOptionType(enumOptions.rectangle)">
                        <v-icon>sbf-rectangle-stroke</v-icon>
                    </button>
                </template>
                <span>Square</span>
            </v-tooltip>

            <!--Upload Image-->
            <input class="nav-action" type="file" name="Image Upload" id="imageUpload" v-show="false"/>
                
            
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.image}"
                             class="nav-action" @click="setOptionType(enumOptions.image)">
                        <v-icon>sbf-upload</v-icon>
                    </button>
                </template>
                <span>Upload Image</span>
            </v-tooltip>

            <!--<button :class="[selectedOptionString === enumOptions.eraser ? 'active-tool' : '']"-->
                    <!--class="nav-action" @click="setOptionType(enumOptions.eraser)">-->
                <!--<v-icon>sbf-eraser-empty</v-icon>-->
            <!--</button>-->

            <!--Undo-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" class="nav-action" :class="{'disabled': dragData.length === 0}" @click="undo()">
                        <v-icon>sbf-undo</v-icon>
                    </button>
                </template>
                <span>Undo</span>
            </v-tooltip>

        </div>
        <div class="nav-container zoom-helper bottom-nav elevation-2">
            <!--Zoom-->
            <div class="zoom-container">   
                <v-tooltip right>
                      <template v-slot:activator="{on}">
                          <button v-on="on" class="nav-action" @click="doZoom(true)">
                              <v-icon>sbf-zoom-in</v-icon>
                          </button>
                      </template>
                      <span>zoom in</span>
                  </v-tooltip>
                  <v-tooltip right>
                      <template v-slot:activator="{on}">
                          <button v-on="on" class="nav-action" @click="doZoom(false)">
                              <v-icon>sbf-zoom-out</v-icon>
                          </button>
                      </template>
                      <span>zoom out</span>
                </v-tooltip>
            </div>
            
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <span v-on="on" class="nav-action zoom-text">
                        {{zoom}}%
                    </span>
                </template>
                <span>zoom</span>
            </v-tooltip>
        </div>
        <div style="position:relative">
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
            <textarea :class="[helperClass, helperStyle.id]"
                      v-model="helperStyle.text"
                      cols="50"
                      rows="3"></textarea>
            <vue-mathjax :class="[helperClass, helperStyle.id]"
                         v-show="!!helperStyle.text"
                         :formula="`$$${helperStyle.text}$$`"
                         class="math-jax"></vue-mathjax>
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
        .formula-text{
                top: 25px;
                position: absolute;
                left: 100px;
            }
            .math-jax{
                margin-top: -84px;
                border: none;
            }
        .nav-container {
            position: fixed;
            background-color: #FFFFFF;
            padding: 16px 0;
            display: flex;
            flex-direction: column;
            width: auto;
            margin-top: 20px;
            z-index: 5;

            &.bottom-nav{
                top: 625px;
            }
            .nav-action {
                padding: 12px 16px;
                outline: none!important;
                .v-icon {
                    color: #000000;
                }
                &.disabled{
                    pointer-events: none;
                    .v-icon {
                        color: gray;
                    }
                }
                &.active-tool{
                    .v-icon {
                        color:#2a79ff;
                    }
                }
                &.active-tool-svg{
                    svg {
                        fill:#2a79ff;
                    }
                }
                &:hover {
                    .v-icon {
                        color: #2a79ff;
                    }

                }
            }
            &.zoom-helper{
                max-width: 58px;
                min-width: 58px;
                padding: 16px 0px 0 0;
                .nav-action{
                    padding: 4px 0;
                    text-align: center;
                    &.zoom-text{
                        font-size:12px;
                        border-top:1px solid rgba(0, 0, 0, 0.16);
                        margin: 0 6px;
                    }
                }
                .zoom-container{
                    display: flex;
                    justify-content: space-evenly;
                    padding-bottom: 10px;
                    i{
                        font-size: 28px;
                    }
                }
            }
            .selected-color {
                font-size: 26px;
                cursor: pointer;
            }
            .color-picker {
                position: absolute;
                top: 106px;
                left: 80px;
                height: auto;
                .vc-compact-color-item {
                    list-style: none;
                    width: 28px;
                    height: 28px;
                    border-radius: 28px;
                    float: left;
                    margin-right: 5px;
                    margin-bottom: 5px;
                    position: relative;
                    cursor: pointer;
                }
            }

        }
        #canvas {
            display: block;
            &.select-object {
                cursor: pointer;
            }
        }
        .helper {
            position: absolute;
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
                position: absolute;
                border: 1px solid #7b7b7b;
                padding: 6px;
                outline: none;
                border-radius: 4px;
                font-family: "Open Sans", sans-serif;
                font-size: 14px;
            }
        }
        .equation-helper-container {
                position: absolute;
                background: #fff;
                padding: 10px;
                border: 1px solid #e1e1ef;
                border-radius: 4px;
                display: flex;
                flex-direction: column;
                padding: 10px;
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
</style>
